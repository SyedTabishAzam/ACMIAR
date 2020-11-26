using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DashboardRow : MonoBehaviour
{

    private Transform aircraft;
    public GameObject frontPlate;
    public int NoOfFrontPlates;
    public int NoOfRows; 

    //Dummy Objects for init
    public string identifierName;
    //This is data from plane
    public string[] data;
    string textname;
    //(starting from 1)
    private int ShowIndex;

    [SerializeField]
    bool pressed = false;
    // Use this for initialization
    void Start()
    {
        for(int i=0; i<data.Count();  i++)
        {
            data[i] = "";
        }
    }

    public void EmptyRow()
    {
        ChangeIdentifier("");
        for (int i = 0; i < data.Count(); i++)
        {
            data[i] = "";
        }
    }
    public void SetData(string[] tdata)
    {
        data = tdata;
        foreach (string value in tdata)
        {
            //Debug.Log("SET DATA VAL ==============="+value);
        }
    }

    public void SetPlane(Transform aircraftObj)
    {
        aircraft = aircraftObj;
       // Debug.Log("aircraft" + aircraft);
    }

    public Transform GetPlane()
    {
        return aircraft;
    }

    public void SetIdentifierName(string tname)
    {
        identifierName = tname;
    }
    public void SetIndex(int idx)
    {
        ShowIndex = idx;
    }

    public void SetDeactivate()
    {

        if (aircraft)
        {
            if (aircraft.GetChild(0))
            {

                aircraft.GetChild(0).gameObject.SetActive(false);
              //  aircraft.gameObject.GetComponent<movement>().callSignObj.GetComponent<TextMesh>().text = "";
                aircraft.gameObject.GetComponent<TrailRenderer>().enabled = false;

            }
         
        }
        string[] dummy = { data[0] };
        SetData(dummy);

    }

    public void SetActivate()
    {
        if(aircraft)
        {
            if(aircraft.GetChild(0))
            {
 
                aircraft.GetChild(0).gameObject.SetActive(true);
                aircraft.gameObject.GetComponent<TrailRenderer>().enabled = true;
                //GameObject dum = aircraft.gameObject.GetComponent<movement>().callSignObj;
                
            }
         
           
        }
     
    }

    public void Pressed()
    {
        if (pressed)
        {
            pressed = false;
            SetActivate();
        }
        else
        {
            pressed = true;
            SetDeactivate();
        }
        Debug.Log("Called useless");

    }

    public void SetWidth(int width)
    {
        //Excluding identifier - so the width is always + 1
        NoOfFrontPlates = width;
    }

    public void SetHeight(int height)
    {
        //Excluding identifier - so the width is always + 1
        NoOfRows = height;
    }
    public void Init()
    {
        float offset = 0.13f;
        float spacing = 0.101f;
        for (int i = 0; i < NoOfFrontPlates; i++)
        {

            GameObject fpGameObject = Instantiate(frontPlate, transform) as GameObject;
            fpGameObject.transform.position = new Vector3(transform.position.x + offset + (i * spacing), transform.position.y, transform.position.z);
            //fpGameObject.GetComponent<FrontPlate>().ChangeText("");

        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
            
        int i = 0;
        foreach (Transform child in transform)
        {

           
            if (child.name == "Identifier")
            {
                continue;
            }


            GameObject fpGameObject = child.gameObject;
            fpGameObject.GetComponent<FrontPlate>().ChangeText("");
            
            if ((i + (ShowIndex * (transform.childCount - 1))) < data.Length)
            {

                fpGameObject.GetComponent<FrontPlate>().ChangeText(data[i + (ShowIndex * (transform.childCount - 1))]);
                i++;
            }

        }
    }

    public void ChangeIdentifier(string text)
    {
        transform.GetChild(0).gameObject.GetComponent<FrontPlate>().ChangeText(text);
    }



}
