using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textfollow : MonoBehaviour {


	Transform bar;
	// Use this for initialization
	void Start () {
		bar = GameObject.Find ("").transform;
		
	}
	
	// Update is called once per frame
	void Update () {


		transform.position = new Vector3 (bar.position.x, transform.position.y, transform.position.z);
		
	}
}
