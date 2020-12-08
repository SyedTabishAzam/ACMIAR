using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Planeinfo : NetworkBehaviour {

    [SyncVar]
    public Vector3 linePosition1;

    [SyncVar]
    public Vector3 linePosition2;

    [SyncVar]
    public string Labeltext;

    [SyncVar]
    public Transform parentObject;

    TextMesh textMesh;
    LineRenderer lineRenderer;
    // Use this for initialization
    void Start () {
        textMesh = transform.Find("Pivot").Find("ContentParent").Find("Label").GetComponent<TextMesh>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, linePosition1);
        lineRenderer.SetPosition(1, linePosition2);
    }
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);

        if (transform.parent != parentObject)
            transform.SetParent(parentObject);

        textMesh.text = Labeltext; ;

        lineRenderer.SetPosition(0, linePosition1);
        lineRenderer.SetPosition(1, linePosition2);
    }
}
