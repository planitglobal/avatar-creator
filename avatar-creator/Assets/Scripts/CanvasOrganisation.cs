using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts;
using UnityEditor;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class CanvasOrganisation : MonoBehaviour
{
    //
    //Colorbox variables START
    //
  
    private readonly string[] _skincolors = new string[]{"#FFDFC4","#F0D5BE","#EECEB3","#E1B899","#E5C298","#FFDCB2","#E5B887","#E5A073","#E79E6D","#DB9065","#CE967C","#C67856","#BA6C49","#A57257","#F0C8C9","#DDA8A0","#B97C6D","#A8756C","#AD6452","#5C3836","#CB8442","#BD723C","#704139","#870400","#710101", "#A3866A", "#430000","#5B0001","#000000"};

    private int _colorboxButtonWidth = 0;

    //private int _colorboxButtonHeight = 0;

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

    private readonly string[] _panelnames = new string[] {"FacePanel","ClothesPanel", "AccessoriesPanel"};

    private string _itemSelected = "BODY";

    

    public readonly string[] ChildrenException = new string[]{"EYES","MOUTH","SHIRT","THROUSERS","GLASSES","HAIR","BEARD","CAPS","JEWELRY"};
    //
    //Assetsbox variables END
    //

    //
    // Model variables START
    //

    

    // Use this for initialization
    private void Start()
    {
        //Set up values for the colorbox and generates it
        ChangeBrightness();

        //Set up values for the assetsbox and generates it
        HidePanels(_panelnames[0]);

       // GenerateModel("male");
    }

    public void GenerateModel(string type)
    {

        GameObject.Find("UI").GetComponent<AudioSource>().Play();

        var folderpath = "Assets/Mannequins/Prefabs/";
        GameObject model = new GameObject();
        model.transform.name = "Model";
        model.transform.SetParent(GameObject.Find("Main Camera").transform);
        model.transform.localPosition= new Vector3(0,0,0);

        var modelPos = new Vector3(-1f,0.5f,2.8f);

        var bodyTemplate = type == "male" ? AssetDatabase.LoadAssetAtPath<GameObject>(folderpath + "mannequin_test.prefab") : AssetDatabase.LoadAssetAtPath<GameObject>(folderpath + "mannequin_02.prefab");

        var newModel = CanvasOrganisation.Instantiate(bodyTemplate);

        newModel.transform.SetParent(model.transform);
        newModel.transform.localPosition = modelPos;
        newModel.transform.name = "BODY";
        newModel.transform.localScale = new Vector3(1.5f,1.5f,1.5f); // Temp

        var headFolder = new GameObject();
        headFolder.transform.name = "HEAD";
        headFolder.transform.SetParent(newModel.transform);
        headFolder.transform.localPosition = new Vector3(0,0,0);

        var faceShape = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Character/SHAPE/JuliusCaesarBust.fbx");

        var newFace = CanvasOrganisation.Instantiate(faceShape);
        newFace.transform.name = "SHAPE";
        newFace.transform.SetParent(headFolder.transform);
        newFace.transform.localPosition = new Vector3(0, 0, 0);
        newFace.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

    }

    public void RotateModel()
    {
        var rotateSliderGet = GameObject.Find("RotateSlider").GetComponent<Slider>();
        GameObject.Find("BODY").transform.eulerAngles = new Vector3(transform.eulerAngles.x, rotateSliderGet.value, transform.eulerAngles.z);
    }

    public void ChangeSizeSlide()
    {
        ChangeSize(GameObject.Find(_itemSelected), GameObject.Find("SizeSlider").GetComponent<Slider>().value);
    }

    public void MoveAsset(string direction)
    {
        var asset = GameObject.Find(_itemSelected);

        var posy = asset.transform.localPosition.y;
        var posx = asset.transform.localPosition.x;

        const float distance = 0.02f;

        switch (direction)
        {
            case "TOP":
                posy += distance;
                break;

            case "DOWN":
                posy -= distance;
                break;

            case "LEFT":
                posx -= distance;
                break;

            case "RIGHT":
                posx += distance;
                break;
            default:
                Debug.Log("Nothing happens.");
                break;
        }
        asset.transform.localPosition = new Vector3(posx, posy, asset.transform.localPosition.z);
    }

    private float _zoomSpeed;
    private Camera _camera;
    private Vector3 _targetPosition;

    void Update()
    {
        if (_camera != null)
        {
            _camera.transform.position = Vector3.Slerp(_camera.transform.position, _targetPosition, Time.deltaTime * _zoomSpeed);
            if (_camera.transform.position == GameObject.Find("Main Camera").transform.position)
                Destroy(_camera.gameObject);
        }
    }

    private void Zoom(Vector3 startPosition, Vector3 endPosition, float speed = 5f)
    {
        _zoomSpeed = speed;
        var cameraObject = _camera == null ? new GameObject("CameraZoom").AddComponent<Camera>().gameObject : _camera.gameObject;
        _camera = cameraObject.GetComponent<Camera>();
        cameraObject.transform.position = startPosition; ;
        _targetPosition = endPosition;
    }

    public void OnClickCategoryButton(string panelname)
    {
        if (panelname == "FacePanel")
        {
            var shapePosition = GameObject.Find("SHAPE").transform.position;
            Zoom(GameObject.Find("Main Camera").transform.position, new Vector3(shapePosition.x + .3f, shapePosition.y, shapePosition.z - 1f));
        }
        else if (_camera != null)
            Zoom(_camera.transform.position, GameObject.Find("Main Camera").transform.position);

        DeletePanelContent("AssetsPanel");
        HidePanels(panelname);
    }

    private void HidePanels(string exception)
    {
        foreach (var name in _panelnames)
        {
            var temp = GameObject.Find(name);
            temp.transform.localScale = name != exception ? new Vector2(0, 0) : new Vector2(1, 1);
        }
    }

    public void OnClickSubCategoryButton(string type)//rename type
    {
        _itemSelected = type;
        DeletePanelContent("AssetsPanel");
       // if (GameObject.Find("AssetsPanel").transform.childCount == 0)
        GenerateAssetBox(type);
        ChangeBrightness();
    }

    private void DeletePanelContent(string panelname)
    {
        var childs = GameObject.Find(panelname).transform.childCount;

        for (var i = childs - 1; i >= 0; i--)
        {
            GameObject.Destroy(GameObject.Find(panelname).transform.GetChild(i).gameObject);
        }

    }

    private void GenerateColorbox(float brightness = 50f)
    {
        var x = -296;
        var y = 0;

       float colorboxButtonScaleX = 0.060f;
       float colorboxButtonScaleY = 0.3f;

        DeletePanelContent("ColorPanel");
        List<string> colors = null;
        GameObject.Find("ColorPanel").transform.localScale = new Vector3(1,1,1);
        //GameObject.Find("BrightnessSlider").transform.localScale = new Vector3(1, 1, 1);
        //Math.Round(Math.Sqrt(nbcolors)
        if (_itemSelected == "BODY")
        {
            colors = _skincolors.ToList();
            colorboxButtonScaleX = 0.2f;
            x = -282;
            GameObject.Find("BrightnessSlider").transform.localScale = new Vector3(0,0,0);
        }
        else
        {
            GameObject.Find("BrightnessSlider").transform.localScale = new Vector3(0, 0, 0);
            foreach (var child in ChildrenException)
            {
                if (_itemSelected == child)
                {
                    colors = GetColorList(brightness).ToList();
                    GameObject.Find("BrightnessSlider").transform.localScale = new Vector3(1, 1, 1);
                    break;
                }
            }
        }

        if (colors == null)
        {
            GameObject.Find("ColorPanel").transform.localScale = new Vector3(0, 0, 0);
            return;
        }
    
        foreach (var color in colors)//_skincolors
        {
            var button= CreateButton(x, y,"ColorPanel","", "ColorButton_" + color, color,colorboxButtonScaleX,colorboxButtonScaleY);
            int[] sizes = { Convert.ToInt32(((RectTransform)button.transform).rect.width * colorboxButtonScaleX), Convert.ToInt32(((RectTransform)button.transform).rect.width * colorboxButtonScaleY) }; ;

            var color1 = color; // necessary
            button.onClick.AddListener(() => { Utility.ChangeColor(GameObject.Find(_itemSelected),color1, ChildrenException); });


            _colorboxButtonWidth = sizes[0];
            //_colorboxButtonHeight = sizes[1]; //useless actually. Would be used if the colorbox has more than 1 row

            x += _colorboxButtonWidth;
        }
    }



    private void GenerateAssetBox(string type)
    {
        //reset x and y values
        var x = AssetsboxbaseX;
        var y = AssetsboxbaseY;

        var assets = new List<GameObject>(GetListObject(type));

        for (var i = 1; i <= assets.Count; i++)
        {        
            var asset = assets[i-1];
            var assetPath= asset.GetComponent<AssetIdentity>().SourceAssetPath;
            var button = CreateButton(x, y, "AssetsPanel", "", asset.name.Replace("(Clone)", ""), "#FFFFFF",
                AssetsboxButtonScaleX, AssetsboxButtonScaleY,
                false, Utility.MakeSprite(asset, 300, 300));
            
            button.onClick.AddListener(() =>
            {
                PermuteCharacterParts(type, AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)); });

            var sizes = new [] { Convert.ToInt32(((RectTransform)button.transform).rect.width * AssetsboxButtonScaleX), Convert.ToInt32(((RectTransform)button.transform).rect.width * AssetsboxButtonScaleY) };

            if (i % 5 == 0)
            {
                x = AssetsboxbaseX;
                y -= sizes[1];
            }
            else
                x += sizes[0];
            

        }
    }

    private void PermuteCharacterParts(string bodyPart, GameObject newObject)
    {
        var getObject = GameObject.Find(bodyPart);
        Utility.PermuteGameObject(ref getObject, newObject,true);
    }

    private Button CreateButton(int posx, int posy, string parent = "", string text = "", string name = "", string color = "#FFFFFF", float scaleX = 1, float scaleY = 1, bool isflat = true, Sprite sprite = null)
    {
        //get button from asset
        var assetTemplate = AssetDatabase.LoadAssetAtPath<Button>("Assets/UI/TemplateButton.prefab");

        //used to transform color into hexadecimal color
        var hexacolor = new Color();
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
        return newButton;
        
    }

   

    /// <summary>
    /// Return a list of items corresponding at a body part
    /// </summary>
    /// <param name="bodyPart">The object type</param>
    /// <returns></returns>
    public List<GameObject> GetListObject(string bodyPart)
    {
        var list = new List<GameObject>();
        foreach (var file in Directory.GetFiles(Config.CharacterPath + bodyPart))
        {
            var asset = Utility.LoadGameObject(file);
            if (asset == null) continue;
            var identity = asset.AddComponent<AssetIdentity>().GetComponent<AssetIdentity>();
            identity.Identity = bodyPart;
            identity.SourceAssetPath = file;
            list.Add(asset);
        }
        return list;
    }

    /// <summary>
    /// Add a scale to an element
    /// </summary>
    /// <param name="addScale">The scale (height and width) added to the object</param>
    public void ChangeSize(GameObject item,float addScale)
    {
        try
        {
            item.transform.localScale = new Vector3(addScale, item.transform.localScale.y, addScale);

        }
        catch (Exception)
        {
            throw new ArgumentNullException("The SelectedGameObject value is null");
        }
    }

    public void ChangeBrightness()
    {
        GameObject lel = GameObject.Find("BrightnessSlider");

       GenerateColorbox(lel.GetComponent<Slider>().value);

    }
    /// <summary>
    /// Get a lit of colors 
    /// </summary>
    /// <param name="numberOfColors">The number of colors</param>
    /// <param name="start">The start color</param>
    /// <param name="end">The end color</param>
    /// <param name="saturation"></param>
    /// <param name="brightness"></param>
    /// <returns></returns>
    public List<string> GetColorList(float brightness, float saturation = 80, int numberOfColors = 100)
    {
        var colorList = new List<string>();
        for (float i = 0; i < numberOfColors; i++)
        {
            var color = Color.HSVToRGB(i / 100, saturation / 100, brightness / 100);
            colorList.Add(ColorToHex(color));
        }
        return colorList;
    }

    string ColorToHex(Color32 color)
    {
        return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
    }

}
