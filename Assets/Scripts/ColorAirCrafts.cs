using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ColorAirCrafts : MonoBehaviour {

    Scene activeScene;
    public GameObject aircraftParent;
    public GameObject terrain3D;
	public GameObject terrain2D;
    public Slider brightnessSlider;
    //private Material terrainMat;
    [Range(0.001f, 3f)]
    public float brightness;
    private float oldBrightness;
    int lastActiveCount;
    int currentActiveCount;
	// Use this for initialization
	void Start () {

        activeScene = SceneManager.GetActiveScene();

        int temp = 0;
        foreach (Transform child in aircraftParent.transform)
        {
            if(child.gameObject.activeInHierarchy)
            {
                temp++;
            }
        }

        lastActiveCount = currentActiveCount = temp;

        
        brightness = 0.8f;
        oldBrightness = brightness;
    }
	
	// Update is called once per frame
	void Update () {

        int temp = 0;
        foreach (Transform child in aircraftParent.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                temp++;
            }
        }

        currentActiveCount = temp;

        if(lastActiveCount !=currentActiveCount)
        {
            UpdateColor();
            lastActiveCount = currentActiveCount;
        }
        brightness = brightnessSlider.value;

        if(brightness!=oldBrightness)
        {
            oldBrightness = brightness;
            
			terrain3D.GetComponent<Renderer>().material.SetFloat("_myRange", brightness);
			terrain2D.GetComponent<Renderer>().material.SetFloat("_myRange", brightness);

        }
    }

    void UpdateColor()
    {
        if (activeScene.name == "ARScene")
        { 
            foreach (Transform child in aircraftParent.transform)
            {
                if (!child.name.Contains("PlaneInfo"))
                {
                    Color col = Color.gray;
                    string code = child.GetComponent<movement>().getColor();
                    if (code == "Blue")
                    {
                        col = Color.blue;
                    }
                    else if (code == "Red")
                    {
                        col = Color.red;
                    }
                    else if (code == "Green")
                    {
                        col = Color.green;
                    }
                    else if (code == "Yellow")
                    {
                        col = Color.yellow;
                    }
                    else if (code == "Pink")
                    {
                        col = Color.magenta;
                    }

                    Transform body = child.GetChild(0);
                    if (body)
                    {

                        Renderer rend = body.gameObject.GetComponent<Renderer>();
                        Material mat = rend.materials[0];
                        col.a = 0.5f;
                        mat.color = col;

                    }

                }



            }

        }
    }
}
