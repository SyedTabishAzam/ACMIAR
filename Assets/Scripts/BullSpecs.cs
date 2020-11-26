using System;
using System.Collections;
using System.Collections.Generic;
using Meta.Mouse;
using UnityEngine;
using UnityEngine.UI;

public class BullSpecs : MonoBehaviour {
   
    public BullEye BullDetails;
    bool init = false;
    bool detected = false;
    

    void Init()
    {
        BullDetails = new BullEye();
        SetName();
        init = true;
    }


    private void OnMouseDown()
    {
        Debug.Log("Mouse down on bullseye");
        detected = true;
    }

    private void OnMouseUp()
    {
        detected = false;
    }
    public GameObject Detected( )
    {
        if(detected)
        {
            detected = false;
            return gameObject;
        }
        
        return null;
    }

    public void SetBullEyeColor(Color bullEyeColor)
    {

        //BullDetails.LineColor = (int)bullEyeColor.r + "" + (int)bullEyeColor.g + "" + (int)bullEyeColor.b;
        //BullDetails.LineColor = bullEyeColor.r.ToString() + ";" + bullEyeColor.g.ToString() + ";" + bullEyeColor.b.ToString() + ";" + bullEyeColor.a.ToString();
        //int clr = bullEyeColor.GetHashCode();

        //BullDetails.LineColor = clr.ToString();

        string HexColour = ColorUtility.ToHtmlStringRGBA(bullEyeColor);

        int decValue = int.Parse(HexColour, System.Globalization.NumberStyles.AllowHexSpecifier);

        BullDetails.LineColor = decValue.ToString();

    }

    void SetBullPoint(Vector3 point)
    {
        gameObject.transform.position = point; 
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
        //Debug.Log("DMS to Degree = " + ConvertDegreeAngleToDouble((float)degree, (float)minutes, (float)seconds).ToString("#.000"));

        return DMS;
    }

    public void SetPointLatLong(Vector2 value)
    {
        if (!init)
            Init();
        BullDetails.Latitude =DecimalDegToDMS(value.x);
        BullDetails.Longitude =DecimalDegToDMS(value.y);
    }

    public void SetName()
    {
        BullDetails.Name = gameObject.name;
        BullDetails.ID = BullDetails.Name[BullDetails.Name.Length - 1].ToString(); 
    }
    
    
     public List<string> OnSelection()
    {
        List<string> specs = new List<string> { };
        specs.Add(BullDetails.Latitude);
        specs.Add(BullDetails.Longitude);
        //specs.Add(BullDetails.Label);
        
        return specs;

    }
    
    public void UpdateBullEyeSpecs(List<string> specs)
    {
        BullDetails.Latitude = specs[0];
        BullDetails.Longitude = specs[1];
        //BullDetails.Label = specs[2];
    }

    public void updateMousePos(List<float> specs)
    {
        SetBullPoint(new Vector3(specs[0], specs[1], -10));

    }
    public void Delete()
    {
        Debug.Log("object destroyed");
        Destroy(gameObject);
    }
}
