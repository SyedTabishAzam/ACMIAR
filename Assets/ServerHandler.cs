using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class ServerHandler : NetworkBehaviour {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SwitchToARScene()
    {
        SceneManager.LoadScene("ARScene");
    }


}
