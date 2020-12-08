
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
public class TrailControl : NetworkBehaviour
{

    public GameObject Aircraft_parent;
    public GameObject sceneContent;
    float minSampleRate = 0.2f;
    float scale = 0.0025f;
    float timeRatio = 0;
    int val;
    float valSpeed = 0f;
    float valScale = 0;
    bool changed = false;
    bool timeup = false;
    public GameObject slider;
    public GameObject sliderScale;
    Scene activeScene;

    [SerializeField]
    public static bool crTrail = false;
    public static bool close = false;
    public bool debug = false;
    public bool aircraftinfo = false;
    public bool airtrail = false;
    public bool hideall = false;
    public bool scaleD = false;

    [Range(1.0f, 30f)]
    public float valueInputFromEditor;

    [Range(0.6f, 1.4f)]
    public float valueInputScale;

    public void AircraftStart()
    {
        foreach (Transform child in Aircraft_parent.transform)
        {
            if (child.gameObject.activeInHierarchy)
                child.gameObject.GetComponent<movement>().FlagOn();
        }
    }

    public void AircraftPause()
    {
        foreach (Transform child in Aircraft_parent.transform)
        {
            if (child.gameObject.activeInHierarchy)
                child.gameObject.GetComponent<movement>().FlagOff();
        }
    }
    public void AircraftReset()
    {

        foreach (Transform child in Aircraft_parent.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                child.gameObject.SetActive(false);
                child.gameObject.GetComponent<movement>().ResetObject();
            }
        }
    }

    public void HideAll()
    {

        if (hideall || sceneContent.gameObject.activeInHierarchy)
            if (sceneContent.gameObject.name != "Data Dashboard")
                sceneContent.SetActive(false);
            else
                sceneContent.SetActive(true);

    }

    [ClientRpc]
    public void RpcClearTrail()
    {
        foreach (Transform child in Aircraft_parent.transform)
        {

            TrailRenderer temp = child.gameObject.GetComponent<TrailRenderer>();

            temp.Clear();

        }
    }

    public void ScalePrefab0(int i)
    {

        foreach (Transform child in Aircraft_parent.transform)
        {
            child.gameObject.GetComponent<movement>().ScalePrefab(i);
        }

    }

    public void ScalePrefab(int i)
    {
        val = 1;
        if (scaleD)
        {
            scale = valueInputScale;
            foreach (Transform child in Aircraft_parent.transform)
            {
                if (activeScene.name == "ARScene")
                    child.GetChild(0).localScale = new Vector3(scale, scale, scale);
                else if (activeScene.name == "LiveData")
                {
                    child.localScale = new Vector3(scale, scale, scale);

                }              

            }
            //SABBAR Line Comment
            //sliderScale.gameObject.GetComponent<HoloToolkit.Examples.InteractiveElements.SliderGestureControl>().SetSliderValue(scale);
            sliderScale.gameObject.GetComponent<Slider>().value = valScale;
            sliderScale.gameObject.SetActive(true);
            Invoke("HideGameObject", 5);
        }
        else
        {
            if (i == 0)
            {
                scale += 0.0005f;
                valScale += 1f;
                if (scale > 0.006f)
                {
                    scale = 0.006f;
                    valScale = 10f;
                }
            }
            else
            {
                scale -= 0.0005f;
                valScale -= 1f;
                if (scale < 0.0005f)
                {
                    valScale = 0f;
                    scale = 0.0005f;
                }
            }
            foreach (Transform child in Aircraft_parent.transform)
            {
                if (activeScene.name == "ARScene")
                    child.GetChild(0).localScale = new Vector3(scale, scale, scale);
                else if (activeScene.name == "LiveData")
                {
                    child.localScale = new Vector3(scale, scale, scale);
                }

            }
            //SABBAR Line Comment
            // sliderScale.gameObject.GetComponent<HoloToolkit.Examples.InteractiveElements.SliderGestureControl>().SetSliderValue(valScale);
            sliderScale.gameObject.GetComponent<Slider>().value = valScale;
            sliderScale.gameObject.SetActive(true);
            Invoke("HideGameObject", 5);
        }
    }

    public void ResetSpeed()
    {
        while (valSpeed != 0)
        {
            SpeedPrefab(1);
            Debug.Log(valSpeed);
        }
    }

    public void ResetScale()
    {
        while (valScale != 0)
        {
            ScalePrefab(1);
        }
    }

    public void SpeedPrefab(int i)
    {
        val = 2;
        if (!debug)
        {

            //Time.timeScale = i;
            if (i == 0)
            {
                Time.timeScale += 3;
                valSpeed += 5f;
                changed = true;

            }
            else
            {
                Time.timeScale -= 3;
                valSpeed -= 5f;
                changed = true;
            }
            if (changed)
            {
                Time.timeScale = Mathf.Clamp(Time.timeScale, 1, timeRatio * minSampleRate);
                valSpeed = Mathf.Clamp(valSpeed, 0, 15);
                Time.fixedDeltaTime = Time.timeScale / timeRatio;
                changed = false;
            }
            //SABBAR Line Comment
            // slider.gameObject.GetComponent<HoloToolkit.Examples.InteractiveElements.SliderGestureControl>().SetSliderValue(valSpeed);
            slider.gameObject.GetComponent<Slider>().value = valSpeed;
            slider.gameObject.GetComponent<Slider>().interactable = false;
            slider.gameObject.SetActive(true);
            Invoke("HideGameObject", 5);

        }
        else
        {
            Time.timeScale = valueInputFromEditor;

            Time.timeScale = Mathf.Clamp(Time.timeScale, 1, timeRatio * minSampleRate);
            Time.fixedDeltaTime = Time.timeScale / timeRatio;
            changed = false;
            //SABBAR Line Comment
            //  slider.gameObject.GetComponent<HoloToolkit.Examples.InteractiveElements.SliderGestureControl>().SetSliderValue((Time.timeScale - 1) / 10);
            slider.gameObject.GetComponent<Slider>().value = valSpeed;
            slider.gameObject.GetComponent<Slider>().interactable = false;
            slider.gameObject.SetActive(true);
            Invoke("HideGameObject", 5);

        }

    }

    void HideGameObject()
    {
        if (val == 2)
        {
            slider.SetActive(false);
            debug = false;
        }
        else
        {

            sliderScale.SetActive(false);
            scaleD = false;

        }
    }


    //public void SpeedPrefab(int i)
    //{
    //        Time.timeScale = valueInputFromEditor;

    //        Time.timeScale = Mathf.Clamp(Time.timeScale, 1, timeRatio * minSampleRate);
    //        Time.fixedDeltaTime = Time.timeScale / timeRatio;
    //        changed = false;

    //}

    public void AircraftInfo(int i)
    {
        foreach (Transform child in Aircraft_parent.transform)
        {
            GameObject go = GameObject.FindGameObjectsWithTag("GameObjectToHide")[0];

            // if (i == 0)
            // {
            child.Find("PlaneInfo").gameObject.SetActive(go.active);

            // }
            //else
            //{
            //    child.Find("PlaneInfo").gameObject.SetActive(false);
            //}
        }
    }

    public void AircraftTrail(int i)
    {
        //Method 2
        foreach (Transform child in Aircraft_parent.transform)
        {
            if (i == 0)
                child.gameObject.GetComponent<TrailRenderer>().enabled = true;
            else
                child.gameObject.GetComponent<TrailRenderer>().enabled = false;


        }
    }
    public void setTrue()
    {
        crTrail = true;
    }
    public void setFalse()
    {
        crTrail = false;
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    void Start()
    {
        activeScene = SceneManager.GetActiveScene();

        timeRatio = Time.timeScale / Time.fixedDeltaTime;
        slider.gameObject.SetActive(false);
        sliderScale.gameObject.SetActive(false);

    }

    private void OnApplicationFocus(bool focus)
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isServer && crTrail)
        {
           RpcClearTrail();
            TrailControl.crTrail = false;
        }


        if (debug)
        {
            SpeedPrefab(0);
        }

        if (scaleD)
        {
            ScalePrefab(0);
        }
    }


    //if (aircraftinfo)
    //    AircraftInfo(1);
    //else if(!aircraftinfo)
    //    AircraftInfo(0);

    //if (airtrail)
    //    AircraftTrail(1);
    //else if (!airtrail)
    //    AircraftTrail(0);


}


