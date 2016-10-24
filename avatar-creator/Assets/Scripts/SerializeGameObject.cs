using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.Scripts
{
    public static class SerializeGameObject {
        // Convert an object to a byte array
        private static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }
        private static object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = (object)binForm.Deserialize(memStream);

            return obj;
        }
        public static byte[] Serialize(GameObject baseObject, bool saveMaterial=true)
        {
            var final=new List<SerializedGameObject>();
            final.Add(new SerializedGameObject(baseObject));
            foreach (Transform child in baseObject.GetComponentsInChildren<Transform>(true))
                    final.Add(new SerializedGameObject(child.gameObject));

            return ObjectToByteArray(final);
        }

        public static void SaveSerializeInFile(GameObject baseObject, string pathSaveFile)
        {
            //File.WriteAllBytes(pathSaveFile, Serialize(baseObject));
        }

        public static void Deserialize(byte[] data, GameObject Parent)
        {
            var deserializedList = ByteArrayToObject(data) as List<SerializedGameObject>;

            foreach (var serializedGameObject in deserializedList)
            {
                var obj= Utility.FindAll(serializedGameObject.Name,Parent);
                obj.SetActive(serializedGameObject.IsActive);
                obj.transform.localPosition = new Vector3(serializedGameObject.LocalPositionX, serializedGameObject.LocalPositionY, serializedGameObject.LocalPositionZ);
                obj.transform.localRotation = new Quaternion(serializedGameObject.LocalRotationX, serializedGameObject.LocalRotationY, serializedGameObject.LocalRotationZ, serializedGameObject.LocalRotationW);
                obj.transform.position = new Vector3(serializedGameObject.PositionX, serializedGameObject.PositionY, serializedGameObject.PositionZ);
                obj.transform.rotation = new Quaternion(serializedGameObject.RotationX, serializedGameObject.RotationY, serializedGameObject.RotationZ, serializedGameObject.RotationW);
                obj.transform.localScale = new Vector3(serializedGameObject.LocalScaleX, serializedGameObject.LocalScaleY, serializedGameObject.LocalScaleZ);

                if (serializedGameObject.MaterialColor != null)
                    obj.transform.GetComponent<Renderer>().material.color = Utility.HexToColor(serializedGameObject.MaterialColor);

            }

        }

        public static void DeserializeByFile(string filePath)
        {
            //Deserialize(File.ReadAllBytes(filePath));
        }

        
    }

    [Serializable]
    public class SerializedGameObject
    {
       public SerializedGameObject(GameObject baseObject)
        {
            PositionX = baseObject.transform.position.x;
            PositionY = baseObject.transform.position.y;
            PositionZ = baseObject.transform.position.z;

            RotationW = baseObject.transform.rotation.w;
            RotationX = baseObject.transform.rotation.x;
            RotationY = baseObject.transform.rotation.y;
            RotationZ = baseObject.transform.rotation.z;

            LocalPositionX = baseObject.transform.localPosition.x;
            LocalPositionY = baseObject.transform.localPosition.y;
            LocalPositionZ = baseObject.transform.localPosition.z;

            LocalRotationW = baseObject.transform.localRotation.w;
            LocalRotationX = baseObject.transform.localRotation.x;
            LocalRotationY = baseObject.transform.localRotation.y;
            LocalRotationZ = baseObject.transform.localRotation.z;


            LocalScaleX = baseObject.transform.localScale.x;
            LocalScaleY = baseObject.transform.localScale.y;
            LocalScaleZ = baseObject.transform.localScale.z;
         

            Name = baseObject.name;
            IsActive=baseObject.activeSelf;
            if (baseObject.transform.parent!=null)
                Parent = baseObject.transform.parent.name;
            if (baseObject.transform.GetComponent<Renderer>() != null)
                MaterialColor = Utility.ColorToHex(baseObject.transform.GetComponent<Renderer>().material.color);

        }


        public float PositionX;
        public float PositionY;
        public float PositionZ;

        public  string Parent;

        public float RotationW;
        public float RotationX;
        public float RotationY;
        public float RotationZ;

        public float LocalRotationW;
        public float LocalRotationX;
        public float LocalRotationY;
        public float LocalRotationZ;

        public float LocalPositionX;
        public float LocalPositionY;
        public float LocalPositionZ;


        public float LocalScaleX;
        public float LocalScaleY;
        public float LocalScaleZ;

        public string MaterialColor;

        public string Name;
        public bool IsActive;
    }
}
