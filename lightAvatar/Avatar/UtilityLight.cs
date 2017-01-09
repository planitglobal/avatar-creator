using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class UtilityLight : MonoBehaviour
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
            {
                t=Instantiate(t);
            }          
            return t == null ? null : t;
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
