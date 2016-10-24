using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine.UI;

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
    private const int AssetsboxbaseY = 50;

    private readonly string[] _panelnames = {"FacePanel","ClothesPanel", "AccessoriesPanel"};

    private string _itemSelected = "BODY";

    

    public readonly string[] ChildrenException = {"EYEBROWS", "SHOES", "eye_right", "eye_left", "pupil_left", "pupil_right", "EYES","MOUTH","SHIRT","PANTS","GLASSES","HAIR","BEARD","CAPS","JEWELRY"};
    public readonly string[] ScalableObject = {"BODY","EYEBROWS","MOUTH","NOSE"};
    public readonly string[] MovableObject = {"EYEBROWS", "EYES","MOUTH","NOSE"};
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
        HidePanels();
        DisplayButton(true);
    }

    public void GenerateModel(string type)
    {
        DisplayButton(false);

        if (GameObject.Find(Config.ModelName))
            Destroy(GameObject.Find(Config.ModelName));

        GameObject.Find("UI").GetComponent<AudioSource>().Play();

        var folderpath = "Mannequins/";
        var newModel = type == "male" ? Utility.LoadGameObject(folderpath + "mannequin_test") : Utility.LoadGameObject(folderpath + "slim_female");
        newModel.transform.name = "Model";
        newModel.transform.SetParent(GameObject.Find("Main Camera").transform);
        newModel.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // Temp
        newModel.AddComponent<Rigidbody>();
        newModel.transform.localPosition = new Vector3(-1.1f, 0.1f, 2.8f);
        StockDefault();


        //Set the default panel
        OnClickSubCategoryButton("BODY");
    }

    public void StockDefault()
    {
        foreach (var element in GameObject.Find(Config.ModelName).GetComponentsInChildren<Transform>(true))
        {
            var AssetIdentity = element.gameObject.AddComponent<AssetIdentity>().GetComponent<AssetIdentity>();
            AssetIdentity.BasePosition = element.localPosition;
            AssetIdentity.BaseScale = element.localScale;

            if (element.transform.GetComponent<Renderer>() != null)
                AssetIdentity.BaseColor = Utility.ColorToHex(element.transform.GetComponent<Renderer>().material.color);
        }
    }

    private float _oldRotation = 0;
    public void RotateModel()
    {
        var rotateSpeed = 10;
        var sensibility = 3;
        var rotateSliderGet = GameObject.Find("RotateSlider").GetComponent<Slider>();
        var model = GameObject.Find(Config.ModelName).transform;
                var rot = rotateSliderGet.value > _oldRotation ? rotateSpeed * -1 : rotateSpeed;

        _oldRotation = rotateSliderGet.value;
         model.eulerAngles = new Vector3(0, model.eulerAngles.y + rot, 0);

    }

    private void DisplayButton(bool display)
    {
        foreach (var b in GameObject.Find("MaleButton").GetComponentsInChildren<Image>()) b.enabled = display;
        foreach (var b in GameObject.Find("MaleButton").GetComponentsInChildren<Button>()) b.enabled = display;
        foreach (var b in GameObject.Find("MaleButton").GetComponentsInChildren<Text>()) b.enabled = display;

        foreach (var b in GameObject.Find("FemaleButton").GetComponentsInChildren<Image>()) b.enabled = display;
        foreach (var b in GameObject.Find("FemaleButton").GetComponentsInChildren<Button>()) b.enabled = display;
        foreach (var b in GameObject.Find("FemaleButton").GetComponentsInChildren<Text>()) b.enabled = display;

        foreach (var b in GameObject.Find("RotateSlider").GetComponentsInChildren<Slider>()) b.enabled = !display;
        foreach (var b in GameObject.Find("RotateSlider").GetComponentsInChildren<Image>()) b.enabled = !display;
    }

    public void ChangeSizeSlide()
    {
        ChangeSize(Utility.GetActiveElement(_itemSelected), GameObject.Find("SizeSlider").GetComponent<Slider>().value);
    }

    public void MoveAsset(string direction)
    {
        var asset = Utility.GetActiveElement(_itemSelected);

        var posy = asset.transform.localPosition.y;
        var posx = asset.transform.localPosition.x;

        const float distance = 0.005f;

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
        if (_targetPosition == endPosition) return;
        _camera = cameraObject.GetComponent<Camera>();
        _camera.fieldOfView = GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView;
        cameraObject.transform.position = startPosition; ;
        _targetPosition = endPosition;
    }

    
    public void OnClickCategoryButton(string panelname)
    {
        if (panelname == "FacePanel")
        {
            Zoom(GameObject.Find("Main Camera").transform.position, new Vector3(839.1f, 314.8f, -824.8f));
        }
        else if (_camera != null)
            Zoom(_camera.transform.position, GameObject.Find("Main Camera").transform.position);

        DeletePanelContent("AssetsPanel");
        HidePanels(panelname);
        ShowMoveElement();
    }

    private void HidePanels(string exception="")
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
        if(Utility.GetActiveElement(_itemSelected)!=null && ScalableObject.Any(a=>a== Utility.GetNameWithoutNumber(_itemSelected)))
            GameObject.Find("SizeSlider").GetComponent<Slider>().value = _itemSelected!="BODY" ? Utility.GetActiveElement(_itemSelected).transform.localScale.x : GameObject.Find(Config.ModelName).transform.localScale.x;
        DeletePanelContent("AssetsPanel");
        GenerateAssetBox(type);
        ChangeBrightness();

    }

    private void DeletePanelContent(string panelname)
    {
        var childs = GameObject.Find(panelname).transform.childCount;

        for (var i = childs - 1; i >= 0; i--)
        {
            Destroy(GameObject.Find(panelname).transform.GetChild(i).gameObject);
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
            button.onClick.AddListener(() =>
            {
                Utility.ChangeColor(Utility.GetActiveElement(_itemSelected), color1, ChildrenException);
            });


            _colorboxButtonWidth = sizes[0];
            //_colorboxButtonHeight = sizes[1]; //useless actually. Would be used if the colorbox has more than 1 row

            x += _colorboxButtonWidth;
        }
    }

    private void ShowMoveElement(string type="")
    {
        var movable = MovableObject.Any(a => a == type);
        foreach (var b in GameObject.Find("MoveArrows").GetComponentsInChildren<Image>()) b.enabled = movable;
        foreach (var b in GameObject.Find("MoveArrows").GetComponentsInChildren<Button>()) b.enabled = movable;

        var sizeable = ScalableObject.Any(a => a == type);
        foreach (var b in GameObject.Find("SizeSlider").GetComponentsInChildren<Image>())b.enabled= sizeable;
        foreach (var b in GameObject.Find("SizeSlider").GetComponentsInChildren<Slider>())b.enabled= sizeable;
    }

    private void GenerateAssetBox(string type)
    {
        ShowMoveElement(type);
        var assets = new List<GameObject>(GetListObject(type).OrderBy(a=>a.name));


            var x = AssetsboxbaseX;
            var y = AssetsboxbaseY;


            for (var i = 1; i <= assets.Count; i++)
            {
                var asset = assets[i - 1];
                var button = CreateButton(x, y, "AssetsPanel", "", "icon_"+asset.name, "#FFFFFF",
                    AssetsboxButtonScaleX, AssetsboxButtonScaleY,
                    false, Utility.MakeSprite(asset.name));
                button.onClick.AddListener(() =>
                {
                     PermuteCharacterParts(asset.name);
                });

                var sizes = new[]
                {
                    Convert.ToInt32(((RectTransform) button.transform).rect.width*AssetsboxButtonScaleX),
                    Convert.ToInt32(((RectTransform) button.transform).rect.width*AssetsboxButtonScaleY)
                };

                if (i%5 == 0)
                {
                    x = AssetsboxbaseX;
                    y -= sizes[1];
                }
                else
                    x += sizes[0];
            }
   
    }

    public void PermuteCharacterParts(string newItemName)
    {
        if (Utility.GetActiveElement(Utility.GetNameWithoutNumber(newItemName)) != null)
        {
            var a = Utility.GetActiveElement(Utility.GetNameWithoutNumber(newItemName));

            Utility.GetActiveElement(Utility.GetNameWithoutNumber(newItemName)).SetActive(false);
        }
        var newItem= Utility.FindAll(newItemName);
        newItem.transform.localPosition = newItem.GetComponent<AssetIdentity>().BasePosition;
        newItem.transform.localScale = newItem.GetComponent<AssetIdentity>().BaseScale;
        if (newItem.GetComponent<AssetIdentity>().BaseColor != "")
            Utility.ChangeColor(newItem,newItem.GetComponent<AssetIdentity>().BaseColor,ChildrenException);
        newItem.SetActive(true);
    }

    public void Load(int userId = 1)
    {
        Reset();
        GenerateModel("slime_female");
        Database.GetDatabase().SelectById("t_character", "cha_data", "fk_userId", userId.ToString(), PlaceModelIntoCamera);
    }
    private void PlaceModelIntoCamera()
    {
        SerializeGameObject.Deserialize(Convert.FromBase64String(Database.GetDatabase().SelectResult.FirstOrDefault(a => a.Key == "cha_data").Value),GameObject.Find(Config.ModelName));
        GameObject.Find(Config.ModelName).transform.parent = GameObject.Find("Main Camera").transform;
        DisplayButton(false);
    }
    public void Save()
    {
        //var fileName = DateTime.Now.ToString().Replace("/", "").Replace(".", "").Replace(":", "").Replace(" ", "");
        //if (!Directory.Exists("Assets/TmpSave"))
        //    Directory.CreateDirectory("Assets/TmpSave");
        //// The paths to the mesh/prefab assets.
        //string prefabPath = "Assets/TmpSave/" + fileName;


        // Create a transform somehow, using the mesh that was previously saved.
        var gameObjectToSave = GameObject.Find("Model");
        var element = Convert.ToBase64String(SerializeGameObject.Serialize(gameObjectToSave));
        var userName = 1;
        Database.GetDatabase().SaveStrings(new Dictionary<string, string> { { "cha_data", element }, { "fk_userId", userName.ToString() } }, "t_character");
    }

    public void Reset()
    {
        DisplayButton(true);
        var model = GameObject.Find(Config.ModelName);
        if (model != null)
            Destroy(model);
    }

    private Button CreateButton(int posx, int posy, string parent = "", string text = "", string name = "", string color = "#FFFFFF", float scaleX = 1, float scaleY = 1, bool isflat = true, Sprite sprite = null)
    {
        //get button from asset
        var assetTemplate = Resources.Load<Button>("UI/TemplateButton");

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
        foreach (var element in GameObject.Find(Config.ModelName).GetComponentsInChildren<Transform>(true))
        {
            if(element.name== Config.ModelName || Utility.GetNameWithoutNumber(element.name)!=bodyPart)
                continue;
            list.Add(element.gameObject);

        }
        return list;
    }

    private Vector3 SizeTable(GameObject item, float addSize)
    {
        Vector3 vector;
        switch (Utility.GetNameWithoutNumber(item.name))
        {
            case Config.ModelName:
                vector= new Vector3(addSize, item.transform.localScale.y, addSize);
                break;
            case "NOSE":
            case "EYEBROWS":
            case "MOUTH":
                vector = new Vector3(addSize, item.transform.localScale.y, item.transform.localScale.z);
                break;
            default:
                vector = new Vector3(addSize, addSize, addSize);
                break;
        }

        return vector;
    }






    /// <summary>
    /// Add a scale to an element
    /// </summary>
    /// <param name="item"></param>
    /// <param name="addScale">The scale (height and width) added to the object</param>
    public void ChangeSize(GameObject item,float addScale)
    {
        try
        {
            if(Utility.GetNameWithoutNumber(item.name)=="BODY")
                item=GameObject.Find(Config.ModelName);
            item.transform.localScale = SizeTable(item, addScale);
        }
        catch (Exception)
        {
            throw new ArgumentNullException("The SelectedGameObject value is null");
        }
    }

    public void ChangeBrightness()
    {
        GameObject slider = GameObject.Find("BrightnessSlider");
       GenerateColorbox(slider.GetComponent<Slider>().value);
    }
    /// <summary>
    /// Get a lit of colors 
    /// </summary>
    /// <param name="numberOfColors">The number of colors</param>
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
