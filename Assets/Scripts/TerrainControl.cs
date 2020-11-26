using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainControl : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector4 GetTerrainSpecs()
    {
        float leftlong = 70;
        float bottomlat = 30;
        float rightlong = 74;
        float toplat = 34;
        return new Vector4(leftlong,bottomlat,rightlong,toplat);
    }
}
