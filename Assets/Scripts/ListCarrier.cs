using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListCarrier : MonoBehaviour {

    List<List<Formation>> formationData;
    List<string> missionData;
    bool hasList = false;
   
    public void SaveList(List<List<Formation>> tempList)
    {
        formationData = tempList;
        hasList = true;
    }

    public void SaveMission(List<string> tempList)
    {
        missionData = tempList;
    }


    public List<string> GetMission()
    {
        return missionData;
    }

    public List<List<Formation>> GetFormation()
    {
        return formationData;
    }

    public bool isContainingList()
    {
        return hasList;
    }
}
