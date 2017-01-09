using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Assets.Scripts
{
    public class Database : MonoBehaviour
    {
        public static Database DataBaseObject;
        public string SelectResult;

        public static Database GetDatabase()
        {
            return DataBaseObject == null
                ? DataBaseObject = new GameObject("Database").AddComponent<Database>().GetComponent<Database>()
                : DataBaseObject;
        }

        public void LoadAvatar(int userId, Action doAfter = null)
        {
            var www = new WWW(Config.Api + "users/avatar/" + userId);
            StartCoroutine(Execute(www, doAfter));
        }
        public void SaveAvatar(int userId, string avatar)
        {
            var url = Config.Api + "users/avatar/" + userId + "/edit?avatar=" + WWW.EscapeURL(avatar);
            var www = new WWW(url);
            StartCoroutine(Execute(www));
        }

        private IEnumerator<WWW> Execute(WWW www, Action doAfter = null)
        {
            yield return www;
            SelectResult = www.text;
            if (doAfter != null)
                doAfter();
        }
    }
}