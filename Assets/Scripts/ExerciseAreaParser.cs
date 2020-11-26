using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using SFB;
public class ExerciseAreaParser : MonoBehaviour
{

    public TextAsset ConfigFile;
    public GameObject BullseyeManager;
    public Transform exAreaParent;

    string exAreaFromAddmissionPath;
    string loadedExcerciseAreaPath;
    string[] excerciseAreaFilePath;
    ExtensionFilter[] extensions = new[] { new ExtensionFilter("xml", "*") };

    void Start()
    {
        //    List<List<float>> exerciseArea = GetExerciseArea();
        //    List<List<float>> exerciseAreaCircles = GetCircles();
        //    List<List<float>> exerciseAreaBullsEye = GetBullsEyeList();
        //    //if (exerciseArea!=null && exerciseArea.Count >0)
        //    //{
        //    GetComponent<ExerciseAreaManager>().Drawline(LatLongToVertices(exerciseArea));
        //    GetComponent<ExerciseAreaManager>().DrawBullEye(LatLongToVerticesForBullsEye(exerciseAreaBullsEye));
        //    GetComponent<ExerciseAreaManager>().DrawCircle(LatLongToVerticesForCircles(exerciseAreaCircles));
        //    BullseyeManager.GetComponent<BullEyeHandle>().StoreExererciseAreasBullseye(GetBullsEyeList());
        //    //}

        LoadExAreaFromOtherScene();
        ShowExcerciseArea();
    }

    public void OnLoadDestroy()
    {
        foreach (Transform child in exAreaParent)
        {
            GameObject.Destroy(child.gameObject);

        }

    }

    public void LoadExcerciseArea()
    {

        excerciseAreaFilePath = StandaloneFileBrowser.OpenFilePanel("Choose Exercise Area To Load", Application.dataPath, extensions, false);
        loadedExcerciseAreaPath = excerciseAreaFilePath[0].ToString();
    }

    public void LoadExAreaFromOtherScene()
    {
        exAreaFromAddmissionPath = PlayerPrefs.GetString("ExerciseAreaPathFromAddmissionScene");
        loadedExcerciseAreaPath = exAreaFromAddmissionPath;
    }

    public void ShowExcerciseArea()
    {
        
        List<List<float>> exerciseArea = GetExerciseArea();
        List<List<float>> exerciseAreaBullsEye = GetBullsEyeList();
        List<List<float>> exerciseAreaCircles = GetCircles();

        GetComponent<ExerciseAreaManager>().Drawline(LatLongToVertices(exerciseArea));
        GetComponent<ExerciseAreaManager>().DrawBullEye(LatLongToVerticesForBullsEye(exerciseAreaBullsEye));
        GetComponent<ExerciseAreaManager>().DrawCircle(LatLongToVerticesForCircles(exerciseAreaCircles));
        BullseyeManager.GetComponent<BullEyeHandle>().StoreExererciseAreasBullseye(GetBullsEyeList());

    }

    public List<List<float>> GetBullsEyeList()
    {
        //Read XML file and parse it for file location of textures (.jpg)
        //if (ConfigFile)
        //{
        //configFile.Load(Application.dataPath + "/Resources/ExerciseAreaFiles/" + ConfigFile.name + ".xml");

        XmlDocument configFile = new XmlDocument();
        configFile.Load(loadedExcerciseAreaPath);
        XmlNode xmlRootNode = configFile.DocumentElement;
        XmlNodeList xmlNodeList = xmlRootNode.ChildNodes;
        XmlNodeList bullseyes;

        List<List<float>> BullsEyeList = new List<List<float>>();
        int i = 0;

        //For each child node in "ExerciseArea" tag
        foreach (XmlNode child in xmlNodeList)
        {
            i++;

            if (child.Name == "BullsEye")
            {
                //Debug.Log("found bullseye");
                bullseyes = child.ChildNodes;
                foreach (XmlNode subChild in bullseyes)
                {
                    List<float> bullsEye = new List<float>();
                    bullsEye.Add(float.Parse(subChild.Attributes.GetNamedItem("Latitude").Value));
                    bullsEye.Add(float.Parse(subChild.Attributes.GetNamedItem("Longitude").Value));
                    bullsEye.Add(float.Parse(subChild.Attributes.GetNamedItem("LineColor").Value));
                    BullsEyeList.Add(bullsEye);

                }
                return BullsEyeList;
            }
        }
        //}
        //Debug.LogError("Config file not selected. Exercies Area wont be loaded");
        return null;
    }

