using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PopulateAirRef : MonoBehaviour {

    public GameObject AirRefManager;
    public GameObject RefRow;
    public GameObject BullRowMain;
    public GameObject BullRowChild;
    private List<CustomTuple> refList;
    private List<CustomDict> BullList;
    public ReferencingData refData;
    // Use this for initialization
    void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
       
        
    }

    public void Populate()
    {
        refList = refData.GetRefList();
        DestroyAllChildren();
        float counter = 0;
        foreach(CustomTuple rowItems in refList)
        {
            GameObject row = Instantiate(RefRow, transform.Find("Rows"));
      
            row.name = rowItems.GetUniqueName();
            row.transform.GetChild(0).GetComponent<Text>().text = rowItems.GetDisplayName();
            row.transform.localPosition += Vector3.down * counter;
            counter += 0.1f;
        }
        
    }

    public void PopulateBull()
    {
        BullList = refData.GetBullseyeList();
        DestroyAllChildren();
        float counter1 = 0;
        
        foreach (CustomDict rowItem in BullList)
        {
            GameObject row = Instantiate(BullRowMain, transform.Find("Rows"));
            row.name = rowItem.GetKey().name;
            row.transform.GetChild(0).GetComponent<Text>().text = row.name;
            row.transform.localPosition += Vector3.down * counter1;

            float counter2 = 0;
            foreach (GameObject aircraft in rowItem.GetValues())
            {
                GameObject rowChild = Instantiate(BullRowChild, row.transform);
                rowChild.transform.GetChild(0).GetComponent<Text>().text = aircraft.GetComponent<movement>(). getCallSign();
                rowChild.name = aircraft.GetComponent<movement>().getCallSign(); 
                rowChild.transform.localPosition += Vector3.down * (counter2 * 20f);
                counter2 += 1;
            }
            counter1 += ((counter2+1) * 0.052f);
        }

    }
    void DestroyAllChildren()
    {
        foreach(Transform child in transform.Find("Rows"))
        {
            Destroy(child.gameObject);
        }
    }



}
