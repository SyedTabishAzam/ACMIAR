using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using SFB;
using System;


public class ProcessExtractor : MonoBehaviour
{

    public Text SuccessTextFormation;
    public List<string> TailIDElements = new List<string>();
    public InputField tailId;
    public Text typeAC;
    public Dropdown callsign;
    public InputField callShort;
    public string missionFolder;
    public Dropdown Pilotname;
    public Dropdown Color;
    public Toggle taxiData; 
    public Text Filename;
    public bool _debugExtractOnlyData = false;
    public bool _extractFolder = false;
    public string _debugRawDataPath = "";
    int counterAllData;
    int count = 0;
    string localtime;
    string starttime="";
    string endtime= "";
    string filename;
    double t1 = 0;
    int offset;
    InputField hr;
    InputField min;
    InputField sec;

    public GameObject dialogueBoxForEmptyFile;
    public GameObject dialogueBoxForRepeadtedTailId;
    public Button addAircraftButton;

    public GameObject hoursOptionSelector;
    public GameObject minutesOptionSelector;
    public GameObject secondsOptionSelector;


    ExtensionFilter[] ExtensionFilterForExArea = new[] { new ExtensionFilter("xml", "*") };

    List<List<float>> alldata = new List<List<float>>();

    void Start()
    {
        //if (_debugExtractOnlyData)
        //    ExtractData(_debugRawDataPath);
        if(_extractFolder)
        {
            string  path = Application.dataPath + "\\StreamingAssets\\MissionData\\" + _debugRawDataPath;
            UnityEngine.Debug.Log(path);

            List<string> m_DropOptions = new List<string> { "Option 1" };
            callsign.AddOptions(m_DropOptions);
            foreach (string file in System.IO.Directory.GetFiles(path, "*.txt"))
            {
                UnityEngine.Debug.Log(_debugRawDataPath + "\\" + Path.GetFileName(file));
                string typeAc = Path.GetFileName(file).Split('_')[0];
                string callsignS = Path.GetFileNameWithoutExtension(file).Split('_')[2];
                Filename.text = Path.GetFileNameWithoutExtension(file);
                callsign.options[0].text = callsignS;
                
                typeAC.text = typeAc; //hig
                ExtractData(_debugRawDataPath + "\\" + Path.GetFileName(file));
            }
        }
       
        hr = hoursOptionSelector.GetComponent<InputField>();
        min = minutesOptionSelector.GetComponent<InputField>();
        sec = secondsOptionSelector.GetComponent<InputField>();
        missionFolder = PlayerPrefs.GetString("CurrentMissionID");
       
    }
    
    public void stTime()
    {
        try
        {
            string startingTime = getStartTime();
            string[] st = startingTime.Split(':');

            hr.text = st[0];
            min.text = st[1];
            sec.text = st[2];
        }
        catch 
        {
        }

    }

    public string getStartTime()
    {
        return starttime; 
    }

    public string getEndTime()
    {
        return endtime;
    }

    public void LoadExerciseArea()
    {
       string[] exAreaPath = StandaloneFileBrowser.OpenFilePanel("Choose Exercise Area To Load", Application.dataPath, ExtensionFilterForExArea, false);
       PlayerPrefs.SetString("ExerciseAreaPathFromAddmissionScene", exAreaPath[0]);

    }

