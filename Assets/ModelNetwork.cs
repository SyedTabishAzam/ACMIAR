using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class ModelNetwork : NetworkBehaviour
{

    [SyncVar]
    public Transform parentObject;
    [SyncVar]
    public Vector3 initialNetworkPosition;
    [SyncVar]
    public Quaternion initialNetworkRotation;
    // Use this for initialization
    void Start()
    {
        // 
    }

    [ClientRpc]
    public void RpcReparent()
    {
        Debug.Log("Setting parent");
        transform.position = initialNetworkPosition;
        transform.rotation = initialNetworkRotation;
    }


    // Update is called once per frame
    void Update()
    {
        if (transform.parent != parentObject)
        {
            transform.SetParent(parentObject);
          //  GetComponent<BoxCollider>().enabled = true;
        }
        if (transform.localRotation.eulerAngles.y != 180)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
