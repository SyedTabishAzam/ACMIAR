using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwapRows : MonoBehaviour, IPointerClickHandler
{
   
    private Color prevcolor;
    void start()
    {
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject loggingScriptRef = GameObject.Find("AddMissionManager");
       Logging refscript =  loggingScriptRef.GetComponent<Logging>();
        refscript.clearSelection();
        refscript.rowSelectedPrefab = this.gameObject;
        prevcolor = gameObject.GetComponent<Image>().color;
       gameObject.GetComponent<Image>().color = Color.red;
       
    }
   void ClearRowColor()
    {
      

    }


    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
