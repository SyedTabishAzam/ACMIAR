using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
		
	}

    private void FixedUpdate()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
        UpdateName();
    }

    void UpdateName()
    {
        if(GetComponent<TextMesh>())
        {

        string  thisName = GetComponent<TextMesh>().text;
        GetComponent<TextMesh>().text = thisName.Substring(0, 2) + thisName.Substring(thisName.Length - 1, 1);
        }
    }
    // Update is called once per frame

}
