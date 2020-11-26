using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Extrat : MonoBehaviour {

    // Use this for initialization
    public void Awake()
    {
        if (!Directory.Exists("C:\\\\Users\\AR 2\\Documents\\datafileX"))
        {
            string[] filePath = Directory.GetDirectories("C:\\\\Users\\AR 2\\Documents\\10-10-18");
            string name = "";
            foreach (string file in filePath)
            {
                //string rootname = Path.GetFileName(Path.GetDirectoryName(file));
                string[] data = Directory.GetFiles(file, "*.ACM");
                string[] data1 = Directory.GetFiles(file, "*.GPS");
                Debug.Log(file); 
                if (data.Length == 0)
                {
                    string[] dire = Directory.GetDirectories(file);
                    foreach (string direx in dire)
                    {
                        //Debug.Log(direx);
                        string rname = Path.GetFileName(Path.GetDirectoryName(direx));
                        data = Directory.GetFiles(file, "*.ACM");
                        data1 = Directory.GetFiles(file, "*.GPS");
                        foreach (string datax in data)
                        {
                            name = rname + " " + Path.GetFileName(Path.GetDirectoryName(datax));
                            File.Copy(datax, "C:\\\\Users\\AR 2\\Documents\\datafileX\\" + name + datax.Split('.')[datax.Count()-1]);


                        }
                    }
                }
                else
                {
                    //Debug.Log(data.Length);
                    foreach (string datax in data)
                    {
                        name = Path.GetFileName(Path.GetDirectoryName(datax));
                        File.Copy(datax, "C:\\\\Users\\AR 2\\Documents\\datafileX\\" + name + datax.Split('.')[datax.Count() - 1]);
                        //Debug.Log(datax);
                    }
                }
                name = "";
            }
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}
