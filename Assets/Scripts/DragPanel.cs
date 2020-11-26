using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragPanel : MonoBehaviour, IPointerDownHandler {

    private bool dragging;

    private Vector3 offset;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void Update ()
    {
        if (dragging)
        {
            Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            transform.position = new Vector3(screenToWorld.x,screenToWorld.y,-1f);
        }

        if(Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
        offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dragging = true;
    }
}
