using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearSignLogic : MonoBehaviour {

    private GameObject Manager;
    private Button clearsignBtn;
    bool isBullseyeClear = false;
    bool init = false;
    bool isDeadSymbol = false;
    // Use this for initialization
    void Start()
    {
        Init();

    }

    private void Init()
    {
        if (transform.parent.parent.parent.name.Contains("ViewAllDeadSymbols"))
        {
            isDeadSymbol = true;
        }
        else if (name.Contains("RefClearSign"))
        {
            isBullseyeClear = false;
        }
        else
        {
            isBullseyeClear = true;
        }
       

        if (isBullseyeClear)
        {
            Manager = GameObject.Find("BullsEyeManager");
        }
        else if(isDeadSymbol)
        {
            Manager = GameObject.Find("DeadSymbolPanel");

        }
        else
        {
            Manager = GameObject.Find("AircraftRefManager");
        }
       
        clearsignBtn = GetComponent<Button>();
        clearsignBtn.onClick.AddListener(() => InvokeButtonPressed());
    }

    public void InvokeButtonPressed()
    {
        if (!init)
            Init();

        Debug.Log("Pressed" + isBullseyeClear);

        if(isDeadSymbol)
        {
            Manager.GetComponent<DeadSymbol>().OnClearSignClick(transform.parent.gameObject);
        }
        else if(isBullseyeClear)
        {
            Manager.GetComponent<BullEyeHandle>().OnClearSignClick(transform.parent.gameObject);
        }
        else
        {
            Manager.GetComponent<ReferenceSelection>().RemoveSelected(transform.parent.gameObject);
        }
    }

    
}