    public void OnBrowse()
    {
        string tailID = "";

        var extensions = new[] { new ExtensionFilter("Data", "ACM,GPS", "All Files", "*") };
       
        var  path0 = StandaloneFileBrowser.OpenFilePanel("Choose a File", Application.dataPath, extensions, false);

        //string[] filter = new string[4] { "Data", "ACM,GPS", "All Files", "*" };
        //string path = EditorUtility.OpenFilePanelWithFilters("Choose a file", Application.dataPath, filter);   

        string path = path0[0].ToString();

        var fileSizeInfo = new System.IO.FileInfo(path);

        if (fileSizeInfo.Length < 10)
        {
            dialogueBoxForEmptyFile.SetActive(true);
        }

        else
        {
            addAircraftButton.interactable = true;

            if (path.Equals("")) { return; }
            string[] name = path.Split('\\');
           // UnityEngine.Debug.Log("onbrowse name check " + name);

            filename = name[name.Length - 3] + "_" + name[name.Length - 2];
          //  UnityEngine.Debug.Log("onbrowse Filename check " + filename);

            typeAC.text = name[name.Length - 3];

            string folderName = Path.GetDirectoryName(path);
            string[] data = Directory.GetFiles(folderName, "log.txt");
          
            if (data.Length != 0)
            {
                using (var sr = new StreamReader(data[0]))
                {
                    for (int i = 1; i < 6; i++)
                    {
                        string line = sr.ReadLine();

                        if (line.Contains("ID"))
                            tailID = line.Split(' ')[line.Split(' ').Length - 1];
                        
                    }
                    tailId.text = tailID;
                }
                //tailID is from file
                //tailid is from ui
                //// UnityEngine.Debug.Log(tailID);
                //UnityEngine.Debug.Log("Tail ID from file" + tailID);
                //UnityEngine.Debug.Log("Tail ID from UI" + tailId);
                //tailId.text = tailID;

                //if (tailID == tailId.text)
                //{
                //    //EditorUtility.DisplayDialog("Same Tail Id Error", "Same Tail Id's not allowed assign another Tail Id.");
                //    EditorUtility.DisplayDialog("Same Tail Id Error",
                //"Same Tail Id's not allowed assign another Tail Id. ",
                //"Ok");
                //}
                //else
                //{
                //    tailId.text = tailID;
                //}
            }

           
            if (path.Contains("GPS"))
            {
                //UnityEngine.Debug.Log("GPS File");
                //if (!path.Contains("ACMI0"))
                //{
                string newFilePath = folderName + "\\" + "ACMI0.GPS";

               // UnityEngine.Debug.Log(newFilePath);
                if (!File.Exists(newFilePath))
                    File.Copy(path, newFilePath, true);
                ExtractTextFromGPS(folderName);
                // string stringmaker = "C:\\Users\\AR 2\\Documents\\datafileX\\" + filename + ".txt";


                //string rawDataDir = Application.persistentDataPath + "/MissionData/" + missionFolder + "/RawData";

                //string rawDataDir = Application.dataPath + "/Resources/MissionData/" + missionFolder + "/RawData";

                string rawDataDir = Application.streamingAssetsPath + "/MissionData/" + missionFolder + "/RawData";




                Directory.CreateDirectory(rawDataDir);
                string mpath = rawDataDir + "/" + filename + ".txt";
                if (!File.Exists(mpath))
                    File.Copy(folderName + "\\acmi0.txt", mpath, true);


                //}
            }

            else if (path.Contains("ACM"))
            {
                //UnityEngine.Debug.Log("ACM File");
                //if (!path.Contains("ACMI0"))
                //{
                string newFilePath = folderName + "\\" + "ACMI0.ACM";

               // UnityEngine.Debug.Log(newFilePath);
                if (!File.Exists(newFilePath))
                    File.Copy(path, newFilePath, true);
                ExtractTextFromACM(folderName);
                // string stringmaker = "C:\\Users\\AR 2\\Documents\\datafileX\\" + filename + ".txt";

                //string rawDataDir = Application.persistentDataPath + "/MissionData/" + missionFolder + "/RawData";
                //string rawDataDir = Application.dataPath + "/Resources/MissionData/" + missionFolder + "/RawData";
                string rawDataDir = Application.streamingAssetsPath + "/MissionData/" + missionFolder + "/RawData";


                Directory.CreateDirectory(rawDataDir);
                string mpath = rawDataDir + "/" + filename + ".txt";
                if (!File.Exists(mpath))
                    File.Copy(folderName + "\\acmi0.txt", mpath, true);

               
                //}

            }
            Filename.text = filename;
            count++;
            // Count.text = "Count: " + count.ToString();

        }

        ExtractData();
        stTime();


    }

