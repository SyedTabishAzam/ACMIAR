using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CollisionDetection : MonoBehaviour, IPointerClickHandler
{

    private Color defaultColor;
    public GameObject airRefManager;
	// Use this for initialization
	void Start () {
        Renderer rend = gameObject.GetComponent<Renderer>();
        defaultColor = rend.material.color;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnClicked();
        if (eventData.button == PointerEventData.InputButton.Right)
            transform.parent.gameObject.GetComponent<movement>().ToggleLaunchMissile();
    }


    public void OnClicked()
    {
        
        transform.parent.GetChild(1).gameObject.SetActive(!transform.parent.GetChild(1).gameObject.activeInHierarchy);
        transform.parent.GetComponent<movement>().changeSelected();

        airRefManager = GameObject.Find("AircraftRefManager");
        if(airRefManager)
        {
            Debug.Log(transform.parent.name);
            if(airRefManager.GetComponent<ReferenceSelection>().GetMode().Contains( "SELECTION1"))
            {
                for(int i = 0; i< airRefManager.GetComponent<ReferenceSelection>().Ddown1.options.Count; i++)
                {
                    if(airRefManager.GetComponent<ReferenceSelection>().Ddown1.options[i].text == transform.parent.GetComponent<movement>().callsign)
                    {
                        airRefManager.GetComponent<ReferenceSelection>().Ddown1.value = i;
                        airRefManager.GetComponent<ReferenceSelection>().SetMode("SELECTION2");
                        break;
                    }
                }
            }

            else if (airRefManager.GetComponent<ReferenceSelection>().GetMode().Contains("SELECTION2"))
            {
                for (int i = 0; i < airRefManager.GetComponent<ReferenceSelection>().Ddown2.options.Count; i++)
                {
                    if (airRefManager.GetComponent<ReferenceSelection>().Ddown2.options[i].text == transform.parent.GetComponent<movement>().callsign)
                    {
                        airRefManager.GetComponent<ReferenceSelection>().Ddown2.value = i;
                        airRefManager.GetComponent<ReferenceSelection>().SetMode("COMPLETE");
                        break;
                    }
                }
            }
        }
    }


    void OnMouseOver()
    {
        Renderer rend = gameObject.GetComponent<Renderer>();
        rend.material.color = Color.green;
    }

    void OnMouseExit()
    {
        Renderer rend = gameObject.GetComponent<Renderer>();
        rend.material.color = defaultColor;
    }

    private void OnMouseDown()
    {
        GetComponentInParent<movement>().toggleMissileLaunch = true;
    }
}
