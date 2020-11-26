using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceScale : MonoBehaviour {

    public GameObject markerPrefab;
	// Use this for initialization
	void Start () {
		for(int i =-10;i <=10;i++)
        {
            GameObject markerObj = Instantiate(markerPrefab, transform);
            markerObj.transform.localPosition = Vector3.right * (i * 0.09615f - 0.0385f);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
