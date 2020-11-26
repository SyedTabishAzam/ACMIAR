using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class ServerManager : NetworkBehaviour {

   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartHost()
    {
        gameObject.GetComponent<NetworkManager>().StartHost();   
    }
}
