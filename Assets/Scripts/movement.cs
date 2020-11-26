using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;


public class movement : NetworkBehaviour
{
    ////////////////////////////////////////
    /// Launch Missile through script for testing purposes
    ////////////////////////////////////////
    ///
    public bool toggleMissileLaunch = false;
    //////////////////////////////////////// 

    public MissileManager _missileManager;
    public string hr;
    public string min;
    public string sec;
    string updatedStartTime;
    string textfileName;
    private string textFile;                      //type changed from TextAsset to string for testing

    private Vector3 targetPosition, startPosition;
    private Quaternion targetRotation, startRotation;
    [SyncVar]
    public string transformName;
    [SyncVar]
    public Vector3 initialNetworkPosition;
    [SyncVar]
    public GameObject childObject;
    string Name;
    string tailID;
    string color;
    string shortCall;
    string localtime;
    string starttime;
    string endtime;
    // bool taxidata; 
    //public float maxheight;
    public GameObject planeInfo;
    LineRenderer anchor;
    public GameObject callSignObj;
    MissionEvents events;
    //public Camera cam;
    //public GameObject time;
    bool start = false;
    //public GameObject slider;
    bool startReal = false;
    private Vector3 initialPosition;
    float timeToReachTarget;
    private IEnumerator coroutine;
    bool flag = false;
    float deltat, x, y, z, pitch, roll, heading; //t1 is initial time
    double t1 = 0, t2 = 0, timeDeltaConstant = 0;
    List<List<float>> alldata = new List<List<float>>();
    float scale = 1.0f;
    float timerTime = 0f;
    float minSampleRate = 0.2f;
    float timeRatio = 0;
    float speed = 0f;
    float altitude;
    public bool loadFromFile = true;
    public bool saveToFile = false;
    string missionID;
    int offset;
    bool changed = false;
    bool selected = false;
    bool playdirection = true; //forward
    bool pause = false;
    string type;
    private TrailRenderer trailRend;
    int counterAllData;
    List<float> dashboardData = new List<float>();

    [SerializeField]
    public string callsign;
    //string textFilex = "Local time is: 7:17:28, 20-9-20015\nTime Latitude    Longitude BaroAlt Roll Pitch   Heading MachNo  CAS G   AOA TrueAirSpeed    IAS AGL BRM Radar Range\n\n8247.44	38.572398	106.058517	5046.6	0.94	11.24	29.88	0.54	   0	1.14	0.13	 181	 157	 0.0	0	999.6	\n8247.64	38.572742	106.058689	5059.9	0.90	11.35	29.90	0.54	   0	1.19	0.09	 181	 157	 0.0	0	999.6	\n8247.84	38.573085	106.058689	5076.6	0.83	11.44	29.93	0.54	   0	1.21	0.05	 181	 157	 0.0	0	999.6	\n8248.04	38.573428	106.058861	5089.9	0.74	11.54	29.95	0.54	   0	1.19	0.03	 181	 157	 0.0	0	999.6	\n8248.24	38.573600	106.059032	5106.6	0.67	11.61	29.97	0.54	   0	1.19	0.32	 181	 157	 0.0	0	999.6	\n8248.44	38.573943	106.059204	5119.9	0.65	11.66	29.99	0.54	   0	1.21	0.28	 181	 157	 0.0	0	999.6	";

    // Use this for initialization

    //private void OnGUI()
    //{

    //	var point = Camera.main.WorldToScreenPoint (transform.position);
    //	var rect = new Rect (point.x,Screen.height - point.y, 200, 100);
    //	GUI.Label (rect,"abcd");



    //}
    public void setRenderer(LineRenderer LR)
    {
        anchor = LR;
    }


    Thread childThread;


    void Awake()
    {

        InitializeExtract();
        ExtractData();
    }


    void Reparent()
    {
        Transform parent = GameObject.Find("Terrain").transform.Find("Zoomables").Find("Aircrafts");
        transform.SetParent(parent);

    }

    void Start()
    {
        //Debug.Log("Start is called");
        Reparent();
        trailRend = GetComponent<TrailRenderer>();
        transform.name = transformName;
        transform.position = initialNetworkPosition;

        setInitialpos();
        ClearTrail();
        timeRatio = Time.timeScale / Time.fixedDeltaTime;

        //Always check for null before use
        //Putting this here will ensure missile manager is independant of caller
        SetMissileManager();

    }

    //set missile manager from Game Object or try and find
    [Server]
    public void SetMissileManager(GameObject missileManagerGO = null)
    {
        if (!_missileManager)
        {
            if (!missileManagerGO)
                _missileManager = GameObject.Find("MissileManager").GetComponent<MissileManager>();
            else
                _missileManager = missileManagerGO.GetComponent<MissileManager>();
        }
    }


