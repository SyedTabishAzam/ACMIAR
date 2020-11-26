using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ActionScript : MonoBehaviour , IPointerDownHandler{
    private Button editButton;
    
    // Use this for initialization
    void Start () {
        editButton = transform.Find("Edit").GetChild(0).GetComponent<Button>();
        editButton.onClick.AddListener(() => OnEditClick());
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void OnEditClick()
    {
        
        int index = transform.GetSiblingIndex() - 1;
        GameObject.Find("ParserManager").GetComponent<ParseMe>().OnEditClick(index);

    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
        int index = transform.GetSiblingIndex() - 1;
        GameObject.Find("ParserManager").GetComponent<ParseMe>().OnRowClick(index); 
    }
}
