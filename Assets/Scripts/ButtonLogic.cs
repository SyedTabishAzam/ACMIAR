using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonLogic : MonoBehaviour {
    private Button aircraftVisible;
    // Use this for initialization
    void Start () {
        aircraftVisible = GetComponent<Button>();
        aircraftVisible.onClick.AddListener(() => InvokeButtonPressed());
    }
	
	

    public void InvokeButtonPressed()
    {
        transform.parent.gameObject.GetComponent<DashboardRow>().Pressed();
    }
}