    public void SetSave(bool _save)
    {
        saveToFile = _save;
    }

    public void SetMissionID(string mmissionId)
    {
        missionID = mmissionId;
    }

    public void SetLoad(bool _load)
    {
        loadFromFile = _load;
    }

    [Server]
    void InitializeExtract()
    {
        counterAllData = 0;
    }

    public void SetMissionEvents(MissionEvents m)
    {
        events = m;
    }
    //for streamingasset testting

    //public void SetTextFile(TextAsset textAsset)
    //{
    //    //Debug.Log("Reahcing");
    //    textFile = textAsset;
    //}

    public void SetTextFile(string textAsset)
    {
        //Debug.Log("Reahcing");
        textFile = textAsset;
    }
    public string[] GetData()
    {
        //Time Latitude    Longitude BaroAlt Roll Pitch   Heading MachNo  CAS G   AOA TrueAirSpeed    IAS AGL BRM Radar Range
        // "A/C Type", "Heading", "Altitude", "CAS", "TAS", "IAS", "Mach No.", "G", "AOA", "Latitude", "Longitude", "AGL", "Pitch"


        List<string> temp = new List<string>();

        temp.Add(callsign);
        temp.Add(type);

        //6, 3, 8, 11, 12, 7, 9, 10, 1, 2, 13, 5 
        int[] lst = { 6, 3, 8, 11, 12, 7, 9, 10, 1, 2, 13, 5 };

        if (dashboardData.Count == 15)
        {
            foreach (int x in lst)
            {
                temp.Add(dashboardData[x].ToString());
            }
        }
        return temp.ToArray();
    }

    [Server]
    public void ExtractData()
    {

        //type = textFile.name.Split('_')[0];
        //callsign = textFile.name.Split('_')[2];
        //Name = textFile.Split('_')[1];
        Name = (Path.GetFileName(textFile));

        string text = textFile;
        //textFile.text
        string[] data = text.Split('\n').Skip(3).ToArray();

        foreach (var item in data)
        {
            Debug.Log("All DATA ALTER ====" + item.ToString());

        }
        alldata.Clear();

        bool IsLoaded = false;
        if (loadFromFile)
        {
            IsLoaded = Load(Name);  //Load(textFile.name);

        }

        if (!IsLoaded)
        {
            foreach (var item in data)
            {
                if (!(item == ""))
                {
                    string[] datapoints = item.Split('\t');
                    List<float> rdata = new List<float>();
                    int maxFields = 0;
                    foreach (var datapoint in datapoints)
                    {

                        float num1;
                        bool res = float.TryParse(datapoint, out num1);
                        if (res)
                        {
                            rdata.Add(num1);
                            maxFields++;

                            if (maxFields >= 15)
                            {
                                break;
                            }
                        }
                    }


                    if (DateTimeOffset.Parse(MathCalculations.ConvertTime(rdata[0])) >= DateTimeOffset.Parse(MathCalculations.missionStart))
                    {
                        //    //Data Extraction
                        if (type == "AGSTA") //highest speed is 160 for AGSTA
                        {
                            if (rdata[11] >= 20)
                            {
                                alldata.Add(rdata);
                            }
                        }
                        else
                        {
                            if (rdata[11] >= 100)
                            {
                                alldata.Add(rdata);
                            }
                        }
                    }
                }
            }

        }
    }

    public void Save(string fname)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + fname + ".dat";
        Debug.Log(path);
        FileStream file = File.Open(path, FileMode.Create);

        PlayerData data = new PlayerData();
        data.allData = alldata;


