using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Logging : MonoBehaviour
{

    // Use this for initialization
    XmlWriter xmlWriter;
    public GameObject rowSelectedPrefab;
    GameObject airRowParrent;
    public InputField formationName;
    public GameObject ExtractorManager; 
    public TabControl tabControlA;
    public TabControl tabControlB;
    public InputField missionName;
    public InputField missionDescription;
    public Dropdown missionType;
    public Dropdown missionPhase;
    public Dropdown callSignDropDown;
    public Dropdown roleDropDown;
    public Dropdown colorDropDown;
    public Dropdown ECMSupportDropDown;
    public Dropdown CombatControllerDropDown;
    public Dropdown InstructorControllerDropDown;
    public GameObject aircraftRowPrefab;
    public GameObject headingPrefab;
    public Text acType;
    public Dropdown pilotName;
    public Text tailID;
    public Text missionDate;
    public Button addPilotButton;
    public GameObject status;
    private bool aircraftAdded;
    public InputField shortFormation;
    public Toggle SideAToggle;
    public Toggle SideBToggle;
    public GameObject _addpilotpannel;
    public bool resetProgress = false;
    string missionId = "ERROR_HANDLED", missionStartTime = "", missionEndTime = "";
    List<List<Formation>> SidesFormation;
    List<Formation> sideA;
    List<Formation> sideB;
    Formation currentFormation;
    TabControl.TabData currentTab;
    GameObject SustainableGameObject;


    void Start()
    {
        SustainableGameObject = GameObject.Find("SustainableGameObject");
        sideA = new List<Formation>();
        sideB = new List<Formation>();
        if (SustainableGameObject)
        {
            PopulateFromEdit();
            Destroy(SustainableGameObject);
        }
        else
        {

            aircraftAdded = false;
            if (resetProgress)
            {
                PlayerPrefs.SetInt("TotalMissions", 0);
            }

            missionId = GenerateMissionID();

            PlayerPrefs.SetString("CurrentMissionID", missionId);
            
            CreateMissionFolder();

            SidesFormation = new List<List<Formation>>();
            OnAddNewMissionClick();
        }

        //OnStartAddFormation();
    }

    //public void OnStartAddFormation()
    //{
    //    Formation newFormation = new Formation();

    //    newFormation.CallSign = callSignDropDown.options[callSignDropDown.value].text;
    //    newFormation.Color = colorDropDown.options[colorDropDown.value].text;
    //    newFormation.Short = shortFormation.text;
    //    currentFormation = newFormation;

    //    if (SideAToggle.isOn)
    //    {
    //        sideA.Add(currentFormation);
    //        tabControlA.AddTab(currentFormation.CallSign);
    //        currentTab = tabControlA.tabs[tabControlA.tabs.Count - 1];
    //    }
    //    else if (SideBToggle.isOn)
    //    {
    //        sideB.Add(currentFormation);
    //        tabControlB.AddTab(currentFormation.CallSign);
    //        currentTab = tabControlB.tabs[tabControlB.tabs.Count - 1];
    //    }

    //        ActivatePilotButton();
    //}

    void PopulateFromEdit()
    {
        Debug.Log("Populate From Edit");
        bool hasList = SustainableGameObject.GetComponent<ListCarrier>().isContainingList();
        if (!hasList)
        {
            Debug.LogError("List not populated");
            return;
        }
        SidesFormation = SustainableGameObject.GetComponent<ListCarrier>().GetFormation();

        List<string> missionData = SustainableGameObject.GetComponent<ListCarrier>().GetMission();
        if (missionData == null)
        {
            Debug.LogError("List not populated");
            return;
        }

        PopulateMissionInformation(missionData);
        PopulateFormationInformation(SidesFormation);
        
    }

    private void PopulateMissionInformation(List<string> missionData)
    {
        missionId = missionData[0];

        
        missionDate.text = missionData[1];
        missionName.text = missionData[2];

        int index = missionType.options.FindIndex((v) => { return v.text.Equals(missionData[3]); });
        missionType.value = index;

        index = missionType.options.FindIndex((v) => { return v.text.Equals(missionData[3]); });
        missionPhase.value = index;

        missionDescription.text = missionData[5];
    }

    private void PopulateFormationInformation(List<List<Formation>> mSidesFormation)
    {
        string[] side = { "SideA", "SideB" };
        int i = 0;
        foreach (List<Formation> formations in mSidesFormation)
        {

            Debug.Log(side[i]);
            foreach (Formation formationInSide in formations)
            {
                if (side[i].Equals("SideA"))
                {
                   //tabControlA.AddTab(formationInSide.CallSign);
                    currentTab = tabControlA.tabs[tabControlA.tabs.Count - 1];
                }
                else
                {
                   // tabControlB.AddTab(formationInSide.CallSign);
                    currentTab = tabControlB.tabs[tabControlB.tabs.Count - 1];
                }

                Debug.Log(formationInSide.CallSign);
                foreach (Aircraft air in formationInSide.airCrafts)
                {
                   // Debug.Log(air.TailID);
                    SpawnAircraftRow(air.ACType, air.PilotName, air.TailID, air.StartTime, air.EndTime, air.CallSign, air.ShortN);
                    aircraftAdded = true;
                }
                ActivatePilotButton();
            }
            i = (i + 1) % 2;
        }
        Debug.Log("End Display");
    }

    private void CreateMissionFolder()
    {
        //string path = Application.dataPath + "/Resources/MissionData/" + missionId;
        string path = Application.streamingAssetsPath + "/MissionData/" + missionId;

        Directory.CreateDirectory(path);
    }

    private string GenerateMissionID()
    {
        string currentDate =  System.DateTime.Now.ToString("yyyyMMdd");

        string currentTime = System.DateTime.Now.ToString("hhmm");
        
        string userName = System.Environment.UserName;
        int TotalMissionCount =  PlayerPrefs.GetInt("TotalMissions") ;
        
        StringBuilder stringMaker = new StringBuilder();
     
        stringMaker.Append(userName);
        stringMaker.Append(currentDate);
        stringMaker.Append(currentTime);
        stringMaker.Append(TotalMissionCount);
        return stringMaker.ToString();
    }

    private void Update()
    {

      //  DisplayAll();

    }

    public void BeginXMLOperation()
    {
        //Create XML Document
       try
       {
            if(!aircraftAdded)
            {
                throw new System.Exception("No aircrafts added : Mission empty");
            }
            CreateXML();
            xmlWriter.WriteStartDocument();

            //Add mission tag
            AddMission();

            //Start tag for sides
            xmlWriter.WriteStartElement("Sides");


            string[] side = { "SideA", "SideB" };
            int i = 0;
            foreach (List<Formation> formations in SidesFormation)
            {
                
                xmlWriter.WriteStartElement("Side");
                xmlWriter.WriteAttributeString("Name", side[i]);
                xmlWriter.WriteStartElement("Formations");
                foreach (Formation formationInSide in formations)
                {
                    
                    AddFormation(formationInSide.Color,formationInSide.CallSign,formationInSide.Short);
               
                    xmlWriter.WriteStartElement("Aircrafts");
                    foreach (Aircraft air in formationInSide.airCrafts)
                    {
                        AddAircrafts(air.ACType,air.TailID,air.PilotName,air.EndTime,air.StartTime,air.TaxiData);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                }
                i = (i + 1) % 2;
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            EndXMLOperation();

            int currentGrandTotal = PlayerPrefs.GetInt("TotalMissions");
            PlayerPrefs.SetInt("TotalMissions", currentGrandTotal + 1);
            DisplaySuccess(true);
        }
        catch(System.Exception e)
        {
            DisplaySuccess(false,e);
        }

    }

    void DisplaySuccess(bool isSuccessful,System.Exception exception = null)
    {
        
        status.SetActive(true);

        if(isSuccessful)
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
      
        foreach(Transform child in status.transform)
        {
            child.gameObject.SetActive(false);
        }
        status.SetActive(false);
    }

    private void CreateXML()
    {
        //string path = Application.dataPath + "/Resources/MissionData/" + missionId;
        string path = Application.streamingAssetsPath + "/MissionData/" + missionId;

        string fileName = "structure.xml"; 
        xmlWriter = XmlWriter.Create(path + "/" + fileName);

    }

    private void EndXMLOperation()
    {
        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
    }

    private void AddMission( )
    {
      
        xmlWriter.WriteStartElement("Mission");
        xmlWriter.WriteAttributeString("MissionID", missionId);
        xmlWriter.WriteAttributeString("MissionName", missionName.text);
        xmlWriter.WriteAttributeString("Date", missionDate.text);
        xmlWriter.WriteAttributeString("Description", missionDescription.text);
        xmlWriter.WriteAttributeString("Type", missionType.options[missionType.value].text);
        xmlWriter.WriteAttributeString("Phase", missionPhase.options[missionPhase.value].text);
        xmlWriter.WriteAttributeString("StartTime", missionStartTime);
        xmlWriter.WriteAttributeString("EndTime", missionEndTime);
    }

    void AddFormation( string color, string callSign, string shortName)
    {

        xmlWriter.WriteStartElement("Formation");
        xmlWriter.WriteAttributeString("Color", color);
        xmlWriter.WriteAttributeString("CallSign", callSign);
        xmlWriter.WriteAttributeString("Short", shortName);
    }

    void AddAircrafts( string acType,string tailId,string pilotName, string endTime, string startTime,string taxiData)
    {
        xmlWriter.WriteStartElement("Aircraft");
        xmlWriter.WriteAttributeString("ACType", acType);
        xmlWriter.WriteAttributeString("TailId", tailId);
        xmlWriter.WriteAttributeString("PilotName", pilotName);
        xmlWriter.WriteAttributeString("EndTime", endTime);
        xmlWriter.WriteAttributeString("StartTime", startTime);
        xmlWriter.WriteAttributeString("TaxiData", taxiData);
    }

    public void OnAddNewMissionClick()
    {
       
        
        SidesFormation.Add(sideA);
        SidesFormation.Add(sideB);
        
    }

    public void ChangeCurrentFormation()
    {

        string name = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text ;
        string parentName = GetSuperParentName(EventSystem.current.currentSelectedGameObject.transform);
        if(parentName.Contains("ControlA"))
        {
            TabControl.TabData tabDataA = tabControlA.FindTab(name);
            if (tabDataA != null)
            {
                string formationCallsign = tabDataA.tabTitle;
                foreach (Formation formation in sideA)
                {
                    if(formation.CallSign.Equals(formationCallsign))
                    {
                        currentFormation = formation;
                    }
                }
                
            }
        }
        else if(parentName.Contains("ControlB"))
        {
            TabControl.TabData tabDataB = tabControlB.FindTab(name);
            if (tabControlB != null)
            {
                string formationCallsign = tabDataB.tabTitle;
                foreach (Formation formation in sideB)
                {
                    if (formation.CallSign.Equals(formationCallsign))
                    {
                        currentFormation = formation;
                    }
                }
                
            }
        }
    }

    private string GetSuperParentName(Transform tabTransform)
    {
        return  tabTransform.parent.parent.parent.parent.parent.name;
    }

    public void OnAddFormationClick()
    {
        
        Formation newFormation = new Formation();

        // newFormation.CallSign = callSignDropDown.options[callSignDropDown.value].text;
        newFormation.formationName = formationName.text;
        newFormation.Color = colorDropDown.options[colorDropDown.value].text;
        newFormation.Short = shortFormation.text;
        currentFormation = newFormation;
        
        if(SideAToggle.isOn)
        {
            sideA.Add(currentFormation);
            tabControlA.AddTab(currentFormation.formationName); //Replaced by formation name
            currentTab = tabControlA.tabs[tabControlA.tabs.Count - 1];
            
        }

        if(SideBToggle.isOn)
        {
            sideB.Add(currentFormation);
            tabControlB.AddTab(currentFormation.formationName); //Replaced by formation name
            currentTab = tabControlB.tabs[tabControlB.tabs.Count - 1];
        }
       // GameObject headings = Instantiate(headingPrefab, currentTab.tabContent.GetChild(0).GetChild(0).GetChild(0));
        ActivatePilotButton();
        
    }

    public void OnBackButtonClick()
    {
        //string path = Application.dataPath + "/Resources/MissionData/" + missionId;
        string path = Application.streamingAssetsPath + "/MissionData/" + missionId;

        string fileName = "structure.xml";
        if(File.Exists(path + "/" + fileName))
        {
            Debug.Log("File is created");
        }
        else
        {
            Debug.Log("No file created");
            Directory.Delete(path,true);

        }
    }

    public void OnApplicationQuit()
    {
        OnBackButtonClick();
    }

    public void ActivatePilotButton()
    {
        addPilotButton.interactable = true;
    }

    private void SpawnAircraftRow(string airType, string mpilotName, string mtailID, string mstartTime, string mendTime, string callSign, string ShortName)
    {
        GameObject airRow = Instantiate(aircraftRowPrefab, currentTab.tabContent.GetChild(0).GetChild(0).GetChild(0).GetChild(1));
        airRow.transform.localPosition += Vector3.down * ((airRow.transform.GetSiblingIndex() - 1) * 27);

        airRow.transform.Find("AC Type").GetComponent<Text>().text = airType;
        airRow.transform.Find("PilotName").GetComponent<Text>().text = mpilotName;
        airRow.transform.Find("TailID").GetComponent<Text>().text = mtailID;
        airRow.transform.Find("StartTime").GetComponent<Text>().text = mstartTime;
        airRow.transform.Find("EndTime").GetComponent<Text>().text = mendTime;
        airRow.transform.Find("Call Sign").GetComponent<Text>().text = callSign;
        airRow.transform.Find("Short").GetComponent<Text>().text = ShortName;

        resetLeader();
        setLeader();
    }

    public void setLeader()
    {
        airRowParrent = GameObject.Find("AirRowPrefabs");
        Transform firstRow = airRowParrent.transform.GetChild(0);
        Text textComponent = firstRow.Find("Call Sign").GetComponent<Text>();
        textComponent.text = textComponent.text + "-L";

    }
    public void resetLeader()
    {
        airRowParrent = GameObject.Find("AirRowPrefabs");
        foreach (Transform child in airRowParrent.transform)
        {
            child.Find("Call Sign").GetComponent<Text>().text.Replace("-L", "") ;
            Text textComponent = child.Find("Call Sign").GetComponent<Text>();
            textComponent.text = textComponent.text.Replace("-L","");
            // child.gameObject.GetComponent<Image>().color = Color.white;

        }
    }

    //ROW SWAP FUNCTIONS #Sabbar

    public void MoveUp()
    {
        if (rowSelectedPrefab == null)
        {
            return;
        }
        GameObject rowParent = rowSelectedPrefab.transform.parent.gameObject;
        int totalChildCount = rowSelectedPrefab.transform.parent.transform.childCount;
        int selectedIndex = rowSelectedPrefab.transform.GetSiblingIndex();
        int previousIndex = rowSelectedPrefab.transform.GetSiblingIndex() - 1;

        if (previousIndex >= 0) {
            Transform previousTransform = rowParent.transform.GetChild(previousIndex);
            Vector3 tempPosition = previousTransform.position;
            previousTransform.position = rowSelectedPrefab.transform.position;
            rowSelectedPrefab.transform.position = tempPosition;
            previousTransform.SetSiblingIndex(selectedIndex);
            rowSelectedPrefab.transform.SetSiblingIndex(previousIndex);

        }
        resetLeader();
        setLeader();
    }
    public void MoveDown()
    {
        if (rowSelectedPrefab == null)
        {
            return;
        }
        GameObject rowParent = rowSelectedPrefab.transform.parent.gameObject; 
        int totalChildCount = rowSelectedPrefab.transform.parent.transform.childCount; //2
        int selectedIndex = rowSelectedPrefab.transform.GetSiblingIndex(); //0
        int nextIndex = rowSelectedPrefab.transform.GetSiblingIndex() + 1; //1

        if (nextIndex < totalChildCount)
        {
            Transform nextTransform = rowParent.transform.GetChild(nextIndex);
            Vector3 tempPosition = nextTransform.position;
            nextTransform.position = rowSelectedPrefab.transform.position;
            rowSelectedPrefab.transform.position = tempPosition;
            nextTransform.SetSiblingIndex(selectedIndex);
            rowSelectedPrefab.transform.SetSiblingIndex(nextIndex);

        }
        resetLeader();
        setLeader();
    }

    public void clearSelection()
    {
        airRowParrent = GameObject.Find("AirRowPrefabs");
        foreach (Transform child in airRowParrent.transform)
        {
            child.gameObject.GetComponent<Image>().color = Color.white;

        }
    }

    public void OnAddAircraftClick()
    {
        ProcessExtractor refScript = ExtractorManager.GetComponent<ProcessExtractor>();

        if (currentFormation != null && refScript.shouldAddNewRow())
        {
            Aircraft air = new Aircraft();
            air.ACType = acType.text;
            air.EndTime = "";
            air.StartTime = "";
            air.TailID = tailID.text;
            air.PilotName = pilotName.options[pilotName.value].text;
            air.TaxiData = "";
            air.CallSign = callSignDropDown.options[callSignDropDown.value].text;
            air.ShortN = shortFormation.text;
            
            currentFormation.airCrafts.Add(air);

            string Stime = ExtractorManager.GetComponent<ProcessExtractor>().getStartTime();
            string Etime = ExtractorManager.GetComponent<ProcessExtractor>().getEndTime();

            SpawnAircraftRow(acType.text, pilotName.options[pilotName.value].text, tailID.text, Stime, Etime, callSignDropDown.options[callSignDropDown.value].text, shortFormation.text);

            aircraftAdded = true;
            _addpilotpannel.SetActive(false);

        }

    }

    public void DisplayAll()
    {
        
       // string []side = { "SideA", "SideB" };
        int i = 0;
        foreach(List<Formation> formations in SidesFormation)
        {
            
            foreach(Formation formationInSide in formations)
            {
                
                foreach(Aircraft air in formationInSide.airCrafts)
                {
                    Debug.Log(air.TailID);
                }
            }
            i= (i + 1) % 2;
        }
        
    }

}

public class Aircraft
{
    public string ACType = "",
           TailID = "",
           PilotName = "",
           EndTime = "",
           StartTime = "",
           TaxiData = "",
           CallSign = "",
           ShortN = "";

};

public class Formation
{
    public List<Aircraft> airCrafts = new List<Aircraft>();
    public string Color = "", CallSign = "", Short = "", formationName="";
};

