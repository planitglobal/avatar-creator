using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts;
using UnityEditor;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class CanvasOrganisation : MonoBehaviour
{

    

	// Use this for initialization
	void Start () {

       
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void GenerateColorBox()
    {
        
    
    }

    void OnGUI()
    {
        List<string> test = GetColorList(40,"#c9ff00","#000000");
        if (GUILayout.Button("Press Me"))
            foreach (string color in test)
            {
                Debug.Log(color);
            }
            

    }

    public void CreateButton(Transform panel, Vector3 position, Vector2 size, UnityEngine.Events.UnityAction method)
    {

    }

    public void onClickFaceButton()
    {

        Button test2 = AssetDatabase.LoadAssetAtPath<Button>("Assets/UI/TemplateButton.prefab");
             

        var test= CanvasOrganisation.Instantiate(test2);
        test.transform.position = new Vector2(400, 200);
      //  test.transform.parent = GameObject.Find("UI").transform;
        test.transform.SetParent(GameObject.Find("UI").transform);
        Debug.Log("CA A MARCHE");
        //
        
        
        //test.GetComponent<Renderer>().material.color = Color.green;
        //Color couleur = new Color();
        //ColorUtility.TryParseHtmlString("#c9ff00",out couleur);
        

    }

    private void CreateButton(int posx, int posy, string text)
    {

        
    }

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
        var colorList = new List<string>();
        var colorShowed = (Utility.HexToDouble(end) - Utility.HexToDouble(start)) / numberOfColors;

        for (var i = 0; i < numberOfColors - 1; i++)
        {
            colorList.Add("#" + Utility.IntToHex(colorShowed * i));
        }
        //Add the last color to the list
        colorList.Add(end);

        return colorList;
    }

}
