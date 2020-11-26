using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataExtraction : MonoBehaviour
{

    List<List<float>> alldata = new List<List<float>>();
    float leftlat = 32.05582f;
    float leftlong = 72.663652f;
    float rightlat = 32.05582f;
    float rightlong= 72.663652f;
    string Stime, Etime; 
    // Use this for initialization
    public void Awake()
    {
        if (!Directory.Exists("C:\\\\Users\\AR 2\\Documents\\datafile"))
        {
            string[] filePath = Directory.GetDirectories("C:\\\\Users\\AR 2\\Documents\\10-10-18");
            string name = "";
            foreach (string file in filePath)
            {
                //string rootname = Path.GetFileName(Path.GetDirectoryName(file));
                string[] data = Directory.GetFiles(file, "acmi0.txt");
                if (data.Length == 0)
                {
                    string[] dire = Directory.GetDirectories(file);
                    foreach (string direx in dire)
                    {
                        //Debug.Log(direx);
                        string rname = Path.GetFileName(Path.GetDirectoryName(direx));
                        data = Directory.GetFiles(direx, "acmi0.txt");
                        foreach (string datax in data)
                        {
                            name = rname + " " + Path.GetFileName(Path.GetDirectoryName(datax));
                            File.Copy(datax, "C:\\\\Users\\AR 2\\Documents\\datafile\\" + name);


                        }
                    }
                }
                else
                {
                    //Debug.Log(data.Length);
                    foreach (string datax in data)
                    {
                        name = Path.GetFileName(Path.GetDirectoryName(datax));
                        File.Copy(datax, "C:\\\\Users\\AR 2\\Documents\\datafile\\" + name);
                        //Debug.Log(datax);
                    }
                }
                name = "";
            }
        }
    }

    public void Start()
    {
        ExtractLatLongBounds();
    }

    public void ExtractData(string filepath)
    {

        string[] text = File.ReadAllText(filepath).Split('\n');
        //Debug.Log(filepath);
        text  = text.Skip(3).ToArray();
        //Debug.Log(text[0]);
        alldata.Clear();
        
        foreach (var item in text)
        {
            //Debug.Log(item);
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

                        rdata.Add(float.Parse(datapoint));
                        maxFields++;

                        if (maxFields >= 12)
                        {
                            break;
                        }

                    }
                }

                if (rdata[11] >= 100)
                {
                    alldata.Add(rdata);
                }

            }
        }
        
        //Etime = alldata[alldata.Count-1][0];
        for (int i = 0; i < alldata.Count; i++)
        {
            Stime = MathCalculations.ConvertTime(alldata[0][0]);
            Etime = MathCalculations.ConvertTime(alldata[i][0]);
            leftlat = Math.Min(leftlat, alldata[i][1]);
            leftlong = Math.Min(leftlong, alldata[i][2]);
            rightlat = Math.Max(rightlat, alldata[i][1]);
            rightlong = Math.Max(rightlong, alldata[i][2]);
        }
        Debug.Log(filepath + " starttime " + Stime + " endtime " + Etime);
    }

    public void ExtractLatLongBounds()
    {
        string[] data = Directory.GetFiles("C:\\\\Users\\AR 2\\Documents\\datafile");
        int i = 0;
        foreach (string file in data)
        {
            ExtractData(file);
            i++;
        }
        //Debug.Log(leftlat + " " + rightlat+ " " + leftlong + " "+ rightlong);
    }

    void Update()
    {
        
    }
}