    void ExtractTextFromGPS(string directory)
    {

        Process myProcess = new Process();
        myProcess.StartInfo.UseShellExecute = false;
        myProcess.StartInfo.WorkingDirectory = directory;
        myProcess.StartInfo.FileName = Application.dataPath +"/Extractors/" + "QStarz1000GPSFileExtractor.exe";


        myProcess.StartInfo.CreateNoWindow = true;

        myProcess.Start();
        myProcess.WaitForExit();
    }

    void ExtractTextFromACM(string directory)
    {

        Process myProcess = new Process();
        myProcess.StartInfo.UseShellExecute = false;
        myProcess.StartInfo.WorkingDirectory = directory;

        if (directory.Contains("JF-17"))
            myProcess.StartInfo.FileName = Application.dataPath + "/Extractors/Extracting parameters from JF_17_3.exe";
        else
            myProcess.StartInfo.FileName = Application.dataPath + "/Extractors/Universal_Extractor_Win2.exe";

        myProcess.StartInfo.CreateNoWindow = true;
        //For all GPS file containing directories

        myProcess.Start();
        myProcess.WaitForExit();
       

    }

    
    //void ExtractNames()
    //{

    //    string[] filePath = Directory.GetDirectories("C:\\Users\\AR 2\\Documents\\10-10-18");

    //    foreach (string file in filePath)
    //    {

    //        string rootname = Path.GetFileName(file);

    //        string[] data = Directory.GetFiles(file, "*.ACM", SearchOption.AllDirectories);
    //        string[] data1 = Directory.GetFiles(file, "*.GPS", SearchOption.AllDirectories);



    //        //Contains ACM FILES
    //        if (data.Length != 0)
    //        {

    //            foreach (string filePPath in data)
    //            {
    //                UnityEngine.Debug.Log("Reading file : " + filePPath.ToString());
    //                string aircraftName = Path.GetFileName(Path.GetDirectoryName(filePPath));
    //                string folderName = Path.GetDirectoryName(filePPath);

    //                string[] currentDirFiles = Directory.GetFiles(folderName, "*.ACM");
    //                if (!currentDirFiles[currentDirFiles.Length - 1].Equals(filePPath))
    //                {
    //                    UnityEngine.Debug.Log("Skipping file : " + filePPath.ToString());
    //                    UnityEngine.Debug.Log("Last file should be : " + currentDirFiles[currentDirFiles.Length - 1].ToString());
    //                    continue;
    //                }

    //                UnityEngine.Debug.Log(filePPath);
    //                UnityEngine.Debug.Log(folderName);
    //                if (!filePPath.Contains("ACMI0"))
    //                {
    //                    string newFilePath = folderName + "\\" + "ACMI0.ACM";

    //                    UnityEngine.Debug.Log(newFilePath);
    //                    if (!File.Exists(newFilePath))
    //                        File.Copy(filePPath, newFilePath, true);
    //                    ExtractTextFromACM(folderName);
    //                    string stringmaker = "C:\\Users\\AR 2\\Documents\\datafileX\\" + rootname + "_" + aircraftName + ".txt";

    //                    if (!File.Exists(stringmaker))
    //                        File.Copy(folderName + "\\acmi0.txt", stringmaker, true);
    //                }
    //            }
    //        }
    //        //Contains GPS FILES
    //        else if (data1.Length != 0)
    //        {
    //            foreach (string filePPath in data1)
    //            {
    //                string aircraftName = Path.GetFileName(Path.GetDirectoryName(filePPath));
    //                string folderName = Path.GetDirectoryName(filePPath);
    //                UnityEngine.Debug.Log(filePPath);
    //                UnityEngine.Debug.Log(folderName);
    //                if (!filePPath.Contains("ACMI0"))
    //                {
    //                    string newFilePath = folderName + "\\" + "ACMI0.GPS";

    //                    UnityEngine.Debug.Log(newFilePath);
    //                    if (!File.Exists(newFilePath))
    //                        File.Copy(filePPath, newFilePath, true);
    //                    ExtractTextFromGPS(folderName);
    //                    string stringmaker = "C:\\Users\\AR 2\\Documents\\datafileX\\" + rootname + "_" + aircraftName + ".txt";

