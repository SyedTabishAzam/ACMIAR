using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DeadSymbol : MonoBehaviour {

    public GameObject deadSymbolCubePrefab;
    public GameObject Aircraft_parent;
    private GameObject selectedAircraft;
    public Dropdown Ddown1;
    public Text counterobj;
    List<GameObject> deadSymboledAirCrafts;
    public GameObject viewAllPanel;
    int count=0;
    private int val1;
    Scene activeScene;

	// Use this for initialization
	void Start () 
    {
        activeScene = SceneManager.GetActiveScene();
        val1 = Ddown1.value;
        deadSymboledAirCrafts = new List<GameObject>();

    }
	
	// Update is called once per frame
	void Update () {

        //counterobj.text = deadSymboledAirCrafts.Count.ToString();
    }

    public GameObject referencedObject(Dropdown Ddown)
    {
        string selected = Ddown.GetComponentInChildren<Text>().text;
        Debug.Log(selected);
        GameObject selectedObj = null;

        if (activeScene.name == "ARScene")
        {
            foreach (Transform child in Aircraft_parent.transform)
            {
                //if(child.name.Contains(selected))
                if (child.GetComponent<movement>().getCallSign().Contains(selected))
                {
                    //Debug.Log(child.name);
                    selectedObj = child.gameObject;
                }
            }
        }
       
        else if (activeScene.name == "LiveData")
        {
            foreach (Transform child in Aircraft_parent.transform)
            {
                string[] nameOfChildAircraft = child.gameObject.name.Split(' ');
                //Debug.Log("tail id of active aircraft is = " + nameOfChildAircraft[1]);
                if (nameOfChildAircraft[1] == selected)
                {
                    selectedObj = child.gameObject;

                }
            }
        }
        return selectedObj;
    }

    public void OnClearSignClick(GameObject aircraft)
    {
        foreach(GameObject child in deadSymboledAirCrafts)
        {
            if(child.name==aircraft.name)
            {
                deadSymboledAirCrafts.Remove(child);
                break;
            }
        }

       Destroy( GameObject.Find(aircraft.name).transform.GetChild(0).GetChild(0).gameObject);
       OnViewClick();
    }
    public void OnSelectAircraftClick(GameObject selectedAircraft = null)
    {
        if(!selectedAircraft)
            selectedAircraft = referencedObject(Ddown1);
        if (selectedAircraft && selectedAircraft.gameObject.activeInHierarchy)
        {
            foreach (GameObject child in deadSymboledAirCrafts)
            {
                if (child.name == selectedAircraft.name)
                {
                    return;
                }
            }
            //if (!selectedAircraft.transform.GetChild(0).Find("DeadSymbol")){}
            deadSymboledAirCrafts.Add(selectedAircraft);
            if (activeScene.name == "ARScene")
            {
                GameObject deadSymbolCube = Instantiate(deadSymbolCubePrefab, selectedAircraft.transform.GetChild(0));
                Bounds bound = selectedAircraft.gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds;
                deadSymbolCube.transform.localScale = new Vector3(bound.size.x, bound.size.y, bound.size.z);
                Debug.Log(bound.size.x + " " + bound.size.y + " " + bound.size.z);
                deadSymbolCube.name = "DeadSymbol";

                //Ddown1.options.RemoveAt(Ddown1.value); //remove dead aircraft from list 

                Ddown1.value = 0;
                OnViewClick();
            }
            else if(activeScene.name == "LiveData")
            {
                GameObject deadSymbolCube = Instantiate(deadSymbolCubePrefab, selectedAircraft.transform);
                Bounds bound = selectedAircraft.gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds;
                deadSymbolCube.transform.localScale = new Vector3(bound.size.x, bound.size.y, bound.size.z);
                Debug.Log(bound.size.x + " " + bound.size.y + " " + bound.size.z);
                deadSymbolCube.name = "DeadSymbol";

                //Ddown1.options.RemoveAt(Ddown1.value); //remove dead aircraft from list 

                Ddown1.value = 0;
                OnViewClick();

            }
           
        }
    }

    public void OnViewClick()
    {
        viewAllPanel.GetComponent<DeadSymbolPopulate>().Populate();
    }

    public List<GameObject> GetList()
    {
        return deadSymboledAirCrafts;
    }

    public void OnClearAllClick()
    {
        List<GameObject> grandChildren = new List<GameObject>();
        foreach (Transform child in viewAllPanel.transform.Find("Rows"))
        {
          
            grandChildren.Add(child.GetChild(1).gameObject);

        }

        foreach (GameObject child in grandChildren)
        {
            child.GetComponent<ClearSignLogic>().InvokeButtonPressed();
        }

        
    }
}
