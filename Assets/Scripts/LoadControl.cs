using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using SFB;

public class LoadControl : MonoBehaviour {
    
    public Transform lineParent;
    public GameObject DrawManager; 
    public GameObject lineprefab;
    private GameObject currentline;
    public ListBox lineListBox;

    int count = 0;

    private GameObject currentpoint;
    public GameObject PointPrefab;
    public Transform BullEyeParent;
    public ListBox BullListBox;
    int countB = 0;

    public Transform circleParent;
    public GameObject circlePrefab;
    public ListBox CircleListBox;
    private GameObject currentCircle;
    int countC = 0;

    public float ConvertDegreeAngleToDouble(string DMS)
    {
        UnityEngine.Debug.Log("DMS orig: " + DMS);
        DMS = DMS.Replace("&quot;", "");
        DMS = DMS.Replace('°', ',');
        DMS = DMS.Replace('\'', ',');
        DMS = DMS.Replace('"', ' ');
        //string[] splited = DMS.Split(new char[] { '°', '\'', '"' });
        UnityEngine.Debug.Log("DMS Spl: " + DMS );
        string[] splited = DMS.Split(',');

        float Degree = float.Parse(splited[0]);
        float Minute = float.Parse(splited[1]);
        float Second = float.Parse(splited[2]);

        return Degree + (Minute / 60) + (Second / 3600);
    }


    public void OnLoadClick()
    {
       
        var path = StandaloneFileBrowser.OpenFilePanel("Load Excercise Area", @"C:\Users\zeeshan\Documents\Unity Projects\Phase 2 Completed March\Assets\Resources\ExerciseAreaFiles", "xml", false);                           

        if (path.Equals("")) { return; }

         UnityEngine.Debug.Log("OPP" + path);

         GetExerciseArea(path[0]);
    }

    void GetExerciseArea(string path)
    {
        //Read XML file and parse it for file location of textures (.jpg)
        XmlDocument configFile = new XmlDocument();
        configFile.Load(path);
        XmlNode xmlRootNode = configFile.DocumentElement;
        XmlNodeList xmlNodeList = xmlRootNode.ChildNodes;
        XmlNodeList lines;
        XmlNodeList bullseyes;
        XmlNodeList circle;

        List<List<float>> ExerciseArea = new List<List<float>>();

        int i = 0;

        //For each child node in "ExerciseArea" tag
        foreach (XmlNode child in xmlNodeList)
        {
            i++;
            //If child name is "Lines"
            if (child.Name == "Lines")
            {

                //Get all children of line
                lines = child.ChildNodes;
                foreach (XmlNode subChild in lines)
                {
                    //UnityEngine.Debug.Log("found line");
                    //UnityEngine.Debug.Log(subChild.Attributes.GetNamedItem("LineColor").Value);

                    List<float> lineVertices = new List<float>();
                    string lineColor = subChild.Attributes.GetNamedItem("LineColor").Value.ToString();
                    lineVertices.Add(0);

                    UnityEngine.Debug.Log("load st lat = " + subChild.Attributes.GetNamedItem("Latitude_Start").Value + "load st lon = " + subChild.Attributes.GetNamedItem("Longitude_Start").Value);
                    UnityEngine.Debug.Log("load end lat = " + subChild.Attributes.GetNamedItem("Latitude_End").Value + "load end lon = " + subChild.Attributes.GetNamedItem("Longitude_End").Value);

                    //UnityEngine.Debug.Log(subChild.Attributes.GetNamedItem("LineWidth").Value);
                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("LineWidth").Value));

                    lineVertices.Add(ConvertDegreeAngleToDouble(subChild.Attributes.GetNamedItem("Longitude_Start").Value));
                    //UnityEngine.Debug.Log(subChild.Attributes.GetNamedItem("Longitude_Start").Value);

                    lineVertices.Add(ConvertDegreeAngleToDouble(subChild.Attributes.GetNamedItem("Longitude_End").Value));
                    //UnityEngine.Debug.Log(subChild.Attributes.GetNamedItem("Longitude_End").Value);

                    lineVertices.Add(ConvertDegreeAngleToDouble(subChild.Attributes.GetNamedItem("Latitude_Start").Value));
                    //UnityEngine.Debug.Log(subChild.Attributes.GetNamedItem("Latitude_Start").Value);

                    lineVertices.Add(ConvertDegreeAngleToDouble(subChild.Attributes.GetNamedItem("Latitude_End").Value));
                    //UnityEngine.Debug.Log(subChild.Attributes.GetNamedItem("Latitude_End").Value);
                    
                    
                    ExerciseArea.Add(lineVertices);
                    Drawline(lineVertices, lineColor);
                }
                //return ExerciseArea;
            }

