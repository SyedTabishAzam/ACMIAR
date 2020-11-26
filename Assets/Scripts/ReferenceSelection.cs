using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReferenceSelection : MonoBehaviour {
    public Dropdown Ddown1, Ddown2;
    public GameObject Aircraft_parent;
    public GameObject referencingManager;
    public GameObject AircraftPanel;
    public GameObject ViewAllPanel;
    public GameObject aircraftSelectIndicator;
    public GameObject metaCanvas;
    //public GameObject ViewBullPanel;
    private string mode = "IDLE";
    private bool copyClicks = false;

    public ReferencingData refData;
    int val1, val2 = 0;
    public Text counter; 
    bool ref1, ref2 = false;
    private GameObject selectedref1, selectedref2 = null;
    public bool clear = false;
    private CustomTable retValues1 ;
    private CustomTable retValues2;
    private bool updatePopulateRow1 = false;
    private bool updatePopulateRow2 = false;
    // Update is called once per frame


    void FixedUpdate ()
    {
        
        if (name == "Parameter Reference Table")
        {
            //-------------------------------------//
            //If dropdown 1 is selected
            if (val1 != Ddown1.value) 
            {
                selectedref1 = referencedObject(Ddown1);
                val1 = Ddown1.value;
                updatePopulateRow1 = true;
            }



            //Only update if dropdown1 is not "none"
            if (updatePopulateRow1)
            {
                int heightOfTable = GetComponent<ReferenceManager>().GetHeight();
                int currentPage = GetComponent<ReferenceManager>().GetPage();

                //h = 4, p = 0 ---- display aircrafts 0 - 3 ---- si >= (p*h) and si < (p*h) + h
                //h = 4, p = 1 ---- display aircrafts 4 - 7

                retValues1 = referencingManager.GetComponent<ReferenceCalculator>().OneToManyParameterReference(selectedref1, selectedref2,currentPage, heightOfTable);
				GetComponent<ReferenceManager> ().SetCustomTable (retValues1);
				GetComponent<ReferenceManager>().PopulateRow();

            }

            //If dropdown1 is cleared
            if (val1 == 0)
            {
                updatePopulateRow1 = false;
                selectedref1 = null;
            }
            //-------------------------------------//

            //-------------------------------------//
            //If dropdown 2 is selected
            if (val2 != Ddown2.value)
            {
                selectedref2 = referencedObject(Ddown2);
                val2 = Ddown2.value;
                updatePopulateRow2 = true;
            }



            //Only update if dropdown2 is not "none"
            if (updatePopulateRow2)
            {
                int heightOfTable = GetComponent<ReferenceManager>().GetHeight();
                int currentPage = GetComponent<ReferenceManager>().GetPage();

                retValues2 = referencingManager.GetComponent<ReferenceCalculator>().OneToManyParameterReference(selectedref1, selectedref2, currentPage, heightOfTable);
                GetComponent<ReferenceManager> ().SetCustomTable (retValues1);
				GetComponent<ReferenceManager>().PopulateRow();
            }

            //If dropdown2 is cleared
            if (val2 == 0)
            {
                updatePopulateRow2 = false;
                selectedref2 = null;
            }
        }
        else
        {
            counter.text = refData.GetRefListCount().ToString();

            if(copyClicks)
            {

                if (Input.GetMouseButtonDown(1))
                {
                    OnMouseButtonClick(1);
                }
                else
                {
                    OnMouseButtonClick(0);
                }
            }
        }
        //-------------------------------------//


    }
    
    public string GetMode()
    {
        return mode;
    }

    public void SetMode(string _mode)
    {
        mode = _mode;
    }

    public void NewSelection()
    {
        AircraftPanel.SetActive(false);
        ActivateAircraftRef();
    }

    public void ActivateAircraftRef()
    {
        copyClicks = true;
        IsolateSelection(true);
        mode = "SELECTION1";
    }

    public void OnMouseButtonClick(int button)
    {
        if (button == 0)
        {
            if (mode == "COMPLETE")
            {
                mode = "IDLE";
                AircraftPanel.SetActive(true);
                IsolateSelection(false);
                copyClicks = false;
            }
        }
        else if (button == 1)
        {
            mode = "IDLE";
            AircraftPanel.SetActive(true);
            IsolateSelection(false);
            copyClicks = false;
        }
    }


    public void IsolateSelection(bool isolate)
    {
        aircraftSelectIndicator.SetActive(isolate);
    }

    public GameObject referencedObject(Dropdown Ddown)
    {

        string selected = Ddown.GetComponentInChildren<Text>().text;

        GameObject selectedObj = null;

        foreach (Transform child in Aircraft_parent.transform)
        {
            if (child.gameObject.GetComponent<movement>().getCallSign() == selected)
            {
                //Debug.Log(child.name);
                
                selectedObj = child.gameObject;
            }
        }
        return selectedObj;
    }


    public void clearSelection()
    {
        selectedref1 = null;
        selectedref2 = null;
        Ddown1.GetComponent<Dropdown>().value = 0;
        Ddown2.GetComponent<Dropdown>().value = 0;

        referencingManager.GetComponent<ReferenceCalculator>().ClearOneToOnePR();
    }
    public void resetAll()
    {
        selectedref1 = null;
        selectedref2 = null;
        updatePopulateRow1 = false;
        updatePopulateRow2 = false;
        Ddown1.GetComponent<Dropdown>().value = 0;
        Ddown2.GetComponent<Dropdown>().value = 0;
        //ClearAllConnections();
    }
    public void AddNew()
    {
        if(refData.GetRefListCount()<7)
        {

            selectedref1 = referencedObject(Ddown1);
            selectedref2 = referencedObject(Ddown2);
   
			if(selectedref1&&selectedref2 && selectedref1!=selectedref2)
                if(selectedref1.activeInHierarchy && selectedref2.activeInHierarchy)
                {

                    refData.AddToRefList(selectedref1,selectedref2);
                    ViewAllPanel.GetComponent<PopulateAirRef>().Populate();
                }
        }
    }

    public void RemoveSelected(GameObject row)
    {
        if(row)
        {
           
            CustomTuple temp = refData.PopFromRefList(row.name);
            GetComponent<ReferenceCalculator>().ClearLine(temp.GetFirst(),temp.GetSecond());
            GetComponent<ReferenceCalculator>().ClearCalculationsText(temp.GetFirst(), temp.GetSecond());
            ViewAllPanel.GetComponent<PopulateAirRef>().Populate();
        }
    }

    public void ClearAllConnections()
    {
        List<GameObject> grandChildren = new List<GameObject>();
        foreach (Transform child in ViewAllPanel.transform.Find("Rows"))
        {
            Debug.Log(child.name);
            grandChildren.Add(child.GetChild(1).gameObject);
           
        }

        foreach(GameObject child in grandChildren)
        {
            child.GetComponent<ClearSignLogic>().InvokeButtonPressed();
        }
  //      refData.ClearRefList();
  //      GetComponent<ReferenceCalculator>().ClearVisuals(false);
		//ViewAllPanel.GetComponent<PopulateAirRef>().Populate();
    }

    public void Close()
    {

    }

    public void Reset()
    {

    }
}
