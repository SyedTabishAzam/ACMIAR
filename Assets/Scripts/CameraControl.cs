using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    // Use this for initialization
    public static int FACING_INDEX = 0;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.name=="FrontWall")
        {
            FACING_INDEX = 0;
        }
        if (other.name == "LeftWall")
        {
            FACING_INDEX = 1;
        }
        if (other.name == "BackWall")
        {
            FACING_INDEX = 2;
        }
        if (other.name == "RightWall")
        {
            FACING_INDEX = 3;
        }
       
    }
}
