using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BullseyeXMLBtnLogic : MonoBehaviour {
    private Button rowBtn;
    // Use this for initialization
    void Start()
    {
        rowBtn = GetComponent<Button>();
        rowBtn.onClick.AddListener(() => InvokeButtonPressed());
    }



    public void InvokeButtonPressed()
    {
        GameObject.Find("xmlPanel").GetComponent<PopulateFromXML>().OnRowClick(name);
    }
}
