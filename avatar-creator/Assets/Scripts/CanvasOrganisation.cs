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
    //
    //Colorbox variables START
    //
    private const int ColorboxbaseX = -270;// best way : Find the colorboxpanel x and y 
    private const int ColorboxbaseY = 0;

    private const int nbcolors = 28;

    private int x = 0;
    private int y = 0;

    private int ColorboxButtonWidth = 0;
    private int ColorboxButtonHeight = 0;

    private const float ColorboxButtonScaleX = 0.2f;
    private const float ColorboxButtonScaleY = 0.3f;

    //
    //Colorbox variables END
    //

    //
    //Assetsbox variables START
    //
    private const float AssetsboxButtonScaleX = 1.1f;
    private const float AssetsboxButtonScaleY = 1.1f;

    private const int AssetsboxbaseX = -228;
    private const int AssetsboxbaseY = 77;

    private GameObject facePanel;
    private GameObject bodyPanel;
    private GameObject clothesPanel;
    private GameObject accessoriesPanel;

    private string[] panelnames = new string[] {"FacePanel","ClothesPanel", "AccessoriesPanel"};


    //private GameObject[,];
    //
    //Assetsbox variables END
    //

    // Use this for initialization
    void Start()
    {
        //Set up values for the colorbox and generates it
        ColorBoxSetUp();
        GenerateColorbox();

        //Set up values for the assetsbox and generates it
        HidePanels(panelnames[0]);
        Debug.Log("1");

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void rotateGuy(string direction)
    {
        int addrotate = 25;
        GameObject temp = GameObject.Find("guy");

        float yRotation = temp.transform.eulerAngles.y;

        if (direction == "right")
            yRotation -= addrotate;
        else
            yRotation += addrotate;

        temp.transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
    }

    public void onClickCategoryButton(string panelname)
    {
        deleteAssetPanelContent();
        HidePanels(panelname);
    }

    void HidePanels(string exception)
    {
        foreach (var name in panelnames)
        {
            GameObject temp = GameObject.Find(name);
            if (name != exception)
              temp.transform.localScale = new Vector2(0, 0);
            else
                temp.transform.localScale = new Vector2(1, 1);
        }
    }

    public void onClickSubCategoryButton(string type)//rename type
    {
        deleteAssetPanelContent();
       // if (GameObject.Find("AssetsPanel").transform.childCount == 0)
        GenerateAssetBox(type);
        
       
    }
    /// <summary>
    /// Set up variables of the x/y axes used to place the first button
    /// </summary>
    void ColorBoxSetUp()
    {
        x = ColorboxbaseX;
        y = ColorboxbaseY;

    }

    void AssetsBoxSetUp()
    {
        x = AssetsboxbaseX;
        y = AssetsboxbaseY;
    }

    void deleteAssetPanelContent()
    {
        int childs = GameObject.Find("AssetsPanel").transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.Destroy(GameObject.Find("AssetsPanel").transform.GetChild(i).gameObject);
        }

    }
    void GenerateColorbox()
    {
        //Math.Round(Math.Sqrt(nbcolors)
        List<string> colors = GetColorList(nbcolors).OrderByDescending(a => a).ToList();


        for (int i = 0; i < colors.Count; i++)
        {
            string color = colors[i];
            int[] sizes = CreateButton(x, y,"ColorPanel","", "ColorButton_" + color, color,ColorboxButtonScaleX,ColorboxButtonScaleY);
            ColorboxButtonWidth = sizes[0];
            ColorboxButtonHeight = sizes[1]; //useless for the moment

            x += ColorboxButtonWidth;
        }


    }
    void GenerateAssetBox(string type)
    {
        if (type != null)
        {
            AssetsBoxSetUp();
            List<GameObject> assets = new List<GameObject>(GetListObject(type)); //temp
            int[] sizes;
            for (int i = 1; i <= assets.Count; i++)
            {

                var asset = assets[i - 1];
                sizes = CreateButton(x, y, "AssetsPanel", "", asset.name.Replace("(Clone)", ""), "#FFFFFF",
                    AssetsboxButtonScaleX, AssetsboxButtonScaleY,
                    false, Utility.MakeSprite(asset, 300, 300));
                if (i%5 == 0)
                {
                    x = AssetsboxbaseX;
                    y -= sizes[1];
                }
                else
                {
                    x += sizes[0];
                }

            }
        }
    }

    private int[] CreateButton(int posx, int posy, string parent = "", string text ="", string name = "", string color = "#FFFFFF", float scaleX = 1, float scaleY = 1,bool isflat = true, Sprite sprite = null)
    {
        //get button from asset
        Button assetTemplate = AssetDatabase.LoadAssetAtPath<Button>("Assets/UI/TemplateButton.prefab");

        //used to transform color into hexadecimal color
        Color hexacolor = new Color();
        ColorUtility.TryParseHtmlString(color, out hexacolor);

        //button initialization and settings
        var newButton = CanvasOrganisation.Instantiate(assetTemplate);

        newButton.transform.SetParent(GameObject.Find(parent).transform);

        if(isflat) newButton.image.sprite = null;
        if(sprite) newButton.image.sprite = sprite;
        newButton.image.color = hexacolor;
        newButton.name = name;
        newButton.transform.localPosition = new Vector2(posx, posy);
        newButton.GetComponentInChildren<Text>().text = text;

        //scale
        newButton.transform.localScale = new Vector3(scaleX, scaleY);

        //temp var used to get width and height
        RectTransform rt = (RectTransform)newButton.transform;

        int[] buttonsizes = new int[] { Convert.ToInt32(rt.rect.width * scaleX), Convert.ToInt32(rt.rect.height * scaleY)};

        return buttonsizes;
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
        return Directory.GetFiles(CharacterPath + bodyPart).Select(file => Utility.LoadGameObject(file)).Where(file => file != null).ToList();
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
