using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine.UI;
using Application = UnityEngine.Application;

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


    private float _zoomSpeed;
    private Camera _camera;
    private Vector3 _targetPosition;
    private Button _buttonAssetTemplate;
    private bool _launchAnimation;

    public readonly string[] ChildrenException = {"EYEBROWS", "SHOES", "eye_right", "eye_left", "pupil_left", "pupil_right", "EYES","MOUTH","SHIRT","PANTS","GLASSES","HAIR","BEARD","CAPS","JEWELRY"};
    public readonly string[] ScalableObject = {"BODY","EYEBROWS","MOUTH","NOSE"};
    public readonly string[] MovableObject = {"EYEBROWS", "EYES","MOUTH","NOSE"};


    //
    //Assetsbox variables END
    //

    //
    // Model variables START
    //

    private string _gender;
    private GameObject _male;
    private GameObject _female;


    // Use this for initialization
    private void Start()
    {
        var folderpath = "Mannequins/";

        //init animation 
        var platform = GameObject.Find("platform").transform;
        var panel = GameObject.Find("SettingsPanel").transform;
        var genderButton = GameObject.Find("GenderButton").transform;
        platform.localPosition = new Vector3(0, platform.localPosition.y, platform.localPosition.z);
        genderButton.localPosition = new Vector3(270, genderButton.localPosition.y, genderButton.localPosition.z);
        panel.localPosition = new Vector3(1600, panel.localPosition.y, panel.localPosition.z);
        _male = Utility.LoadGameObject(folderpath + "slim_male");
        _female = Utility.LoadGameObject(folderpath + "slim_female");

        //Set up values for the colorbox and generates it
        ChangeBrightness();

        //Set up values for the assetsbox and generates it
        HidePanels();
        DisplayButton(true);
    }

    void Awake()
    {
        _buttonAssetTemplate = Resources.Load<Button>("UI/TemplateButton");
    }

    void Update()
    {
        if (_launchAnimation)
        {
            var speed = 5;
            var platform = GameObject.Find("platform").transform;
            var panel = GameObject.Find("SettingsPanel").transform;

            platform.localPosition = Vector3.Lerp(platform.localPosition, new Vector3(-1.2f, platform.localPosition.y, platform.localPosition.z), Time.deltaTime * speed);
            panel.localPosition = Vector3.Lerp(panel.localPosition, new Vector3(200f, panel.localPosition.y, panel.localPosition.z), Time.deltaTime * speed);

        }

        if (_camera != null)
        {
            _camera.transform.position = Vector3.Slerp(_camera.transform.position, _targetPosition, Time.deltaTime * _zoomSpeed);
            if (_camera.transform.position == GameObject.Find("Main Camera").transform.position)
                Destroy(_camera.gameObject);
        }
    }

    public void GenerateModel(string type)
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
            if(_male!=null)
                _male.SetActive(false);
        }
        DisplayButton(false);
        newModel.transform.name = Config.ModelName;
        GenerateAssetBox();

        newModel.transform.SetParent(GameObject.Find("Main Camera").transform);
        newModel.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f); // Temp
        newModel.transform.localPosition = new Vector3(-1.1f, 6.5f, 2.8f);

        //Set the default panel
        StartCoroutine(LaunchAnimation());

    }

    private IEnumerator LaunchAnimation()
    {
        yield return new WaitForSeconds(.2f);
        _launchAnimation = true;

        yield return new WaitForSeconds(1.5f);
        OnClickSubCategoryButton("BODY");
        StockDefault();

    }
    
    private IEnumerator OnColorChange()
    {
        float value = GameObject.Find("BrightnessSlider").GetComponent<Slider>().value;
        yield return new WaitForSeconds(0.1f);
        if (GameObject.Find("BrightnessSlider").GetComponent<Slider>().value== value)
            GenerateColorbox(value);

    }

    public void StockDefault()
    {
        foreach (var element in GameObject.Find(Config.ModelName).GetComponentsInChildren<Transform>(true))
        {
            var assetIdentity = element.gameObject.AddComponent<AssetIdentity>().GetComponent<AssetIdentity>();
            assetIdentity.BasePosition = element.localPosition;
            assetIdentity.BaseScale = element.localScale;

            if (element.transform.GetComponent<Renderer>() != null)
                assetIdentity.BaseColor = Utility.ColorToHex(element.transform.GetComponent<Renderer>().material.color);
        }
    }

    private float _oldRotation = 0;
    public void RotateModel()
    {
        var rotateSpeed = 10;
        var rotateSliderGet = GameObject.Find("RotateSlider").GetComponent<Slider>();
        var model = GameObject.Find(Config.ModelName).transform;
        var cube = GameObject.Find("cube").transform;
        var rot = rotateSliderGet.value > _oldRotation ? rotateSpeed * -1 : rotateSpeed;
        _oldRotation = rotateSliderGet.value;
        model.eulerAngles = new Vector3(0, model.eulerAngles.y + rot, 0);
        cube.eulerAngles = new Vector3(cube.eulerAngles.x, cube.eulerAngles.y + rot, cube.eulerAngles.z);
    }

    private void DisplayButton(bool display)
    {
        foreach (var b in GameObject.Find("GenderButton").GetComponentsInChildren<Image>()) b.enabled = display;
        foreach (var b in GameObject.Find("GenderButton").GetComponentsInChildren<Button>()) b.enabled = display;
        foreach (var b in GameObject.Find("GenderButton").GetComponentsInChildren<Text>()) b.enabled = display;

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

            case "RIGHT":
                posx -= distance;
                break;

            case "LEFT":
                posx += distance;
                break;
            default:
                Debug.Log("Nothing happens.");
                break;
        }
        asset.transform.localPosition = new Vector3(posx, posy, asset.transform.localPosition.z);
    }

    public void Load()
    {
        var userId = GetUserId();
        Reset();
        GenerateModel("slim_female");
        Database.GetDatabase().LoadAvatar(userId, PlaceModelIntoCamera);
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

    private void ZoomOut()
    {
        if(_camera!=null)
            Zoom(_camera.transform.position, GameObject.Find("Main Camera").transform.position);
    }

    public void OnClickCategoryButton(string panelname)
    {
        if (panelname == "FacePanel")
        {
            Zoom(GameObject.Find("Main Camera").transform.position, new Vector3(839.1f, 314.9f, -824.8f));
        }
        else if (_camera != null)
            ZoomOut();
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
        ShowIcon(type);
        ChangeBrightness();

    }

    private void DeletePanelContent(string panelname)
    {
        var childs = GameObject.Find(panelname).transform.childCount;

        for (var i = childs - 1; i >= 0; i--)
        {
            GameObject.Find(panelname).transform.GetChild(i).gameObject.SetActive(false);
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
                    colors = Utility.GetColorList(brightness).ToList();
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

    private void ShowIcon(string type)
    {
        ShowMoveElement(type);
        foreach (var asset in GetListObject(type))
            Utility.FindAll(Config.IconPrefix + asset.name).SetActive(true);
    }

    private void GenerateAssetBox()
    {

        var assets = new List<GameObject>(GetListObject().OrderBy(a => a.name));
        var types = new List<string>(assets.Select(a=> Utility.GetNameWithoutNumber(a.name)));
        foreach (var type in types)
        {
            var x = AssetsboxbaseX;
            var y = AssetsboxbaseY;
            var typedAssets = assets.Where(a => Utility.GetNameWithoutNumber(a.name) == type).ToList();

            for (var i = 1; i <= typedAssets.Count; i++)
            {
                var asset = typedAssets[i - 1];
                var button = CreateButton(x, y, "AssetsPanel", "", Config.IconPrefix + asset.name, "#FFFFFF",
                    AssetsboxButtonScaleX, AssetsboxButtonScaleY,
                    false, Utility.MakeSprite(asset.name));
                button.onClick.AddListener(() =>
                {
                    PermuteCharacterParts(asset.name);
                });
                button.gameObject.SetActive(false);

                var sizes = new[]
                {
                Convert.ToInt32(((RectTransform) button.transform).rect.width*AssetsboxButtonScaleX),
                Convert.ToInt32(((RectTransform) button.transform).rect.width*AssetsboxButtonScaleY)
            };

                if (i % 5 == 0)
                {
                    x = AssetsboxbaseX;
                    y -= sizes[1];
                }
                else
                    x += sizes[0];
            }

        }
        
   
    }

    public void PermuteCharacterParts(string newItemName)
    {
        if (Utility.GetActiveElement(Utility.GetNameWithoutNumber(newItemName)) != null)
            Utility.GetActiveElement(Utility.GetNameWithoutNumber(newItemName)).SetActive(false);
        var newItem= Utility.FindAll(newItemName);
        newItem.transform.localPosition = newItem.GetComponent<AssetIdentity>().BasePosition;
        newItem.transform.localScale = newItem.GetComponent<AssetIdentity>().BaseScale;
        if (newItem.GetComponent<AssetIdentity>().BaseColor != "")
            Utility.ChangeColor(newItem,newItem.GetComponent<AssetIdentity>().BaseColor,ChildrenException);
        newItem.SetActive(true);
    }

    private void PlaceModelIntoCamera()
    {

        SerializeGameObject.Deserialize(Convert.FromBase64String(Database.GetDatabase().SelectResult), GameObject.Find(Config.ModelName));
        GameObject.Find(Config.ModelName).transform.parent = GameObject.Find("Main Camera").transform;
        GameObject.Find(Config.ModelName).transform.localEulerAngles = new Vector3(0, 180, 0);
        GameObject.Find(Config.ModelName).transform.localPosition = new Vector3(-1.1f, 5f, 2.8f);

        DisplayButton(false);
    }

    private int GetUserId()
    {
        var userId = 1;
        if (Application.isWebPlayer)
        {
            var id = Application.absoluteURL.Substring(Application.absoluteURL.LastIndexOf("userId="));
            id = id.Substring(id.IndexOf('&') - 1);
            id = id.Substring(0, id.IndexOf('&'));
            userId = int.Parse(id);
        }
        return userId;
    }

    public void Save()
    {
        // Create a transform somehow, using the mesh that was previously saved.
        var avatar = Convert.ToBase64String(SerializeGameObject.Serialize(GameObject.Find(Config.ModelName)));             
        Database.GetDatabase().SaveAvatar(GetUserId(), avatar);
    }

    public void Reset()
    {
        ZoomOut();
        _launchAnimation = false;
        DisplayButton(true);
        var model = GameObject.Find(Config.ModelName);
        if (model != null)
            Destroy(model);
        if (_female != null)
            Destroy(_female);
        if (_male != null)
            Destroy(_male);
        Start();


    }

    private Button CreateButton(int posx, int posy, string parent = "", string text = "", string name = "", string color = "#FFFFFF", float scaleX = 1, float scaleY = 1, bool isflat = true, Sprite sprite = null)
    {
        //get button from asset

        //used to transform color into hexadecimal color
        var hexacolor = new Color();
        ColorUtility.TryParseHtmlString(color, out hexacolor);

        //button initialization and settings
        var newButton = Instantiate(_buttonAssetTemplate);

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
    public List<GameObject> GetListObject(string bodyPart="")
    {
        var list = new List<GameObject>();
        foreach (var element in GameObject.Find(Config.ModelName).GetComponentsInChildren<Transform>(true))
        {
            if ((element.parent!=null && element.parent.name != Config.ModelName) || element.name== Config.ModelName || (bodyPart!="" && Utility.GetNameWithoutNumber(element.name)!=bodyPart))
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
        StartCoroutine(OnColorChange());
    }

}
