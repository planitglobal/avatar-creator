using System;
using System.Threading;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Assets.Scripts
{
    public class Utility : MonoBehaviour
    {

        /// <summary>
        /// Instantiate an object 
        /// </summary>
        /// <param name="assetPath">Absolute path to the asset</param>
        /// <returns></returns>
        public static GameObject LoadGameObject(string assetPath)
        {
            var t = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            return t == null ? null : Instantiate(t);
        }


        /// <summary>
        /// Permute a GameObject with another on
        /// </summary>
        /// <param name="sourceObject">The gameobject to replace</param>
        /// <param name="newGameObject">The new gameobject</param>
        /// <param name="removeSourceGameObject">The Source GameObject will be destroyed after the permute</param>
        /// <param name="permutePosition">Set the newGameObject with the postion of the source one</param>
        /// <param name="permuteParent">Set the newGameObject with the parent of the source one</param>
        public static void PermuteGameObject(ref GameObject sourceObject, GameObject newGameObject, bool removeSourceGameObject = true, bool permutePosition = true, bool permuteParent = true)
        {
            try
            {
                if (permutePosition)
                    newGameObject.transform.position = sourceObject.transform.position;
                if (permuteParent)
                    newGameObject.transform.parent = sourceObject.transform.parent;
                if (removeSourceGameObject)
                    Destroy(sourceObject);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                throw;
            }
         
        }

        public static void ChangeColor(GameObject element, Color color)
        {
            element.GetComponent<Renderer>().material.color = color;
        }

        /// <summary>
        /// Return the int value of a hexadecimal string
        /// </summary>
        /// <param name="hex">Hexadecimal string</param>
        /// <returns></returns>
        public static int HexToDouble(string hex)
        {
            return int.Parse(hex.Replace("#", ""), System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Return the hexadecimal value of an int
        /// </summary>
        /// <param name="number">The int number to convert</param>
        /// <returns></returns>
        public static string IntToHex(int number)
        {
            return number.ToString("X3");
        }


        public static Sprite MakeSprite(GameObject element, int textureWidth, int textureHeight,Color backgroundColor=new Color(), bool clone = false, float elementDistance = 3)
        {
            element = clone ? Instantiate(element): element;
            var cameraObject= new GameObject("CameraSnapshot");
            cameraObject.AddComponent<Camera>();
            var cameraSnapShot = cameraObject.GetComponent<Camera>();
            cameraSnapShot.enabled=false;
            cameraSnapShot.clearFlags = backgroundColor == new Color() ? CameraClearFlags.Depth: CameraClearFlags.SolidColor;
            cameraSnapShot.backgroundColor = backgroundColor;
            element.transform.parent = cameraSnapShot.transform;
            element.transform.localPosition = new Vector3(0, 0, elementDistance);
            SetLayerRecursively(element, cameraObject.layer);
            cameraSnapShot.targetTexture = RenderTexture.GetTemporary(textureWidth, textureHeight, 16);
            cameraSnapShot.Render();
            RenderTexture saveActive = RenderTexture.active;
            RenderTexture.active = cameraSnapShot.targetTexture;
            int width = cameraSnapShot.targetTexture.width;
            int height = cameraSnapShot.targetTexture.height;
            Texture2D texture = new Texture2D(width, height);
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = saveActive;
            RenderTexture.ReleaseTemporary(cameraSnapShot.targetTexture);
            DestroyImmediate(element);
            DestroyImmediate(cameraObject);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            return  Sprite.Create(texture, rec, new Vector2(0, 0), .01f);
        }

        /// <summary>
        /// Change the layer of an element Recursively
        /// </summary>
        /// <param name="o">The GameObject to change layer</param>
        /// <param name="layer">The index of the new layer</param>
        static void SetLayerRecursively(GameObject o, int layer)
        {
            foreach (Transform t in o.GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = layer;
        }
    }
}
