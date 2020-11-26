using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConnection : MonoBehaviour {

	// Use this for initialization
	void Start () {

        ConnectTOServer();


    }
	

    void ConnectTOServer()
    {
        try
        {
            Network.Connect("172.25.103.109", 12345);

        }
        catch (System.Exception ep)
        {
            Debug.Log(ep.Message);
            
        }
    }
}
