using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;


public class SaveControl : MonoBehaviour {

    XmlWriter xmlWriter;
    public GameObject status;
    public GameObject lineParent;
    public GameObject BullParent;
    public GameObject circleParent;
    private bool isSuccessful = false;


    public void CreateXML()
    {
        
        string path = StandaloneFileBrowser.SaveFilePanel("Save Exercise Area", @"C:\Users\zeeshan\Documents\Unity Projects\Phase 2 Completed March\Assets\Resources\ExerciseAreaFiles", "ExerciseArea01", "xml");

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = "\t";
        xmlWriter = XmlWriter.Create(path , settings);
        if (path.Equals("")) { return; }
        if (File.Exists(path))
        {
            Debug.Log("File is created");
            XMLOperation();
            //AssetDatabase.Refresh();                                                                                                                                          //This is Using UnityEditor Namespace!
        }
    }

    public void XMLOperation()
    {
        //Create XML Document
        xmlWriter.WriteStartDocument();
        //start tag for exercise area
        xmlWriter.WriteStartElement("ExerciseArea");

        //Add line tag
        AddLines();
        AddBullsEye();
        AddCircle();
        xmlWriter.WriteEndElement();

        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
        DisplaySuccess(true);

    }
   
    private void AddLines()
    {
        xmlWriter.WriteStartElement("Lines");
        int count = 0;
        foreach (Transform child in lineParent.transform)
        {
            Line currentLine = child.GetComponent<LineSpecs>().lineDetails;

            Debug.Log("st lat = " + currentLine.Latitude_Start + "st lon = " + currentLine.Longitude_Start);
            Debug.Log("end lat = " + currentLine.Latitude_End + "end lon = " + currentLine.Longitude_End);

            xmlWriter.WriteStartElement("Line");
            xmlWriter.WriteAttributeString("Transparency", "1.0");
            xmlWriter.WriteAttributeString("Name", "line");
            xmlWriter.WriteAttributeString("LineColor", currentLine.LineColor);
            xmlWriter.WriteAttributeString("ID", count.ToString());
            xmlWriter.WriteAttributeString("Longitude_Start", currentLine.Longitude_Start);
            xmlWriter.WriteAttributeString("Longitude_End", currentLine.Longitude_End);
            xmlWriter.WriteAttributeString("LineWidth", "3.0");
            xmlWriter.WriteAttributeString("LineStyle", "Standard");
            xmlWriter.WriteAttributeString("Latitude_Start", currentLine.Latitude_Start);
            xmlWriter.WriteAttributeString("Latitude_End", currentLine.Latitude_End);
            xmlWriter.WriteEndElement();
            
            count++;
        }
        xmlWriter.WriteEndElement();

    }
 
    private void AddBullsEye()
    {
        xmlWriter.WriteStartElement("BullsEye");
        int count = 0;
        foreach (Transform child in BullParent.transform)
        {
           BullEye currentBull = child.GetComponent<BullSpecs>().BullDetails;

            xmlWriter.WriteStartElement("BullsEye");
            xmlWriter.WriteAttributeString("Latitude", currentBull.Latitude);
            xmlWriter.WriteAttributeString("Longitude", currentBull.Longitude);
            xmlWriter.WriteAttributeString("LineAngle", currentBull.LineAngle);
            xmlWriter.WriteAttributeString("ID", currentBull.ID);
            xmlWriter.WriteAttributeString("LineColor", currentBull.LineColor);
            xmlWriter.WriteAttributeString("LineThickness", currentBull.LineThickness);
            xmlWriter.WriteAttributeString("Name", currentBull.Name);
            xmlWriter.WriteAttributeString("NumOfCircles", currentBull.NumOfCircles);
            xmlWriter.WriteAttributeString("Radius", currentBull.Radius);
            xmlWriter.WriteAttributeString("Unit", currentBull.Unit);
            xmlWriter.WriteEndElement();
            count++;
        
        }
        xmlWriter.WriteEndElement();

    }

    private void AddCircle()
    {
        xmlWriter.WriteStartElement("Circle");
        int count = 0;
        foreach (Transform child in circleParent.transform)
        {
            Circle currentCircle = child.GetComponent<CircleSpecs>().circleDeails;

            xmlWriter.WriteStartElement("Circle");
            xmlWriter.WriteAttributeString("Latitude", currentCircle.Latitude);
            xmlWriter.WriteAttributeString("Longitude", currentCircle.Longitude);
            xmlWriter.WriteAttributeString("XRadius", currentCircle.XRadius);
            xmlWriter.WriteAttributeString("YRadius", currentCircle.YRadius);
            xmlWriter.WriteAttributeString("ID", currentCircle.ID);
            xmlWriter.WriteAttributeString("Name", currentCircle.Name);
            xmlWriter.WriteAttributeString("Unit", currentCircle.Unit);
            xmlWriter.WriteAttributeString("Label", currentCircle.Label);
            xmlWriter.WriteEndElement();
            count++;

        }
        xmlWriter.WriteEndElement();

    }

    private void AddGeneralGeometry()
    {
        xmlWriter.WriteStartElement("General_Geometrys");

        int count = 0;
        foreach (Transform child in lineParent.transform)//needstobechanged
        {
            //Line currentLine = child.GetComponent<LineSpecs>().lineDetails;

            xmlWriter.WriteStartElement("General_Geometry");
            xmlWriter.WriteAttributeString("FillColor", "");
            xmlWriter.WriteAttributeString("HorizontalRadius", "");
            xmlWriter.WriteAttributeString("VerticalRadius", "");
            xmlWriter.WriteAttributeString("Latitude", "");
            xmlWriter.WriteAttributeString("Longitude","");
            xmlWriter.WriteAttributeString("ID", "");
            xmlWriter.WriteAttributeString("Name", "");
            xmlWriter.WriteAttributeString("Unit", "");
            xmlWriter.WriteAttributeString("Transparency", "");
            xmlWriter.WriteAttributeString("Type", "");
            xmlWriter.WriteAttributeString("LineColor", "");
            xmlWriter.WriteAttributeString("LineThickness", "");
            xmlWriter.WriteAttributeString("Rotation", "");
           
            xmlWriter.WriteEndElement();
            count++;
        }
        xmlWriter.WriteEndElement();
    }

    void DisplaySuccess(bool isSuccessful, System.Exception exception = null)
    {

        status.SetActive(true);

        if (isSuccessful)
        {
            status.transform.Find("Successful").gameObject.SetActive(true);
        }
        else
        {
            status.transform.Find("Unsuccessful").gameObject.SetActive(true);
            status.transform.Find("Unsuccessful").GetChild(1).GetComponent<Text>().text = exception.Message;
        }

        Invoke("HideSuccess", 3.0f);
    }

    void HideSuccess()
    {

        foreach (Transform child in status.transform)
        {
            child.gameObject.SetActive(false);
        }
        status.SetActive(false);
    }
}
