
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Assets.Scripts
{

    public class LoadAvatar : MonoBehaviour
    {
        private static string _gender;
        private static GameObject _male;
        private static GameObject _female;
        public static void Load(int userId = 1)
        {
            var folderpath = "Mannequins/";
            _male = UtilityLight.LoadGameObject(folderpath + "slim_male");
            _female = UtilityLight.LoadGameObject(folderpath + "slim_female");
            GenerateModel("female");
            Database.GetDatabase().LoadAvatar(userId, PlaceModelIntoCamera);
        }

        private static void PlaceModelIntoCamera()
        {
            SerializeGameObject.Deserialize(Convert.FromBase64String(Database.GetDatabase().SelectResult), GameObject.Find(Config.ModelName));
            GameObject.Find(Config.ModelName).transform.localPosition = new Vector3(270, -600, 70);
            GameObject.Find(Config.ModelName).transform.localEulerAngles = new Vector3(0, 210, 0);
            GameObject.Find(Config.ModelName).transform.localScale = new Vector3(100, 100, 100);
        }
        public static void GenerateModel(string type)
        {
            _gender = type;
            if (GameObject.Find(Config.ModelName))
                Destroy(GameObject.Find(Config.ModelName));
            GameObject newModel;
            if (_gender == "male")
            {
                newModel = _male;
                if (_female != null)
                    _female.SetActive(false);
            }
            else
            {
                newModel = _female;
                if (_male != null)
                    _male.SetActive(false);
            }
            newModel.transform.name = Config.ModelName;

            //Set the default panel
            newModel.transform.SetParent(GameObject.Find("BuisnessCamera").transform);

        }
    }
}