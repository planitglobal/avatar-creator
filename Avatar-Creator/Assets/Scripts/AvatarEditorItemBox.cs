using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class AvatarEditorItemBox : MonoBehaviour {

    public Transform Panel;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    

    private GameObject LoadGameObject(string assetPath)
    {

        GameObject t = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
        return t == null ? null : Instantiate(t);
    }
    const string CHARACTER_PATH = "Assets/Character/";
    public List<GameObject> PopulateBox(string bodyPart)
    {
        var assets = new List<GameObject>();
        foreach (var file in Directory.GetFiles(CHARACTER_PATH+bodyPart))
        {
            var asset = LoadGameObject(file);
            if (asset == null) continue;
            assets.Add(asset);
        }
        return assets;
    }
    
    public void ShowUI(string bodyPart)
    {
        PopulateBox(bodyPart);
    }


  

}
