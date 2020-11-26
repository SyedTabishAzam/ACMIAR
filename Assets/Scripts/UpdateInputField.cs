using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpdateInputField : MonoBehaviour {

    public InputField mainInputField;
    private Calendar calendar;
    private Canvas dateCanvas;
    string day = System.DateTime.Now.ToString("MM/dd/yy");


    void Start()
	{
		
        mainInputField.text = day; 
        dateCanvas = GameObject.Find("Date Canvas").GetComponent<Canvas>();

    	if(!dateCanvas)
    	Debug.LogWarning("Did not find 'Date Canvas' canvas component!");
    	
    	dateCanvasEnabled(false);

    }

    public void UpdateField()
    {
		int dayNumb = EventSystem.current.currentSelectedGameObject.GetComponent<DayButton>().CurrentNumber();
		calendar = GameObject.FindObjectOfType<Calendar>();
       	mainInputField.text = calendar.ReturnDate(dayNumb);
		dateCanvasEnabled(false);

    }

    public void dateCanvasEnabled(bool canvasbool)
	{
		
    	dateCanvas.enabled = canvasbool;

    }
}