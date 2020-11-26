using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CircleSpecs : MonoBehaviour {
    
    public Circle circleDeails;
    bool init = false;
    bool drawRay = false;
    Ray outray;
    GameObject hoverCircle;
  
    void Init()
    {
        circleDeails = new Circle();
        init = true;
    }

	void Update () {

        if (drawRay)
        {
            Debug.DrawRay(outray.origin, outray.direction * 1000f,Color.yellow);
        }

	}

    public string DecimalDegToDMS(float decimalDegree)
    {
        // set decimal_degrees value here
        double degree = Math.Truncate(decimalDegree);
        double minutes = (decimalDegree - Math.Floor(decimalDegree)) * 60.0;
        double seconds = (minutes - Math.Floor(minutes)) * 60.0;
        //double tenths = (seconds - Math.Floor(seconds)) * 10.0;

        // get rid of fractional part
        minutes = Math.Floor(minutes);
        seconds = Math.Floor(seconds);
        //tenths = Math.Floor(tenths);

        string DMS = degree + "°" + minutes + "'" + seconds + "\"";
        return DMS;
    }

    public void SetPointLatLong(Vector2 value)
    {
        if (!init)
            Init();
        circleDeails.Latitude = DecimalDegToDMS(value.x);
        circleDeails.Longitude = DecimalDegToDMS(value.y);
        //circleDeails.Latitude = value.x.ToString();
        //circleDeails.Longitude = value.y.ToString();
    }
    
   
    public void SetXYRadius(float xRadius,float yRadius)
    {
        
        if (!init)
            Init();
        circleDeails.XRadius = xRadius.ToString();
        circleDeails.YRadius = yRadius.ToString();

    }
    
    public GameObject Detected(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 1000f);
        if (hit)
        {
            Debug.Log(hit.transform.name);
            outray = ray;
            if(hit.transform.gameObject == gameObject)
            {
                return gameObject;
            }
        }

        return null;
    }

    public List<string> OnSelection()
    {
        List<string> specs = new List<string> { };
        specs.Add(circleDeails.Latitude);
        specs.Add(circleDeails.Longitude);
        specs.Add(circleDeails.XRadius);
        specs.Add(circleDeails.YRadius);
        //specs.Add(circleDeails.Label);

        return specs;

    }

    public void UpdateCircleSpec(List<string> specs)
    {
        circleDeails.Latitude = specs[0];
        circleDeails.Longitude = specs[1];
        circleDeails.XRadius = specs[2];
        circleDeails.YRadius = specs[3];
        //circleDeails.Label = specs[4];
    }

    public void updateMousePos(List<float> specs)
    {
        SetStartPoint(new Vector3(specs[0], specs[1], -1));
    }

    void SetStartPoint(Vector3 point)
    {
        gameObject.transform.position = point;
    }


}
