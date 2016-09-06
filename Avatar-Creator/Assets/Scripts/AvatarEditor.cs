using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class AvatarEditor : MonoBehaviour
    {
        private const string CharacterPath = "Assets/Character/";
        public Transform Panel;
        public static GameObject SelectedGameObject;

        /// <summary>
        /// Return a list of items corresponding at a body part
        /// </summary>
        /// <param name="bodyPart">The object type</param>
        /// <returns></returns>
        public List<GameObject> GetListObject(string bodyPart)
        {
            return Directory.GetFiles(CharacterPath + bodyPart).Select(file => Utility.LoadGameObject(file)).ToList();
        }

        /// <summary>
        /// Add a scale to an element
        /// </summary>
        /// <param name="addScale">The scale (height and width) added to the object</param>
        public void ChangeSize(float addScale)
        {
            try
            {
                SelectedGameObject.transform.localScale = new Vector3(SelectedGameObject.transform.localScale.x + addScale, SelectedGameObject.transform.localScale.y + addScale, SelectedGameObject.transform.localScale.z);
            }
            catch (Exception)
            {            
                throw new ArgumentNullException("The SelectedGameObject value is null");
            }
        }

        /// <summary>
        /// Get a lit of colors 
        /// </summary>
        /// <param name="numberOfColors">The number of colors</param>
        /// <param name="start">The start color</param>
        /// <param name="end">The end color</param>
        /// <returns></returns>
        public List<string> GetColorList(int numberOfColors, string start = "#000000", string end = "#FFFFFF")
        {
            var colorList=new List<string>();
            var colorShowed = (Utility.HexToDouble(end) - Utility.HexToDouble(start)) / numberOfColors;

            for (var i = 0; i < numberOfColors-1; i++)
            {
                colorList.Add("#"+Utility.IntToHex(colorShowed * i));
            }
            //Add the last color to the list
            colorList.Add(end);

            return colorList;
        }
    }
}
