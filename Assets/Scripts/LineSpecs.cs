using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LineSpecs : MonoBehaviour
{

    public Line lineDetails;
    LineRenderer lineRend;
    bool init = false;
   

    void Init()
    {
        lineDetails = new Line();
        //Debug.Log("Started");
        lineRend = gameObject.GetComponent<LineRenderer>();
        lineRend.positionCount = 2;
        init = true;

    }

    public GameObject Detected(Vector2 point)
    {
        point = Camera.main.ScreenToWorldPoint(point);

        Vector2 direction = (new Vector2(lineRend.GetPosition(1).x, lineRend.GetPosition(1).y) - new Vector2(lineRend.GetPosition(0).x, lineRend.GetPosition(0).y)).normalized;
        Vector2 newDirection = (new Vector2(lineRend.GetPosition(1).x, lineRend.GetPosition(1).y) - point).normalized;

        bool isXequal = System.Math.Round(newDirection.x, 1) == System.Math.Round(direction.x, 1);
        bool isYequal = System.Math.Round(newDirection.y, 1) == System.Math.Round(direction.y, 1);
        if (isXequal && isYequal)
        {
            //Debug.Log(gameObject.name);
            return gameObject;
        }

        return null;
    }


    public void SetLineStart(Vector3 worldPos)
    {
        if (!init)
            Init();
        lineRend.SetPosition(0, worldPos);

    }

    public void SetLineEnd(Vector3 worldPos)
    {
        if (!init)
            Init();
        lineRend.SetPosition(1, worldPos);

    }

    public void UpdateCollider()
    {
        //gameObject.AddComponent<MeshCollider>();
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

    public void SetStart(Vector2 value)
    {
        if (!init)
            Init();

        lineDetails.Latitude_Start = DecimalDegToDMS(value.x);
        lineDetails.Longitude_Start = DecimalDegToDMS(value.y);

        //lineDetails.Latitude_Start = value.x.ToString();
        //lineDetails.Longitude_Start = value.y.ToString();
    }

    public void SetEnd(Vector2 value)
    {
        if (!init)
            Init();

        lineDetails.Latitude_End = DecimalDegToDMS(value.x);
        lineDetails.Longitude_End = DecimalDegToDMS(value.y);

        //lineDetails.Latitude_End = value.x.ToString();
        //lineDetails.Longitude_End = value.y.ToString();
    }

    public void SetLineColor(Color lineColor)
    {
        //lineDetails.LineColor = (int) lineColor.r +""+(int)lineColor.g +""+(int)lineColor.b;
        //lineDetails.LineColor = lineColor.r.ToString() + ";" + lineColor.g.ToString() + ";" + lineColor.b.ToString() + ";" + lineColor.a.ToString();
        //int clr = lineColor.GetHashCode();
        //lineDetails.LineColor = clr.ToString();

        string HexColour = ColorUtility.ToHtmlStringRGBA(lineColor);
        int decValue = int.Parse(HexColour, System.Globalization.NumberStyles.AllowHexSpecifier);
        lineDetails.LineColor = decValue.ToString();
    }

    public List<string> OnSelection()
    {
        List<string> specs = new List<string> { };
        specs.Add(lineDetails.Latitude_Start);
        specs.Add(lineDetails.Longitude_Start);
        specs.Add(lineDetails.Latitude_End);
        specs.Add(lineDetails.Longitude_End);
        //specs.Add(lineDetails.Label);
        return specs;

    }


    public void updateSpecs(List<string> specs)
    {
        lineDetails.Latitude_Start = specs[0];
        lineDetails.Longitude_Start = specs[1];
        lineDetails.Latitude_End = specs[2];
        lineDetails.Longitude_End = specs[3];
        //lineDetails.Label = specs[4];
    }

    public void updateMousePos(List<float> specs)
    {
        SetLineStart(new Vector3(specs[0], specs[1], -10));
        SetLineEnd(new Vector3(specs[2], specs[3], -10));

    }

    public void delete()
    {
        Debug.Log("object destroyed");
        Destroy(gameObject);
    }

}

