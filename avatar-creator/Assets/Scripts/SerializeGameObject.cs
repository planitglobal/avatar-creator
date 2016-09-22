using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public static class SerializeGameObject {

        public static byte[] Serialize(GameObject baseObject, bool saveMaterial=true)
        {
            return baseObject.SaveObjectTree();
        }

        public static void SaveSerializeInFile(GameObject baseObject, string pathSaveFile)
        {
            File.WriteAllBytes(pathSaveFile, Serialize(baseObject));
        }

        public static void Deserialize(byte[] data) 
        {
            data.LoadObjectTree();     
        }

        public static void DeserializeByFile(string filePath)
        {
            Deserialize(File.ReadAllBytes(filePath));
        }
    }   
}
