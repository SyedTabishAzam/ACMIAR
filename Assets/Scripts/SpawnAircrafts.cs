
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class SpawnAircrafts : NetworkBehaviour
{
    public LiveAcmiDataReciever.ACMIdata liveDataPacket;
    public Transform aircraftsParent;
    //for each file in folder datafile
    //create a gameobject prefab with file name 
    //assign the right model 
    //assign the text file 
    public GameObject gameManager;
    public GameObject prefab;
    public GameObject planeInfoPrefab;
    public GameObject CallsignDisplayPrefab;
    public GameObject missionevents;
    public GameObject _missileManagerGO;
    public GameObject[] DropDownReferences;

    [Range(0, 39)]
    public int AircraftsToSpawn;

    public bool SaveToFile = false;
    public bool LoadFromFile = true;
    public string currentMission;
    List<string> callsign;

    [Server]
    public void StartSim()
    {
        //CallsignDisplayPrefab.transform.localScale = new Vector3 (0.5f,1,1);
        string playePrefs = PlayerPrefs.GetString("Current_MissionID");
        if (playePrefs != "")
        {
            PlayerPrefs.SetString("Current_MissionID", "");
            currentMission = playePrefs;
        }

        //string[] data = Directory.GetFiles(Application.dataPath + "\\Resources\\MissionData\\" + currentMission + "\\RawData", "*.txt");
        string[] data = Directory.GetFiles(Application.streamingAssetsPath + "\\MissionData\\" + currentMission + "\\RawData", "*.txt");


        //var textFile = Resources.Load<TextAsset>("Text/textFile01");
        callsign = new List<string>();
        callsign.Add("None");

        int spawnedAircrafts = 0;
        int len = data.Length;
        for (int it = 0; it < len; it++)
        {
            string file = data[it];
            if (spawnedAircrafts >= AircraftsToSpawn)
            {
                break;
            }
            spawnedAircrafts++;

            string rname = Path.GetFileName(file);

            //Intantiate Aircraft Object from prefab
            GameObject aircraft = Instantiate(prefab, aircraftsParent) as GameObject;

            string transformName = rname.Split('.')[0];
            aircraft.transform.name = transformName;
            aircraft.gameObject.GetComponent<movement>().transformName = transformName;
            aircraft.gameObject.GetComponent<movement>().initialNetworkPosition = aircraft.transform.position;
            aircraft.gameObject.GetComponent<movement>().SetLoad(LoadFromFile);

            aircraft.gameObject.GetComponent<movement>().SetMissionID(currentMission);
            aircraft.gameObject.GetComponent<movement>().SetMissionEvents(missionevents.GetComponent<MissionEvents>());
            aircraft.gameObject.GetComponent<movement>().SetMissileManager(_missileManagerGO);
            //Load plane data file
            //TextAsset textFileAsset = Resources.Load<TextAsset>("MissionData\\" + currentMission + "\\RawData\\" + rname.Split('.')[0]) as TextAsset;

            string path = Application.streamingAssetsPath + "\\MissionData\\" + currentMission + "\\RawData\\" + rname.Split('.')[0];
            // Debug.Log("yeh kia hy" +rname.Split('.')[0]);
            // Debug.Log(" TEST PATH FOR DAT FILE" + path);
            // string FileData = File.ReadAllText(path);


            aircraft.gameObject.GetComponent<movement>().SetTextFile(path);


            //Instantiate Model from Prefab
            GameObject model = Resources.Load<GameObject>("Prefabs\\Aircrafts\\" + rname.Split('_')[0]) as GameObject;

            if (!model)
            {
                model = Resources.Load<GameObject>("Prefabs\\Aircrafts\\" + "F-7") as GameObject;
                Debug.Log("Model for : " + rname.Split('_')[0] + " file not found. Loading Dummy Model");
            }

            GameObject instance = Instantiate(model, aircraft.transform);
            instance.GetComponent<ModelNetwork>().parentObject = aircraft.transform;
            instance.GetComponent<ModelNetwork>().RpcReparent();
            instance.GetComponent<ModelNetwork>().initialNetworkPosition = instance.transform.position;
            instance.GetComponent<ModelNetwork>().initialNetworkRotation = instance.transform.rotation;
            NetworkServer.Spawn(instance);

            aircraft.gameObject.SetActive(true);



            //instantiate Plane Info 
            GameObject planeInfoObject = Instantiate(planeInfoPrefab, aircraft.transform) as GameObject;
            planeInfoObject.SetActive(false);
            aircraft.gameObject.GetComponent<movement>().planeInfo = planeInfoObject.GetComponentInChildren<Transform>().Find("Pivot").Find("ContentParent").Find("Label").gameObject;
            LineRenderer lineRenderer = planeInfoObject.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, planeInfoObject.transform.localPosition);
            lineRenderer.SetPosition(1, aircraft.transform.localPosition);
            //Debug.Log(aircraft.transform.position);
            aircraft.gameObject.GetComponent<movement>().setRenderer(lineRenderer);

            //aircraft.gameObject.GetComponent<Button>().onClick.AddListener(instance.GetComponent<CollisionDetection>().OnClicked);
            //aircraft.gameObject.GetComponent<Button>().onClick.AddListener(aircraft.gameObject.GetComponent<movement>().ToggleLaunchMissile);
            //aircraft info trigger 
            //Instantiate Call Sign Object 
            GameObject CallsignDisplayObject = Instantiate(CallsignDisplayPrefab, instance.transform) as GameObject;
            // CallsignDisplayObject.GetComponent<SpriteTextFactory>().RpcSpawnWithParams(instance.transform.name,CallsignDisplayObject.transform.position);
            // NetworkServer.Spawn(CallsignDisplayObject);

            //Problem
            string name = aircraft.gameObject.GetComponent<movement>().getCallSign();
            string shortname = aircraft.gameObject.GetComponent<movement>().getShortCallSign();


            CallsignDisplayObject.GetComponent<SpriteTextFactory>().ChangeText(shortname);
            CallsignDisplayObject.GetComponent<SpriteTextFactory>().DrawText();

            aircraft.gameObject.GetComponent<movement>().setcallsign(CallsignDisplayObject);
            aircraft.gameObject.GetComponent<movement>().setInitialpos();
            CallsignDisplayObject.transform.parent = aircraft.transform;
            //for dropdown menue
            callsign.Add(name);


           
            NetworkServer.Spawn(aircraft);
        }


        foreach (GameObject reference in DropDownReferences)
        {
            reference.GetComponent<Dropdown>().ClearOptions();
            reference.GetComponent<Dropdown>().AddOptions(callsign);
        }
        gameManager.GetComponent<TimeControl>().StartManual();
    }

}