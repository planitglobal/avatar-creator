using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Assets.Scripts;
using UnityEditor;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class CanvasOrganisation : MonoBehaviour
{
    private int x = 0;
    private int y = 0;
    private int nbcolors = 300;
    private int width = 0;
    private int height = 0;
    private float scalex = 0.1f;
    private float scaley = 0.3f;
    private const int baseX = -130;
    private const int baseY = 130;


    // Use this for initialization
    void Start()
    {
        resetX();
        resetY();
        GenerateColorbox();


    }

    // Update is called once per frame
    void Update()
    {

    }

    void resetX()
    {
        x = baseX;
    }

    void resetY()
    {
        y = baseY;
    }
    public void GenerateColorbox()
    {
        //Math.Round(Math.Sqrt(nbcolors)
        List<string> colors = GetColorList(nbcolors).OrderByDescending(a => a).ToList();


        for (int i = 0; i < colors.Count; i++)
        {

        
            string color = colors[i];
            CreateButton(x, y, "", color);
            x += width;


        }

        //GetComponent<Renderer>().material.color = Color.black;
        //var temp = test.colors.normalColor;
        //temp = Color.red;
        //Debug.Log("CA A MARCHE");

        //Color couleur = new Color();
        //ColorUtility.TryParseHtmlString("#c9ff00",out couleur);
    }

    public void onClickFaceButton()
    {

    }

    private void CreateButton(int posx, int posy, string name, string color)
    {
        //get button from asset
        Button assetTemplate = AssetDatabase.LoadAssetAtPath<Button>("Assets/UI/TemplateButton.prefab");

        //used to transform color into hexadecimal color
        Color hexacolor = new Color();
        ColorUtility.TryParseHtmlString(color, out hexacolor);

        //button initialization and settings
        var colorButton = CanvasOrganisation.Instantiate(assetTemplate);
        colorButton.transform.SetParent(GameObject.Find("ColorPanel").transform);
        colorButton.image.sprite = null;
        colorButton.image.color = hexacolor;
        colorButton.name = "ColorButton_" + color;
        colorButton.transform.localPosition = new Vector2(posx, posy);
        colorButton.GetComponentInChildren<Text>().text = "";



        //scale
        colorButton.transform.localScale = new Vector3(scalex, scaley);

        //temp var used to get width and height
        RectTransform rt = (RectTransform)colorButton.transform;

        width = Convert.ToInt32(rt.rect.width * scalex);
        height = Convert.ToInt32(rt.rect.height * scaley);




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

        for (var i = 1; i < numberOfColors + 1; i++)
        {
            colorList.Add("#" + Utility.IntToHex(Utility.HexToDouble(start) + colorShowed * i));
        }
        return colorList;
    }

}
