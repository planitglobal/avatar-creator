using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Assets.Scripts
{
    public class Database : MonoBehaviour
    {
        public static Database DataBaseObject;
        private const string ServerPath = Config.PhpServerFile;
        public Dictionary<string,string> SelectResult;

        private Database()
        {
            DataBaseObject = this;
        }

        public void SaveStrings(Dictionary<string,string> input, string tableName)
        {                 
            StartCoroutine(Insert(input,tableName));
        }

        private IEnumerator<WWW> Insert(Dictionary<string, string> input, string tableName)
        {
            WWWForm form = new WWWForm();
            foreach (var value in input)
            {
                form.AddField(value.Key, value.Value);
            }
            var www = new WWW(ServerPath + "?table=" + tableName + "&type=insert",form);
            yield return www;
            if (www.text=="1")
                Debug.Log("Action completed on : "+tableName);
            else
                Debug.LogError(www.text);
        }

        private IEnumerator<WWW> Select(WWW www, Action DoAfter=null)
        {
            yield return www;
            foreach (var result in www.text.Split(','))
            {
                SelectResult.Add(result.Split('/')[0], result.Split('/')[1]);
            }
            DoAfter();
        }

    }
}