        bf.Serialize(file, data);
        file.Close();
        Debug.Log("File saved: " + path);
    }

    public bool Load(string fname)
    {
        //string path = Application.persistentDataPath + "/" + fname + ".dat";

        //string path =  Application.dataPath + "/Resources/MissionData/"+ missionID+ "/PlaneData/" + fname + ".dat";
        string path = Application.dataPath + "/StreamingAssets/MissionData/" + missionID + "/PlaneData/" + fname + ".dat";

        //  Debug.Log("FNAME TEST FOR PATH /n" + path);

        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);

            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            alldata = data.allData;


            tailID = data.TailId;
            type = data.TypeAC;
            callsign = data.CallSign;
            shortCall = data.CallShort;
            Name = data.Pilotname;
            color = data.Color;
            Debug.Log("Color Form Pallet" + data.Color.ToString());

            //taxidata = data.taxiData;
            //  Debug.Log(callsign);
            // Debug.Log(color);
            if (alldata.Count == 0)
            {
                Debug.Log("File empty: " + path);
                return false;
            }
            Debug.Log("File loaded: " + path);
            return true;
        }
        Debug.Log("File not existant: " + path);
        return false;
    }

    public string getCallSign()
    {
        return callsign;
    }

    public string getShortCallSign()
    {

        return shortCall;
    }

    public string getColor()
    {
        return color;
    }
    public void setcallsign(GameObject T)
    {
        callSignObj = T;
    }

    public bool GetSelected()
    {
        return selected;
    }

    public void ClearTrail()
    {
        trailRend.Clear();
    }

    public bool getPaused()
    {
        return pause;
    }

    public void setPaused(bool val)
    {
        pause = val;
    }

    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }

    public void FlagOn()
    {
        flag = true;
    }

    public void FlagOff()
    {
        flag = false;
    }
    public void changeSelected()
    {
        selected = !selected;
    }
    public void Display()
    {
        if (!selected)
        {
            planeInfo.SetActive(true);
            selected = true;
        }
        else
        {
            planeInfo.SetActive(false);
            selected = false;
        }
    }
    public void ResetObject()
    {
        gameObject.SetActive(false);
        callSignObj.SetActive(false);

        //gameObject.SetActive(false);
        startReal = false;
        flag = false;
        InitializeExtract();
        setInitialpos();
        ClearTrail();
        timeRatio = Time.timeScale / Time.fixedDeltaTime;

    }

    public static bool AlmostEqual(Vector3 v1, Vector3 v2, float precision)
    {
        bool equal = true;

        if (Mathf.Abs(v1.x - v2.x) > precision) equal = false;
        if (Mathf.Abs(v1.y - v2.y) > precision) equal = false;
        if (Mathf.Abs(v1.z - v2.z) > precision) equal = false;

        return equal;
    }

    public static bool CheckR(Quaternion v1, Quaternion v2)
    {

        return Quaternion.Angle(v1, v2) < 2;
    }
    public string GetStartTime()
    {
        return starttime;
    }

    public string GetEndTime()
    {
        return endtime;
    }

    public DateTime GetLocalTime()
    {
        return DateTime.Parse(localtime);
    }

    public void StartReal(bool start)
    {
        // transform.GetChild(1).gameObject.SetActive(selected);
        if (start)//play forward
        {
            startReal = true;
            gameObject.SetActive(true);

        }
        else//play reverse
        {
            startReal = false;
            gameObject.SetActive(false);
            //     callSignObj.SetActive(false);
        }

    }
    public void movementDirection(bool direction)
    {
        playdirection = direction;
    }
    [Server]
    public void setInitialpos()
    {

        altitude = alldata[0][3];
        float[] xzy = MathCalculations.Convert(alldata[0][1], alldata[0][2], alldata[0][3]); //lat long

        //MathCalculations.DistanceToLatlong(new Vector2(xzy[0], xzy[1]));
        x = xzy[0];
        z = xzy[1];
        //maxheight = y;
        y = xzy[2];
        targetPosition = new Vector3(x, y, z);
        transform.localPosition = targetPosition;

        //This is correct and verified
        roll = alldata[0][4];
        pitch = alldata[0][5];
        heading = alldata[0][6];
        speed = alldata[0][11];
        targetRotation = Quaternion.Euler(pitch, heading, -roll); //roll and heading are swapped 
        transform.localRotation = targetRotation;

        startPosition = targetPosition = gameObject.transform.localPosition;
        startRotation = targetRotation = transform.localRotation;
        initialPosition = transform.position;

        t1 = alldata[0][0];

        //Debug.Log("Time T1"+ t1);
        //Add Time Offset to start time
        //int offset = (type == "F7") ? 8 : 0;
        //		hr = GameObject.Find("StartTimeManager").GetComponent<EditStartTime>().hrs.ToString();
        //		min = GameObject.Find ("StartTimeManager").GetComponent<EditStartTime> ().mins.ToString();
        //		sec = GameObject.Find ("StartTimeManager").GetComponent<EditStartTime> ().secs.ToString();
        //
        //		updatedStartTime = hr + ":" + min + ":" + sec;
        //Debug.Log ("Updated Time From UI From AddMission Scene = " + updatedStartTime);

        localtime = MathCalculations.ConvertTime(t1, offset);

        //        hr = PlayerPrefs.GetFloat("StartHour").ToString();
        //		min = PlayerPrefs.GetFloat("StartMinute").ToString();
        //		sec = PlayerPrefs.GetFloat("StartSecond").ToString();
        //UnityEngine.Debug.Log("list values" + TailIDElements);

        //starttime = updatedStartTime;
        starttime = localtime;
        //Debug.Log("Starttime after mathcalculation = "+ starttime);

        endtime = MathCalculations.ConvertTime(alldata[alldata.Count - 1][0], offset);

        //dashboardData = alldata[counterAllData];
        counterAllData = 1;
        //time.GetComponent<TextMesh>().text = "Local Time:\n" + localtime; 
        planeInfo.GetComponent<TextMesh>().text = "Callsign: " + callsign + "\nTailID: " + tailID + "\nAltitude: " + altitude.ToString() + "ft\nHeading: " + heading.ToString() + "\nSpeed: " + speed.ToString() + "kn";

    }


    public void ScalePrefab(int i)
    {
        if (i == 0)
        {
            scale += 0.2f;
            if (scale > 1.4f)
                scale = 1.4f;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            scale -= 0.2f;
            if (scale < 0.2f)
                scale = 0.2f;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void SpeedPrefab(int i)
    {
        if (i == 0)
        {
            Time.timeScale += 1f;
            changed = true;

        }
        else
        {
            Time.timeScale -= 1f;
            changed = true;
        }
        if (changed)
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale, 1, timeRatio * minSampleRate);
            Time.fixedDeltaTime = Time.timeScale / timeRatio;
            changed = false;
        }
    }


    public int GetCounterAllData()
    {
        return counterAllData;
    }

    public int GetAllDataLength()
    {
        return alldata.Count;
    }
    //public void SetCounterAllData(int cdata)
    //{
    //    counterAllData = cdata;
    //}
    public void SetOffsetTime(int offsetTime)
    {
        offset = offsetTime;
    }
    public int GetOffsetTime()
    {
        return offset;
    }

    public void SetCounterAllDataPercent(float inp)
    {
        //counterAllData = GetCounterAllDataFromTime(inp);

        counterAllData = Mathf.CeilToInt((alldata.Count - 1) * inp) - offset;

        counterAllData = Mathf.Clamp(counterAllData, 0, alldata.Count);

        SetCurrentPosition(counterAllData);

    }

    public void SetCounterAllDataBySeconds(double seconds)
    {
        //0.2 - 1
        //seconds - seconds 
        counterAllData = Mathf.CeilToInt((float)seconds / 0.2f) - offset;

        counterAllData = Mathf.Clamp(counterAllData, 0, alldata.Count - 1);
        SetCurrentPosition(counterAllData);
    }



    public void SetCurrentPosition(int positionId)
    {
        altitude = alldata[positionId][3];
        float[] xzy = MathCalculations.Convert(alldata[positionId][1], alldata[positionId][2], alldata[positionId][3]); //lat long
        x = xzy[0];
        z = xzy[1];
        //maxheight = y;
        y = xzy[2];
        targetPosition = new Vector3(x, y, z);
        transform.localPosition = targetPosition;

        //This is correct and verified
        roll = alldata[positionId][4];
        pitch = alldata[positionId][5];
        heading = alldata[positionId][6];
        speed = alldata[positionId][11];
        targetRotation = Quaternion.Euler(pitch, heading, -roll); //roll and heading are swapped 
        transform.localRotation = targetRotation;

        startPosition = targetPosition = gameObject.transform.localPosition;
        startRotation = targetRotation = transform.localRotation;


        t1 = alldata[positionId][0];

        //Add Time Offset to start time
        int offset = (type == "F7") ? 8 : 0;

        localtime = MathCalculations.ConvertTime(t1, offset);




        //dashboardData = alldata[counterAllData];
        counterAllData = positionId + 1;
        //time.GetComponent<TextMesh>().text = "Local Time:\n" + localtime; 
        planeInfo.GetComponent<TextMesh>().text = "Callsign: " + callsign + "\nTailID: " + tailID + "\nAltitude: " + altitude.ToString() + "ft\nHeading: " + heading.ToString() + "\nSpeed: " + speed.ToString() + "kn";
    }
    public void PlaneInfoOff()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        selected = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {



        //Object Position and rotation Updated
        if (flag && startReal)
        {

            if (counterAllData < alldata.Count)
            {

                if (timerTime >= timeDeltaConstant)
                {

                    timerTime = 0f;
                    //Display();
                    planeInfo.GetComponent<TextMesh>().text = "Call Sign: " + callsign + "\nTailID: " + tailID + "\nAltitude: " + altitude.ToString() + "\nHeading: " + heading.ToString() + "\nSpeed: " + speed.ToString();
                    //time.GetComponent<TextMesh>().text = "Local Time:\n" + localtime;
                    List<float> data = alldata[counterAllData];
                    dashboardData = alldata[counterAllData - 1];

                    //alldata.RemoveAt(0);
                    if (playdirection)
                    {
                        counterAllData++;
                    }
                    else
                    {
                        if (counterAllData != 0)
                            counterAllData--;
                    }


                    t2 = data[0];
                    localtime = MathCalculations.ConvertTime(t2);
                    if (t2 > t1)
                        timeToReachTarget = (float)(t2 - t1);
                    else
                        timeToReachTarget = (float)(t1 - t2);
                    timeDeltaConstant = timeToReachTarget;
                    t1 = t2;
                    altitude = data[3];
                    float[] xzy = MathCalculations.Convert(data[1], data[2], data[3]);
                    x = xzy[0];
                    z = xzy[1];
                    y = xzy[2];

                    //This is correct and verified
                    roll = data[4];
                    pitch = data[5];
                    heading = data[6];
                    speed = data[11];
                    targetRotation = Quaternion.Euler(pitch, heading, -roll);
                    /*assuming plane is facing left, 
                    roll is Rotation on longitudinal axis 
                    pitch is Rotation on lateral axis
                    yaw is Rotation on vertical axis*/
                    if (data[14] != 0 || toggleMissileLaunch)
                    {
                        toggleMissileLaunch = false;
                        Debug.Log("Mission Event Happened");
                        int x = GetCounterAllData() + GetOffsetTime();
                        events.AddMissionEvent(localtime.ToString() + " " + callsign, x);
                        LaunchMissile();
                        //localtime.ToString() + " " + transform.name;
                    }


                    deltat = 0;
                    startPosition = gameObject.transform.localPosition;
                    startRotation = gameObject.transform.localRotation;
                    //We are dealing with 200 milliseconds
                    //timeToReachTarget = 0.2f;
                    targetPosition = new Vector3(x, y, z);

                    //Vector3.RotateTowards(transform.forward, targetPosition, 0.2f * Time.deltaTime, 0.0f);
                    // transform.rotat(targetPosition);
                    //transform.rotation = Quaternion.LookRotation(targetPosition);
                }
                //gameObject.transform.position += transform.forward * Time.deltaTime * 2;
                float varDeltaTime = Time.fixedDeltaTime;
                float ratio = varDeltaTime / timeToReachTarget;
                timerTime += varDeltaTime;
                gameObject.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, ratio);
                //callSignObj.transform.localPosition = gameObject.transform.localPosition;
                transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, ratio);
                // callSignObj.transform.localPosition = gameObject.transform.localPosition;
                timeToReachTarget -= varDeltaTime;
                anchor.SetPosition(0, gameObject.transform.GetChild(1).GetChild(1).position + (Vector3.down * 0.008f));
                anchor.SetPosition(1, gameObject.transform.GetChild(0).position);


                //Vector3 v = Camera.main.transform.position - transform.position;
                //v.x = v.z = 0.0f;
                //callSignObj.transform.LookAt(Camera.main.transform.position - v);
                //callSignObj.transform.Rotate(0, 180, 0);

            }
            else
            {
                dashboardData = alldata[counterAllData - 1];
            }

        }


    }

    /* Required changes tabish */
    public double GetCurrentLatitue()
    {
        List<float> temp = alldata[counterAllData - 1];
        return temp[1];
    }

    public double GetCurrentLongitude()
    {
        List<float> temp = alldata[counterAllData - 1];
        return temp[2];
    }

    public double GetCurrentAltitude()
    {
        List<float> temp = alldata[counterAllData - 1];
        return temp[3];
    }

    public double GetCurrentSpeed()
    {
        List<float> temp = alldata[counterAllData - 1];
        return temp[11];
    }

    public void ToggleLaunchMissile()
    {
        toggleMissileLaunch = true;
    }
    async void LaunchMissile()
    {
        await LaunchMissileAsync();
    }

    private async Task LaunchMissileAsync()
    {
        _missileManager.Launch(gameObject, type);
    }

    public void SetAndShowKillProbability(float probability)
    {

    }


    //private void OnMouseDown()
    //{
    //    Debug.Log("clocl");
    //    planeInfo.SetActive(!planeInfo.activeInHierarchy);
    //}

}

//[Serializable]
//class PlayerData
//{
//    public List<List<float>> allData;
//}

//[Serializable]
//class PlayerData
//{
//    public List<List<float>> allData;
//    public string TailId;
//    public string TypeAC;
//    public string CallSign;
//    public string CallShort;
//    public string Pilotname;
//    public string Color;
//    public bool taxiData;
//}