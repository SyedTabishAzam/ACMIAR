using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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

public class loadcontrol : MonoBehaviour {

	// Use this for initialization
	void OnLoadClick () {
       
        string path = EditorUtility.OpenFilePanel("Choose a file", Application.dataPath, ".xml");
        if (path.Equals("")) { return; }
        UnityEngine.Debug.Log("OPP" + path);
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
