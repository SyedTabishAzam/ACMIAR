using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClickDetection : MonoBehaviour {


    bool clicked = false;
    private LineRenderer lineRend;
    //private Vector2 mousePos;
    //private Vector2 StartMousePos; 
	// Use this for initialization
	void Start () {
		lineRend=GetComponent<LineRenderer>();
        lineRend.positionCount = 2; 


	}

    Vector3 GetMousePosInWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       // ray.origin = new Vector3(ray.origin.x,ray.origin.y,0);
        return ray.origin + ray.direction * (Camera.main.transform.position.z * -1);
    }
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetMouseButtonDown(0))
        {
            
            Vector3 mc = Input.mousePosition;
            mc.z = (Camera.main.transform.position.z * -1);
             Vector3 pos = Camera.main.ScreenToWorldPoint(mc);
           // Vector3 pos = GetMousePosInWorld();
            pos = new Vector3(pos.x, pos.y, -1);
         //   Vector3 StartMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10f);
            lineRend.SetPosition(0, pos);
      
         //   Debug.Log(StartMousePos);

      
        }

        if (Input.GetMouseButton(0))
        {
            //Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10f);
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.main.transform.position.z * -1)));
           // Vector3 pos = GetMousePosInWorld();
            pos = new Vector3(pos.x, pos.y, -1);
            lineRend.SetPosition(1, pos);
            //Debug.Log("This is mid");
        }
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("This is up");
        }
  

    }

    public void Drawline()
    {
        clicked = true; 
    
        //GameObject line1;
        //LineRenderer currentLineRenderer;
        //line1 = Instantiate(line, lineParent.transform);
        //LineRenderer currentLineRenderer = gameObject.GetComponent<LineRenderer>();

        //if (Input.GetMouseButton(0))
        //{
        //    currentLineRenderer.positionCount++;
        //    Vector3 point1 = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, -10) ;
        //    currentLineRenderer.SetPosition(currentLineRenderer.positionCount-1, point1);

        //}
        
        //if(Input.GetMouseButtonUp(0))
        //{
        //    enabled = false; 
        //}
        
    }


    
}
