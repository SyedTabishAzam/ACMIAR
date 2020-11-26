using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateFromXML : MonoBehaviour {

    public GameObject rowPrefab;
    public GameObject BullsEyeManager;
    private List<List<float>> data;
    public void PopulateRows()
    {
        //get 
        data = BullsEyeManager.GetComponent<BullEyeHandle>().GetXMLData();
        //Iterate and create row
        //Edit text - lat long
        //Make "Rows" parent
        DestroyAllChildren();
        float counter = 0;
        float itemCounter = 0;
        foreach (List<float> item in data)
        {
            GameObject row = Instantiate(rowPrefab, transform.Find("Rows"));
            string stringMaker = "Lat: " + item[0] + ", Long: " + item[1];
            row.transform.GetChild(0).GetComponent<Text>().text = stringMaker;
            row.transform.localPosition += Vector3.down * counter;
            row.name = itemCounter.ToString();
            itemCounter++;
            counter += 0.1f;
        }
        
    }

    void DestroyAllChildren()
    {
        foreach (Transform child in transform.Find("Rows"))
        {
            Destroy(child.gameObject);
        }
    }

    public void OnRowClick(string name)
    {
        int indx = int.Parse(name);
        BullsEyeManager.GetComponent<BullEyeHandle>().SelectFromXMLIndex(indx);
    }
  
}
