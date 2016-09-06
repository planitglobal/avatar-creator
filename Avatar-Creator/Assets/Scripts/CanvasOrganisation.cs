using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.UI;

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
        if (GUILayout.Button("Press Me"))
            Debug.Log("Hello!");
    }

    public void CreateButton(Transform panel, Vector3 position, Vector2 size, UnityEngine.Events.UnityAction method)
    {

    }

    public void onClickFaceButton()
    {
        Button test = AssetDatabase.LoadAssetAtPath<Button>("Assets/UI/TemplateButton.prefab");
        test.transform.position = new Vector2(400, 200);
    

        CanvasOrganisation.Instantiate(test);
        Debug.Log("CA A MARCHE");

    }

    private void CreateButton(int posx, int posy, string text)
    {

        
    }

}
