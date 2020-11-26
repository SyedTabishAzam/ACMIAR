using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
public class ParseMe : MonoBehaviour {

   // public TextAsset ConfigFile;
    List<Formation> sideA;
    List<Formation> sideB;

    public Text displayHolderA;
    public Text displayHolderB;
  
    List<List<string>> allmissions;
    int currentMissionId;
    List<List<List<Formation>>> allformations;
    //Formation currentFormation;

    public Transform transformParent;
    public Text missionID;
    public Text TotalMission;
    public GameObject SustainableGameObject;
    int count = 1; 

    public GameObject rowPrefab;
    // Use this for initialization
    public void Start()
    {
        currentMissionId = 0;
         
        sideA= new List<Formation>();
        sideB = new List<Formation>();
        allmissions = new List<List<string>>();
        allformations = new List<List<List<Formation>>>();
         
        GetAllData();
        TotalMission.text = numMission().ToString();

    }

    private void Update()
    {
        //DisplayAll();
        
    }


    public void OnApplicationQuit()
    {
        PlayerPrefs.SetString("Current_MissionID", "");
    }

    public void OnBackBtnClick()
    {
        PlayerPrefs.SetString("Current_MissionID", "");
    }


    void GetAllData()
    {
        //string folderPath = Application.dataPath + "/Resources/MissionData";
       string folderPath = Application.streamingAssetsPath + "/MissionData";


        string[] files = Directory.GetDirectories(folderPath).SelectMany(Directory.GetFiles).Where(f => f.EndsWith(".xml")).ToArray();
      
        foreach (var filePath in files)
        {
            
            GetData(filePath);
            Populate();
        }
    }

    public int numMission()
    {
        return allmissions.Count;
    }


    public void OnRowClick(int index)
    {      
        missionID.text = allmissions[index][0];
        PlayerPrefs.SetString("Current_MissionID", missionID.text);
        
        Display(index);      
    }

    public void OnEditClick(int index)
    {
        SustainableGameObject.GetComponent<ListCarrier>().SaveList(allformations[index]);
        SustainableGameObject.GetComponent<ListCarrier>().SaveMission(allmissions[index]);
        PlayerPrefs.SetString("Current_MissionID", "");
        DontDestroyOnLoad(SustainableGameObject);
        SceneManager.LoadScene(1);
    }

    void GetData(string path)
    {
        //Read XML file and parse it for file location of textures (.jpg)
        
        XmlDocument configFile = new XmlDocument();
        configFile.Load(path);
        XmlNode xmlRootNode = configFile.DocumentElement;
        XmlNodeList xmlNodeList = xmlRootNode.ChildNodes;
        //XmlNodeList lines;
       
        List<string> mission = new List<string>();
        mission.Add(xmlRootNode.Attributes.GetNamedItem("MissionID").Value);
        mission.Add(xmlRootNode.Attributes.GetNamedItem("Date").Value);
        mission.Add(xmlRootNode.Attributes.GetNamedItem("MissionName").Value);
        mission.Add(xmlRootNode.Attributes.GetNamedItem("Type").Value);
        mission.Add(xmlRootNode.Attributes.GetNamedItem("Phase").Value);
        mission.Add(xmlRootNode.Attributes.GetNamedItem("Description").Value);


        allmissions.Add(mission);

        //For each child node in "ExerciseArea" tag
        List<List<Formation>> SidesFormation = new List<List<Formation>>();
        foreach (XmlNode side in xmlNodeList[0])
        {
             
            //This gives side names ( A and B)
            
            XmlNodeList formations = side.ChildNodes[0].ChildNodes;
            List<Formation> formationLst = new List<Formation>();
            
            foreach (XmlNode formation in formations)
            {
                //This gives formation callsigns (For each side)
                
                XmlNodeList aircrafts = formation.ChildNodes[0].ChildNodes;
                Formation currentFormation = new Formation();
                currentFormation.CallSign = formation.Attributes.GetNamedItem("CallSign").Value;
                currentFormation.Short  = formation.Attributes.GetNamedItem("Short").Value;
                currentFormation.Color = formation.Attributes.GetNamedItem("Color").Value;
                currentFormation.airCrafts = new List<Aircraft>();

                foreach(XmlNode airCraft in aircrafts)
                {
                    Aircraft currentAir = new Aircraft();
                    currentAir.PilotName = airCraft.Attributes.GetNamedItem("PilotName").Value;
                    currentAir.ACType = airCraft.Attributes.GetNamedItem("ACType").Value;
                    currentAir.TailID= airCraft.Attributes.GetNamedItem("TailId").Value;
                    currentAir.EndTime = airCraft.Attributes.GetNamedItem("EndTime").Value;
                    currentAir.StartTime = airCraft.Attributes.GetNamedItem("StartTime").Value;
                    currentAir.TaxiData = airCraft.Attributes.GetNamedItem("TaxiData").Value;
                    //This gives aircrafts pilot names for each formation in every side
                    currentFormation.airCrafts.Add(currentAir);
                }
                formationLst.Add(currentFormation);
                    
            }
            SidesFormation.Add(formationLst);               
        }


        allformations.Add(SidesFormation);
        //SidesFormation.Clear();
    }


    public void Display(int index)
    {
        
        
        string[] side = { "SideA", "SideB" };
        int i = 0;
        string strMaker = "";
        displayHolderA.text = strMaker;
        displayHolderB.text = strMaker;
        foreach (List<Formation> formations in allformations[index])
        {
            strMaker += "\tFormations\n";
            
            foreach (Formation formation in formations)
            {
                strMaker += "\t\t" + formation.CallSign + "\n";
                foreach (Aircraft air in formation.airCrafts)
                {
                    strMaker += "\t\t\t" + air.PilotName + "\t\t" + air.ACType + "\n";
                }

            }
            if (side[i] == "SideA")
            {
                displayHolderA.text = strMaker;
                strMaker = "";
            }
            else
            {
                displayHolderB.text = strMaker;
                strMaker = "";
            }
            i++;

        }
            
        //}
        //displayHolder.text += strMaker; 

    }

    public void Populate()
    {
        GameObject row = Instantiate(rowPrefab, transformParent);
        //rowPrefab.transform.position += Vector3.down * ((count * 57.3f));
        row.transform.Find("S.no").GetChild(0).GetComponent<Text>().text = count.ToString();
        row.transform.Find("MissionID").GetChild(0).GetComponent<Text>().text = allmissions[currentMissionId][0];
        row.transform.Find("Date").GetChild(0).GetComponent<Text>().text = allmissions[currentMissionId][1];
        row.transform.Find("Name").GetChild(0).GetComponent<Text>().text = allmissions[currentMissionId][2];
        row.transform.Find("Type").GetChild(0).GetComponent<Text>().text = allmissions[currentMissionId][3];
        row.transform.Find("Phase").GetChild(0).GetComponent<Text>().text = allmissions[currentMissionId][4];
        row.transform.Find("Description").GetChild(0).GetComponent<Text>().text = allmissions[currentMissionId][5];
        count++;
        currentMissionId++;
        // mission.Clear();
    }
}