            if (child.Name == "BullsEye")
            {
                UnityEngine.Debug.Log("found bullseye");
                bullseyes = child.ChildNodes;
                foreach (XmlNode subChild in bullseyes)
                {
                    List<float> bullsEye = new List<float>();

                    //bullsEye.Add(float.Parse(subChild.Attributes.GetNamedItem("LineColor").Value));

                    bullsEye.Add(ConvertDegreeAngleToDouble(subChild.Attributes.GetNamedItem("Latitude").Value));
                    bullsEye.Add(ConvertDegreeAngleToDouble(subChild.Attributes.GetNamedItem("Longitude").Value));
                    bullsEye.Add(0);
                    bullsEye.Add(float.Parse(subChild.Attributes.GetNamedItem("NumOfCircles").Value));

                    DrawBull(bullsEye, subChild.Attributes.GetNamedItem("LineColor").Value.ToString());

                }
                //return BullsEyeList;
            }


            if (child.Name == "Circle")
            {
                UnityEngine.Debug.Log("found circle");
                circle = child.ChildNodes;
                foreach (XmlNode subChild in circle)
                {
                    List<float> circles = new List<float>();
                    circles.Add(ConvertDegreeAngleToDouble(subChild.Attributes.GetNamedItem("Latitude").Value));
                    circles.Add(ConvertDegreeAngleToDouble(subChild.Attributes.GetNamedItem("Longitude").Value));
                    circles.Add(float.Parse(subChild.Attributes.GetNamedItem("XRadius").Value));
                    circles.Add(float.Parse(subChild.Attributes.GetNamedItem("YRadius").Value));

                    DrawCircle(circles);

                }
                //return BullsEyeList;
            }
        }
    //return null;

    }

    public void DrawBull(List<float> points, string BullEyeColorHexCode)
    {
        GameObject point = Instantiate(PointPrefab, BullEyeParent);
        float[] p1 = DrawManager.GetComponent<DrawControl>().LatLongToXY(points[0], points[1]);
        point.transform.position = new Vector3 (p1[0], p1[1], -1);
        point.name = "BullEye" + countB.ToString();

        SpriteRenderer spriteRend = point.GetComponent<SpriteRenderer>();

        #region Color Converters


        //Reading Color in HEX Format From Excercise Area XML File*************

        //float r, g, b;
        //string rs, gs, bs;

        //rs = BullEyeColorHexCode.Substring(0, 2);
        //gs = BullEyeColorHexCode.Substring(2, 2);
        //bs = BullEyeColorHexCode.Substring(4, 2);

        //UnityEngine.Debug.Log("BullEye : Before conversion values of RS = {0} , GS = {1} && Bs = {2}" + rs + gs + bs);

        ////r = Convert.ToInt32(rs, 16);
        ////b = Convert.ToInt32(gs, 16);
        ////g = Convert.ToInt32(bs, 16);

        //r = int.Parse(rs, System.Globalization.NumberStyles.HexNumber);
        //g = int.Parse(gs, System.Globalization.NumberStyles.HexNumber);
        //b = int.Parse(bs, System.Globalization.NumberStyles.HexNumber);

        //UnityEngine.Debug.Log("BullEye : After conversion values of R = {0} , G = {1} && B = {2}" + r + g + b);

        //Reading Color in HEX Format From Excercise Area XML File*************

        //Reading Color in RGBA Format From Excercise Area XML File***********

        //string als, rs, gs, bs;

        //rs = BullEyeColorHexCode.Split(';')[0];
        //gs = BullEyeColorHexCode.Split(';')[1];
        //bs = BullEyeColorHexCode.Split(';')[2];
        //als = BullEyeColorHexCode.Split(';')[3];

        //Color clr = new Color();
        //clr.r = float.Parse(rs);
        //clr.g = float.Parse(gs);
        //clr.b = float.Parse(bs);
        //clr.a = float.Parse(als);

        //Reading Color in RGBA Format From Excercise Area XML File************

        #endregion

        string intToHexColour = ColorIntToHex(Convert.ToInt32(BullEyeColorHexCode));
        spriteRend.material.color = HexToColor(intToHexColour);


        BullListBox.AddItem(new ListBox.ListItem(point.name));

        currentpoint = point;
        countB++;
        currentpoint.GetComponent<BullSpecs>().SetPointLatLong(new Vector2 (points[0], points[1]));
    }

    public void Drawline(List<float> points, string LineColorHexCode)
    {
        //format long1, long2, lat1, lat2

        GameObject line = Instantiate(lineprefab, lineParent);
        line.name = "line" + count.ToString();
        

        float[] p1 = DrawManager.GetComponent<DrawControl>().LatLongToXY(points[4], points[2]);
        float[] p2 = DrawManager.GetComponent<DrawControl>().LatLongToXY(points[5], points[3]);

        #region ColorConverts


        //Reading Color in HEX Format From Excercise Area XML File*************

        //float r, g, b;
        //string rs, gs, bs;

        //rs = LineColorHexCode.Substring(0, 2);
        //gs = LineColorHexCode.Substring(2, 2);
        //bs = LineColorHexCode.Substring(4, 2);

        //UnityEngine.Debug.Log("LINE : Before conversion values of RS = {0} , GS = {1} && Bs = {2}" + rs + gs + bs);
        ////r = Convert.ToInt32(rs, 16);
        ////b = Convert.ToInt32(gs, 16);
        ////g = Convert.ToInt32(bs, 16);

        //r = int.Parse(rs, System.Globalization.NumberStyles.HexNumber);
        //g = int.Parse(gs, System.Globalization.NumberStyles.HexNumber);
        //b = int.Parse(bs, System.Globalization.NumberStyles.HexNumber);
        //UnityEngine.Debug.Log("LINE : After conversion values of R = {0} , G = {1} && B = {2}" + r + g + b);

        //Reading Color in HEX Format From Excercise Area XML File*************

        //Reading Color in RGBA Format From Excercise Area XML File*************

        //string als, rs, gs, bs;
        //rs = LineColorHexCode.Split(';')[0];
        //gs = LineColorHexCode.Split(';')[1];
        //bs = LineColorHexCode.Split(';')[2];
        //als = LineColorHexCode.Split(';')[3];

        //Color clr = new Color();
        //clr.r = float.Parse(rs);
        //clr.g = float.Parse(gs);
        //clr.b = float.Parse(bs);
        //clr.a = float.Parse(als);

        //Reading Color in RGBA Format From Excercise Area XML File**************

        #endregion
        LineRenderer lnRend = line.GetComponent<LineRenderer>();
        string intToHexColour = ColorIntToHex(Convert.ToInt32(LineColorHexCode));
        lnRend.material.color = HexToColor(intToHexColour);

        currentline = line;
        count++;
        lineListBox.AddItem(new ListBox.ListItem(line.name));
        
        currentline.GetComponent<LineSpecs>().SetLineStart(new Vector3(p1[0], p1[1], -1));
        currentline.GetComponent<LineSpecs>().SetLineEnd(new Vector3(p2[0], p2[1], -1));

        currentline.GetComponent<LineSpecs>().SetStart(new Vector2(points[4], points[2]));
        currentline.GetComponent<LineSpecs>().SetEnd(new Vector2(points[5], points[3]));
    }


    public void DrawCircle(List<float> points)
    {

        GameObject Circle = Instantiate(circlePrefab, circleParent);

        float[] p1 = DrawManager.GetComponent<DrawControl>().LatLongToXY(points[0], points[1]);

        Circle.transform.position = new Vector3(p1[0], p1[1],-1);
        Circle.AddComponent<PolygonCollider2D>();
        Circle.transform.localScale = new Vector3(points[2], points[3],0);

        Circle.name = "Circle" + countC.ToString(); //Added

        CircleListBox.AddItem(new ListBox.ListItem(Circle.name)); //Added

        currentCircle = Circle;
        countC++;

        currentCircle.GetComponent<CircleSpecs>().SetXYRadius(points[2], points[3]);
        currentCircle.GetComponent<CircleSpecs>().SetPointLatLong(new Vector2(points[0], points[1]));

    }

    string ColorIntToHex(int colorValue)
    {
        return colorValue.ToString("X8");
    }

    Color HexToColor(string hex)
    {
        Color col = new Color();
     
        string red = hex.Substring(0, 2);
        string green = hex.Substring(2, 2);
        string blue = hex.Substring(4, 2);
        string alpha = hex.Substring(6, 2);
        col.r = int.Parse(red, System.Globalization.NumberStyles.AllowHexSpecifier);
        col.g = int.Parse(green, System.Globalization.NumberStyles.AllowHexSpecifier);
        col.b = int.Parse(blue, System.Globalization.NumberStyles.AllowHexSpecifier);
        col.a = int.Parse(alpha, System.Globalization.NumberStyles.AllowHexSpecifier);
        
        return col;
    }


    public void ClearPreviousDrawing()
    {
        foreach (Transform child in lineParent)
        {
            GameObject.Destroy(child.gameObject);

        }


        foreach (Transform child in BullEyeParent)
        {
            GameObject.Destroy(child.gameObject);

        }

        foreach (Transform child in circleParent)
        {
            GameObject.Destroy(child.gameObject);

        }

        foreach (RectTransform line in lineListBox.content)
        {
            GameObject.Destroy(line.gameObject);

        }

        foreach (RectTransform bull in BullListBox.content)
        {
            GameObject.Destroy(bull.gameObject);

        }

        foreach (RectTransform circle in CircleListBox.content)
        {
            GameObject.Destroy(circle.gameObject);

        }

    }

   

}