    //                    if (!File.Exists(stringmaker))
    //                        File.Copy(folderName + "\\acmi0.txt", stringmaker, true);
    //                }


    //            }
    //        }
    //        else //this loop is not being used
    //        {

    //            string[] dire = Directory.GetDirectories(file);
    //            foreach (string direx in dire)
    //            {

    //                string rname = Path.GetFileName(Path.GetDirectoryName(direx));
    //                data = Directory.GetFiles(file, "*.ACM");
    //                data1 = Directory.GetFiles(file, "*.GPS");
    //                //UnityEngine.Debug.Log(data);
    //                //   UnityEngine.Debug.Log(data1);
    //                //foreach (string datax in data)
    //                //{
    //                //    name = rname + " " + Path.GetFileName(Path.GetDirectoryName(datax));
    //                //    File.Copy(datax, "C:\\\\Users\\AR 2\\Documents\\datafileX\\" + name + datax.Split('.')[datax.Count() - 1]);


    //            }
    //        }




    //    }


    //}
    //Tail ID in log.txt "- Tail ID: 531" 5th line Mir, F7; or "" in Jf17 "- A/C ID 141"
    // Update is called once per frame
    public void LogFile()
    {

        //string planeLogDir = Application.dataPath + "/Resources/MissionData/" + missionFolder + "/PlaneLog";
        string planeLogDir = Application.streamingAssetsPath + "/MissionData/" + missionFolder + "/PlaneLog";

        UnityEngine.Debug.Log("This is the path of planelog = " + planeLogDir);

        Directory.CreateDirectory(planeLogDir);
        string path = planeLogDir + "/" + Filename.text + "_Log.txt";
        StreamWriter file = File.CreateText(path);
        //Callsign & Short logic by sabs
        file.WriteLine("Callsign: {0}", callsign.options[callsign.value].text.ToString()); // +count Removed sabbar
        file.WriteLine("Short: {0}", callShort.text + count.ToString());
        file.WriteLine(typeAC.text);
        file.WriteLine(tailId.text);
        int i = Pilotname.value;
        file.WriteLine("Pilot Name: {0}", Pilotname.options[i].text);
        int j = Color.value;
        file.WriteLine("Color: {0}", Color.options[j].text);
        file.WriteLine("Remove Taxi Data: {0}", taxiData.isOn);
        file.Close();
       
    }

    public void resetCount()
    {
        count = 0;
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        //string path = Application.persistentDataPath + "/" + fname + ".dat";

        //string planeDataDir = Application.persistentDataPath + "/MissionData/" + missionFolder + "/PlaneData";
        //string planeDataDir = Application.dataPath + "/Resources/MissionData/" + missionFolder + "/PlaneData";
        string planeDataDir = Application.streamingAssetsPath + "/MissionData/" + missionFolder + "/PlaneData";

        Directory.CreateDirectory(planeDataDir);
        string path = planeDataDir + "/" + Filename.text + ".dat";
       
        FileStream file = File.Open(path, FileMode.Create);

        PlayerData data = new PlayerData();
        data.allData = alldata;
        data.TailId = tailId.text;
        data.TypeAC = typeAC.text;
        data.CallSign = callsign.options[callsign.value].text.ToString(); //removing count
        data.CallShort = callShort.text.ToString();//removing count

        int i = Pilotname.value;
        data.Pilotname = Pilotname.options[i].text;
        int j = Color.value;
        data.Color = Color.options[j].text;
        data.taxiData = taxiData.isOn;

        bf.Serialize(file, data);
        file.Close();



    }

        public void GetActivePlaneNameFromInputField()
        {
            PlayerPrefs.SetString("ActvePlaneName",filename );
           //UnityEngine.Debug.Log("ACTIVE PLANE NAME AFTER INPUT FROM UI" + PlayerPrefs.GetString("ActvePlaneName").ToString());

        }
    //Tail id Check
    public bool shouldAddNewRow() {
        if (TailIDElements.Contains(tailId.text))
        {
            dialogueBoxForRepeadtedTailId.SetActive(true);
            return false;
        }
        else
        {

            TailIDElements.Add(tailId.text);
            return true;
        }
    }

