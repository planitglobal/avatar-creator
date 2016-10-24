using System;
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
            {
                t=Instantiate(t);
            }          
            return t == null ? null : t;
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

        /// <summary>
        /// Create a sprite with an gameobject
        /// </summary>
        /// <param name="element">The gameobject source for the sprite</param>
        /// <param name="textureWidth">The width of the texture</param>
        /// <param name="textureHeight">The height of the texture</param>
        /// <param name="backgroundColor">Background color for the sprite (if none, transparent)</param>
        /// <param name="clone">Clone the object before making the sprite</param>
        /// <param name="elementDistance">Distance of the gameobject from the camera</param>
        /// <returns></returns>
        //public static Sprite MakeSprite(GameObject element, int textureWidth, int textureHeight,Color backgroundColor=new Color(), bool clone = false, float elementDistance = .1f)
        //{

        //    var layerUsed = LayerMask.NameToLayer("Ignore Raycast");
        //    var cameraObject = new GameObject("CameraSnapshot");
        //    cameraObject.AddComponent<Camera>();
        //    var cameraSnapShot = cameraObject.GetComponent<Camera>();
        //    element = clone ? Instantiate(element) : element;
        //    cameraSnapShot.cullingMask = 1 << layerUsed;
        //    SetLayerRecursively(element, layerUsed);
        //    cameraSnapShot.enabled=false;
        //    cameraSnapShot.clearFlags = CameraClearFlags.SolidColor;
        //    cameraSnapShot.backgroundColor = backgroundColor == new Color() ? Color.clear: backgroundColor;
        //    element.transform.parent = cameraSnapShot.transform;
        //    element.transform.rotation=new Quaternion(0,0,0,0);
        //    var vector = element.GetComponent<Renderer>();

        //    if (vector != null)
        //    {
        //        element.transform.RotateAround(vector.bounds.center, new Vector3(0, 0, 0), 90);
        //    }
        //    element.transform.localPosition = new Vector3(0, 0, elementDistance);


        //    cameraSnapShot.targetTexture = RenderTexture.GetTemporary(textureWidth, textureHeight, 16);
        //    cameraSnapShot.Render();
        //    RenderTexture saveActive = RenderTexture.active;
        //    RenderTexture.active = cameraSnapShot.targetTexture;
        //    int width = cameraSnapShot.targetTexture.width;
        //    int height = cameraSnapShot.targetTexture.height;
        //    Texture2D texture = new Texture2D(width, height);
        //    texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        //    texture.Apply();
        //    RenderTexture.active = saveActive;
        //    RenderTexture.ReleaseTemporary(cameraSnapShot.targetTexture);
        //    DestroyImmediate(element);
        //    DestroyImmediate(cameraObject);
        //    Rect rec = new Rect(0, 0, texture.width, texture.height);
        //    return  Sprite.Create(texture, rec, new Vector2(0, 0));
        //}

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
