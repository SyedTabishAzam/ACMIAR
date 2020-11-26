using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MetaDebug : MonoBehaviour {

    public  GameObject debugPrefab;
    public  GameObject parentRows;
    public static int counter = 1;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Log(string text)
    {
        GameObject debugLogRow = Instantiate(debugPrefab, parentRows.transform);
        debugLogRow.transform.localPosition += Vector3.down * counter * 0.073f;
        counter++;
        if (counter >= 10)
        {

            ResetLogs();
            counter = 1;
        }
        debugLogRow.GetComponent<TextMesh>().text = text;
    }

    public void ResetLogs()
    {
        foreach(Transform child in parentRows.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
