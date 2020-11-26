using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class ZoomCollection : NetworkBehaviour
{

    // Use this for initialization
    public float scale = 1.0f;
    float valZoom = 1f;
    public GameObject zoomable;
    public GameObject slider;
    public GameObject panPanel;
    [SerializeField]
    public bool scaleD;
    [Range(0.8f, 1.4f)]
    public float valueInputScale;
    public float maxZoomValue;
    private bool isBeingPlaced;
    private string contDirection = "None";
    private float panFactor;
    private Vector3 defaultPosition;
    private Vector3 defaultScale;
    float defaultMaxMarkerValue = 20.0f;

    public GameObject markerTextMax;
    public GameObject markerTextMid;
    Vector3 startLocation;
    GameObject metaMouse;
    bool track = false;
    bool isMeta;
    void Start()
    {
        defaultPosition = zoomable.transform.localPosition;
        defaultScale = zoomable.transform.localScale;
        isBeingPlaced = false;
        slider.gameObject.SetActive(false);
        panFactor = 0.02f;
        maxZoomValue = 3.0f;
        defaultMaxMarkerValue = 20;

    }

    public void SetMeta(bool set)
    {
        isMeta = set;
        metaMouse = GameObject.Find("MetaMouse(Clone)");
    }

    public void ResetZoom()
    {
        zoomable.transform.localPosition = defaultPosition;
        zoomable.transform.localScale = defaultScale;
    }
    public void IsBeingPlaced(bool isInMovement)
    {
        isBeingPlaced = isInMovement;
    }

    public void ScalePrefab(int i)
    {
        if (i == 0)
        {


            scale += 0.2f;
            valZoom += 0.5f;
            if (scale > 1.8)
            {
                scale = 1.8f;
                valZoom = 3;
            }
            zoomable.transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            scale -= 0.2f;
            valZoom -= 0.5f;
            if (scale < 0.8f)
            {
                scale = 0.8f;
                valZoom = 0.5f;
            }
            zoomable.transform.localScale = new Vector3(scale, scale, scale);
        }
        if (scaleD)
        {
            scale = valueInputScale;
        }

        slider.gameObject.GetComponent<Slider>().value = valZoom;
        slider.gameObject.SetActive(true);
        Invoke("HideGameObject", 5);
    }

    [ClientRpc]
    public void RpcLog()
    {
        Debug.Log("Called on client");

    }
    [ClientRpc]
    public void RpcZoomIn()
    {

       
        scale += 0.2f;
        //valZoom += 0.5f;
        //if (scale > 2.6)
        //{
        //    scale = 2.6f;
        //    valZoom = 6;
        //}
        slider.gameObject.GetComponent<Slider>().maxValue = maxZoomValue;
        scale = Mathf.Clamp(scale, 1, maxZoomValue);
        valZoom = scale;

        zoomable.transform.localScale = new Vector3(scale, scale, scale);

        slider.gameObject.GetComponent<Slider>().value = valZoom;
        slider.gameObject.SetActive(true);
        slider.gameObject.GetComponent<Slider>().interactable = false;
        Invoke("HideGameObject", 5);
        ChangeMarker();
    }

    [ClientRpc]
    public void RpcZoomOut()
    {

        slider.gameObject.GetComponent<Slider>().maxValue = maxZoomValue;

        scale -= 0.2f;
        if (scale < 1)
        {
            scale = 1;
            zoomable.transform.localPosition = defaultPosition;
            panPanel.SetActive(false);
        }

        valZoom = scale;

        zoomable.transform.localScale = new Vector3(scale, scale, scale);
        Pan("Left");
        slider.gameObject.GetComponent<Slider>().value = valZoom;
        slider.gameObject.SetActive(true);
        slider.gameObject.GetComponent<Slider>().interactable = false;
        Invoke("HideGameObject", 5);
        ChangeMarker();
    }

    public void ChangeMarker()
    {
        float currentVal = defaultMaxMarkerValue / scale;
        markerTextMax.GetComponent<TextMesh>().text = currentVal.ToString("F2") + "NM";
        markerTextMid.GetComponent<TextMesh>().text = (currentVal / 2.0f).ToString("F2") + "NM";
    }
    public void Pan(string direction)
    {
        float x = zoomable.transform.localPosition.x;
        float z = zoomable.transform.localPosition.z;
        if (direction == "Left")
        {
            x += Vector3.left.x * panFactor;
        }
        if (direction == "Right")
        {
            x += Vector3.right.x * panFactor;
        }
        if (direction == "Top")
        {
            z += Vector3.forward.z * panFactor;
        }
        if (direction == "Bottom")
        {
            z += Vector3.back.z * panFactor;
        }

        x = Mathf.Clamp(x, -(zoomable.transform.localScale.x - 1), (zoomable.transform.localScale.x - 1));
        z = Mathf.Clamp(z, defaultPosition.z - ((zoomable.transform.localScale.z - 1) * 2), defaultPosition.z);

        zoomable.transform.localPosition = new Vector3(x, defaultPosition.y, z);

    }

    public void PanContinuous(string direction)
    {

        contDirection = direction;
        TrailControl.crTrail = true;

    }

    public void StopPanContinuous()
    {

        contDirection = "None";
        TrailControl.crTrail = false;

    }
    void HideGameObject()
    {

        slider.SetActive(false);

        scaleD = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (scaleD)
        {
            ScalePrefab(0);
        }

        if (contDirection != "None")
        {
            Pan(contDirection);
        }

        if (panPanel.active)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                RpcZoomIn();
                TrailControl.crTrail = true;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                RpcZoomOut();
                TrailControl.crTrail = true;
            }


            if (Input.GetMouseButtonDown(2))
            {
                if (!isMeta)
                {
                    startLocation = Input.mousePosition;
                }
                else
                {
                    if (!metaMouse)
                        metaMouse = GameObject.Find("MetaMouse(Clone)");
                    startLocation = metaMouse.transform.localPosition;
                }

                track = true;
            }

            if (track && Input.GetMouseButton(2))
            {
                Vector3 currentLocation;
                if (!isMeta)
                {
                    currentLocation = Input.mousePosition;
                }
                else
                {
                    if (!metaMouse)
                        metaMouse = GameObject.Find("MetaMouse(Clone)");
                    currentLocation = metaMouse.transform.localPosition;
                }
                float horizontalDiff = currentLocation.x - startLocation.x;
                float verticalDiff = currentLocation.y - startLocation.y;
                TrailControl.crTrail = true;
                //If horizontal is positive, mouse going towards right
                if (horizontalDiff > 0)
                    Pan("Left");
                else if (horizontalDiff < 0)
                    Pan("Right");

                if (verticalDiff > 0)
                    Pan("Bottom");
                else if (verticalDiff < 0)
                    Pan("Top");
            }

            if (Input.GetMouseButtonUp(2))
            {
                track = false;
            }
        }


    }
}
