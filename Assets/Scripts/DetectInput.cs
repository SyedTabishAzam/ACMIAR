using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectInput : MonoBehaviour {

    public Text LatLong;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 mc = Input.mousePosition;
        Debug.Log(mc);
        LatLong.text = mc.ToString(); 
    }
}
