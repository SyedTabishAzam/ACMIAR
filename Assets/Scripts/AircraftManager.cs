using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftManager : MonoBehaviour {

    public float alt= 0; 
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        float tometer = MathCalculations.ConvertFeettoMeter(alt);
        float topixel = MathCalculations.ConvertFeettoMeter(tometer);
        Debug.Log(topixel);
        //this.transform.position = new Vector3(0, topixel, 0);

    }
}
