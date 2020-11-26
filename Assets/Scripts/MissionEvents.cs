using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionEvents : MonoBehaviour {

    public List<string> missionEvents;
    public List<int> currentcounterAlldata;
    public List<float> slidervalues;
    public Dropdown Ddownevents;
    public GameObject GameManager;
    int val1 = 0;

    public void Start()
    {
        missionEvents.Add("None");
    }

    public void AddMissionEvent(string data, int datatime)
    {
        if (!(missionEvents.Contains(data)))
        {
            missionEvents.Add(data);
            currentcounterAlldata.Add(datatime);
            slidervalues.Add(GameManager.GetComponent<TimeControl>().GetSliderValue());
        }
        updateDropdown();
        
    }

    public void updateDropdown()
    {
        Ddownevents.GetComponent<Dropdown>().ClearOptions();
        Ddownevents.GetComponent<Dropdown>().AddOptions(missionEvents);
    }

    public void JumptoEvent()
    {
        int i = Ddownevents.GetComponent<Dropdown>().value-1; 
        float sliderValue = slidervalues[i];
        GameManager.GetComponent<TimeControl>().JumptoEvent(sliderValue);
    }

    private void FixedUpdate()
    {
        if (val1 != Ddownevents.value)
        {
            val1 = Ddownevents.value;
            JumptoEvent();
        }
    }

}
