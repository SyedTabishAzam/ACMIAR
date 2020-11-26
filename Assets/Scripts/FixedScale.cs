using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class FixedScale : MonoBehaviour {

	// Use this for initialization
	public float FixScale = 1;
	public GameObject parent;
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3 (FixScale / parent.transform.localScale.x, FixScale / parent.transform.localScale.y, FixScale / parent.transform.localScale.z);
	}
}
