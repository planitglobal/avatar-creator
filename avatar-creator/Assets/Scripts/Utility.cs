using System;
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


    }
}
