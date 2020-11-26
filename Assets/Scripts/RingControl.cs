using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingControl : MonoBehaviour {

    public GameObject ringPrefab;
    public Transform ringParent;
    private GameObject ringObject;
    bool startDraw = false;
    bool registerMouse = false;
    Vector3 initialPosition;
    Vector3 initialScale;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(registerMouse)
        {
            Debug.Log("yes");

            if (startDraw && Input.GetMouseButtonUp(0))
            {
                Debug.Log("y");
                startDraw = false;
                registerMouse = false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("x");
                startDraw = true;
                initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                initialScale = ringObject.transform.localScale;
            }
           
           
            

            if (startDraw)
            {
                Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - initialPosition;
                ringObject.transform.localScale = new Vector3(initialScale.x + difference.x, initialScale.y + difference.y);
            }
        }
    }

   public void DrawRing()
    {
        ringObject = Instantiate(ringPrefab, ringParent);
        registerMouse = true;
       
    }
}
