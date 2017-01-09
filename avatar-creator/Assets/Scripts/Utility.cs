using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if(Path.GetExtension(assetPath)!="")
                assetPath= assetPath.Replace(Path.GetExtension(assetPath), "");
            var t = Resources.Load<GameObject>(assetPath);
            if (t != null)
                t=Instantiate(t);
            return t == null ? null : t;
        }
        /// <summary>
        /// Get a lit of colors 
        /// </summary>
        /// <param name="numberOfColors">The number of colors</param>
        /// <param name="saturation"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static List<string> GetColorList(float brightness, float saturation = 80, int numberOfColors = 100)
        {
            var colorList = new List<string>();
            for (float i = 0; i < numberOfColors; i++)
            {
                var color = Color.HSVToRGB(i / 100, saturation / 100, brightness / 100);
                colorList.Add(ColorToHex(color));
            }
            return colorList;
        }

        /// <summary>
        /// Get the name without number
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetNameWithoutNumber(string name)
        {
            return name.Contains("_") ? name.Substring(0, name.LastIndexOf("_", StringComparison.Ordinal)) : name;
        }

        public static GameObject GetActiveElement(string type)
        {
            type = GetNameWithoutNumber(type);
            foreach (var element in GameObject.Find(Config.ModelName).GetComponentsInChildren<Transform>())
                if (GetNameWithoutNumber(element.name) == type)
                    return GameObject.Find(element.name);
            return null;
        }


        /// <summary>
        /// Permute a GameObject with another on
        /// </summary>
        /// <param name="sourceObject">The gameobject to replace</param>
        /// <param name="newGameObject">The new gameobject</param>
        /// <param name="removeSourceGameObject">The Source GameObject will be destroyed after the permute</param>
        /// <param name="permutePosition">Set the newGameObject with the postion of the source one</param>
        /// <param name="permuteParent">Set the newGameObject with the parent of the source one</param>
        public static void PermuteGameObject(ref GameObject sourceObject, GameObject newGameObject, bool instanciate = false, bool removeSourceGameObject = true, bool permuteName = true, bool permutePosition = true, bool permuteParent = true)
        {
            try
            {
                if (instanciate)
                    newGameObject = Instantiate(newGameObject);
                if (permutePosition)
                    newGameObject.transform.position = sourceObject.transform.position;
                if (permuteParent)
                    newGameObject.transform.parent = sourceObject.transform.parent;
                newGameObject.transform.localRotation = sourceObject.transform.localRotation;
                newGameObject.transform.localScale = sourceObject.transform.localScale;
                if (permuteName)
                    newGameObject.transform.name = sourceObject.transform.name;
                if (removeSourceGameObject)
                    Destroy(sourceObject);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                throw;
            }
         
        }

        public static void ChangeColor(GameObject element, string color, string[] exception)
        {
            Color hexacolor = new Color();
            ColorUtility.TryParseHtmlString(color, out hexacolor);
            foreach (Transform t in element.GetComponentsInChildren<Transform>(true))
            {
                if(exception.Any(a=>a== t.transform.name) && element.name!= t.transform.name)
                    continue;

                  
                  
                if (GetNameWithoutNumber(element.name) == "BODY")
                {
                    if (GetActiveElement("NOSE").GetComponent<Renderer>() != null)
                        GetActiveElement("NOSE").GetComponent<Renderer>().material.color = hexacolor;
                    if (GetActiveElement("EARS").GetComponent<Renderer>() != null)
                        GetActiveElement("EARS").GetComponent<Renderer>().material.color = hexacolor;
                }
                if (t.GetComponent<Renderer>() != null)
                    t.GetComponent<Renderer>().material.color=hexacolor;

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

        public static Sprite MakeSprite(string IconPath)
        {
            IconPath = "Character/Materials/Icon/icon_" + Path.GetFileNameWithoutExtension(IconPath).ToLower();
            var texture = Resources.Load<Texture2D>(IconPath);
            if (texture == null)
            {
                Debug.LogWarningFormat("ICON " + IconPath + " NOT FOUND");
                return null;
            }

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        public static string ColorToHex(Color32 color)
        {
            return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        }


        public static GameObject FindAll(string name, GameObject into=null)
        {
            if (into == null)
            {
                var gameobject = Resources.FindObjectsOfTypeAll(typeof(GameObject)).FirstOrDefault(a => a.name == name);
                if (gameobject != null)
                    return gameobject as GameObject;
            }
            else
            {
                foreach (var element in into.GetComponentsInChildren<Transform>(true))
                {
                    if (element.name == name)
                        return element.gameObject;
                }
            }
            return null;
        }


        public static Color HexToColor(string hex)
        {
            if (hex == null)
                return Color.white;
            hex = hex.Replace("#", "");
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}
