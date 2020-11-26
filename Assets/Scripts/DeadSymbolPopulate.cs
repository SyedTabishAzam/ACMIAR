using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DeadSymbolPopulate : MonoBehaviour {

    public GameObject deadSymbolPan;
    public GameObject RefRow;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Populate()
    {
        DestroyAllChildren();

        List<GameObject> deadAirCrafts = deadSymbolPan.GetComponent<DeadSymbol>().GetList();
        float counter = 0;
        foreach (GameObject rowItems in deadAirCrafts)
        {
            GameObject row = Instantiate(RefRow, transform.Find("Rows"));

            row.name = rowItems.name;
            row.transform.GetChild(0).GetComponent<Text>().text = rowItems.name;
            row.transform.localPosition += Vector3.down * counter;
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
}