    public void OnOk()
    {
        if (shouldAddNewRow())
        {
            ExtractData();
            LogFile();
           
            //string startingTime = getStartTime();
            //string[] st = startingTime.Split(':');

            PlayerPrefs.SetFloat("StartHour", float.Parse(hr.text));
            PlayerPrefs.SetFloat("StartMinute", float.Parse(min.text));
            PlayerPrefs.SetFloat("StartSecond", float.Parse(sec.text));

			UnityEngine.Debug.Log ("updated hr from ui = " + PlayerPrefs.GetFloat("StartHour"));
			UnityEngine.Debug.Log ("updated hr from ui = " + PlayerPrefs.GetFloat("StartHour"));
			UnityEngine.Debug.Log ("updated hr from ui = " + PlayerPrefs.GetFloat("StartHour"));

			UnityEngine.Debug.Log (hr.text);
            //st[0] = PlayerPrefs.GetFloat("StartHour").ToString();
            //st[1] = PlayerPrefs.GetFloat("StartMinute").ToString();
            //st[2] = PlayerPrefs.GetFloat("StartSecond").ToString();
            //UnityEngine.Debug.Log("list values" + TailIDElements);
        }
       
    }
    //Main thread
    //TODO : Parallel thread
    public void ExtractData(string debugPath = "")
    {
        counterAllData = 0;
        //type = textFile.name.Split('_')[0];
        //callsign = textFile.name.Split('_')[2];
        //Name = textFile.name.Split('_')[1];
        //string filePath = missionFolder + "\\RawData\\" + Filename.text;
        // UnityEngine.Debug.Log(filePath);
        string path;
        if (debugPath == "")
            path = Application.dataPath + "\\StreamingAssets\\MissionData\\" + missionFolder + "\\RawData\\" + Filename.text + ".txt";
        else
            path = Application.dataPath + "\\StreamingAssets\\MissionData\\" + debugPath;
        string FileData = File.ReadAllText(path);

        //TextAsset textFile = Resources.Load<TextAsset>("MissionData\\" + missionFolder + "\\RawData\\" + Filename.text) as TextAsset;

        //string path = Application.dataPath + "\\Resources\\MissionData\\" + missionFolder + "\\RawData\\" + Filename.text + ".txt";
        //string FileData = File.ReadAllText(path);
        //string text = textFile.text;

        string[] data = FileData.Split('\n').Skip(3).ToArray();
        alldata.Clear();

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
                    if (taxiData.isOn)
                    {

                        if (typeAC.text == "AGSTA") //highest speed is 160 for AGSTA
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
                    else
                    {
                        alldata.Add(rdata);
                    }
                    //    //Data Extraction
                }
            }
        }

        t1 = alldata[0][0];
        localtime = MathCalculations.ConvertTime(t1, offset);
        starttime = localtime;

        endtime = MathCalculations.ConvertTime(alldata[alldata.Count - 1][0], offset);

        Save();


    }

    public void OnManualEdit()
    {
        Process p = new Process();
        p.StartInfo.FileName = filename  + ".txt";

        //p.StartInfo.WorkingDirectory = Application.dataPath + "/Resources/MissionData/" + missionFolder + "/RawData";

        p.StartInfo.WorkingDirectory = Application.streamingAssetsPath + "/Resources/MissionData/" + missionFolder + "/RawData";

        //p.StartInfo.WorkingDirectory = Application.persistentDataPath + "/MissionData/" + missionFolder + "/RawData";

        //p.StartInfo.Arguments = "E:\\ACMI Projects\\ProcessTest\\Assets\\Resources\\F-7_FL TALAHA.txt";
        p.StartInfo.CreateNoWindow = false;
        bool isStart = p.Start();
    
    }
    
}