    List<List<float>> GetExerciseArea()
    {
        //Read XML file and parse it for file location of textures (.jpg)
        //if (ConfigFile)
        //{
        //configFile.Load(Application.dataPath + "/Resources/ExerciseAreaFiles/" + ConfigFile.name + ".xml");

        XmlDocument configFile = new XmlDocument();
        configFile.Load(loadedExcerciseAreaPath);
        XmlNode xmlRootNode = configFile.DocumentElement;
        XmlNodeList xmlNodeList = xmlRootNode.ChildNodes;
        XmlNodeList lines;

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
                    List<float> lineVertices = new List<float>();
                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("LineColor").Value));
                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("LineWidth").Value));
                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("Longitude_Start").Value));
                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("Longitude_End").Value));
                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("Latitude_Start").Value));
                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("Latitude_End").Value));
                    ExerciseArea.Add(lineVertices);
                }

                return ExerciseArea;
            }
        }
        //}
        //Debug.LogError("Config file not selected. Exercies Area wont be loaded");
        return null;

    }

    List<List<float>> GetCircles()
    {
        //Read XML file and parse it for file location of textures (.jpg)
        //if (ConfigFile)
        //{
        //configFile.Load(Application.dataPath + "/Resources/ExerciseAreaFiles/" + ConfigFile.name + ".xml");

        XmlDocument configFile = new XmlDocument();
        configFile.Load(loadedExcerciseAreaPath);
        XmlNode xmlRootNode = configFile.DocumentElement;
        XmlNodeList xmlNodeList = xmlRootNode.ChildNodes;
        XmlNodeList circles;

        List<List<float>> ExerciseArea = new List<List<float>>();

        int i = 0;

        //For each child node in "ExerciseArea" tag
        foreach (XmlNode child in xmlNodeList)
        {
            i++;
            //If child name is "Circle"
            if (child.Name == "Circle")
            {

                //Get all children of circle
                circles = child.ChildNodes;
                foreach (XmlNode subChild in circles)
                {
                    List<float> lineVertices = new List<float>();

                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("Latitude").Value));
                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("Longitude").Value));
                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("XRadius").Value));
                    lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("YRadius").Value));
                    //lineVertices.Add(float.Parse(subChild.Attributes.GetNamedItem("Label").Value));
                    //Debug.Log("label value retrieved  = " + subChild.Attributes.GetNamedItem("Label").Value);
                    ExerciseArea.Add(lineVertices);
                }

                return ExerciseArea;
            }
        }


        //}
        //Debug.LogError("Config file not selected. Exercies Area wont be loaded");
        return null;

    }


    List<List<float>> LatLongToVertices(List<List<float>> LinesList)
    {
        List<List<float>> lineVerticesInXY = new List<List<float>>();
        foreach (List<float> line in LinesList)
        {
            lineVerticesInXY.Add(ConvertToXY(line));
        }
        return lineVerticesInXY;
    }



    private List<float> ConvertToXY(List<float> attributes)
    {
        List<float> convertedAttributes = new List<float>();

        double altitude = 1000;

        float[] start = MathCalculations.Convert(attributes[4], attributes[2], altitude); //lat long
        float startX4 = start[0];
        float startY2 = start[1];


        float[] end = MathCalculations.Convert(attributes[5], attributes[3], altitude); //lat long
        float endX5 = end[0];
        float endY3 = end[1];

        convertedAttributes.Add(attributes[0]);
        convertedAttributes.Add(attributes[1]);
        convertedAttributes.Add(startY2);
        convertedAttributes.Add(endY3);
        convertedAttributes.Add(startX4);
        convertedAttributes.Add(endX5);

        return convertedAttributes;
    }


    List<List<float>> LatLongToVerticesForCircles(List<List<float>> CirclesList)
    {
        List<List<float>> CircleVerticesInXY = new List<List<float>>();
        foreach (List<float> circle in CirclesList)
        {
            CircleVerticesInXY.Add(ConvertToXYForCircle(circle));
        }
        return CircleVerticesInXY;
    }


    private List<float> ConvertToXYForCircle(List<float> attributes)
    {
        List<float> convertedAttributes = new List<float>();

        double altitude = 1000;

        float[] p1 = MathCalculations.Convert(attributes[0], attributes[1], altitude); //lat long
        float latitude = p1[0];
        float longitude = p1[1];

        convertedAttributes.Add(latitude);
        convertedAttributes.Add(longitude);
        convertedAttributes.Add(attributes[2]);
        convertedAttributes.Add(attributes[3]);

        return convertedAttributes;
    }

    List<List<float>> LatLongToVerticesForBullsEye(List<List<float>> BullsEyeList)
    {
        List<List<float>> BullVerticesInXY = new List<List<float>>();
        foreach (List<float> bull in BullsEyeList)
        {
            BullVerticesInXY.Add(ConvertToXYForBullsEye(bull));
        }
        return BullVerticesInXY;
    }


    private List<float> ConvertToXYForBullsEye(List<float> attributes)
    {
        List<float> convertedAttributes = new List<float>();

        double altitude = 1000;

        float[] p1 = MathCalculations.Convert(attributes[0], attributes[1], altitude); //lat long
        float latitude = p1[0];
        float longitude = p1[1];

        convertedAttributes.Add(latitude);
        convertedAttributes.Add(longitude);
        convertedAttributes.Add(attributes[2]);


        return convertedAttributes;
    }




    int GetIndex(string name)
    {
        if (name == "LineColor")
        {
            return 0;
        }
        if (name == "LineWidth")
        {
            return 1;
        }
        if (name == "Longitude_Start")
        {
            return 2;
        }
        if (name == "Longitude_End")
        {
            return 3;
        }
        if (name == "Latitude_Start")
        {
            return 4;
        }
        if (name == "Latitude_End")
        {
            return 5;
        }
        return -1;

    }

    string ColorIntToHex(int colorValue)
    {
        return colorValue.ToString("X");
    }

}
