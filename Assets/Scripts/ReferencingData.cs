using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class ReferencingData : ScriptableObject {

    public List<CustomTuple> AircraftReferences = new List<CustomTuple>();
    public List<CustomDict> BullseyeRef = new List<CustomDict>();

    public void AddToRefList(GameObject air1, GameObject air2)
    {
        CustomTuple temp = new CustomTuple();
        temp.SetFirst(air1);
        temp.SetSecond(air2);
        temp.SetUniqueName(air1.name + air2.name);
		temp.SetDisplayName(air1.gameObject.GetComponent<movement>().getCallSign() + " ---- " + air2.gameObject.GetComponent<movement>().getCallSign());

        if (!RefListContains(temp))
            AircraftReferences.Add(temp);
    }

    public void RemoveFromRefList(string rowName)
    {
        foreach (CustomTuple child in AircraftReferences)
        {

            if (child.GetUniqueName() == rowName)
            {
                AircraftReferences.Remove(child);
                break;
            }
        }
    }

    public CustomTuple PopFromRefList(string rowName)
    {
        CustomTuple popObject = null;
        foreach (CustomTuple child in AircraftReferences)
        {

            if (child.GetUniqueName() == rowName)
            {
                popObject = child;
                AircraftReferences.Remove(child);
                break;
            }
        }
        return popObject;
    }
    private bool RefListContains(CustomTuple temp)
    {
        foreach (CustomTuple child in AircraftReferences)
        {
            if (child.GetUniqueName() == temp.GetUniqueName())
            {
                return true;
            }
        }
        return false;
    }

    public int GetRefListCount()
    {
        return AircraftReferences.Count;
    }

    public List<CustomTuple> GetRefList()
    {
        return AircraftReferences;
    }

    public void ClearRefList()
    {
        AircraftReferences.Clear();
       
    }

    public void ClearBullseyeList()
    {
        BullseyeRef.Clear();

    }
    public void AddToBullseyeList(GameObject ref1, GameObject ref2)
    {
        CustomDict temp = GetBullsEyeListContains(ref1.name);
        if (temp!=null)
        {
            if(!IsInBullseyeListOfList(temp,ref2))
            {

                temp.AddToList(ref2);
                SetBullList(ref1.name, temp.GetValues());
            }
        }

        else
        {
            temp = new CustomDict();
            temp.SetKey(ref1);
            temp.AddToList(ref2);
            BullseyeRef.Add(temp);
        }
       // temp.SetUniqueName(air1.name + air2.name);
        //temp.SetDisplayName(air1.name.Substring(0, 3) + " ---- " + air2.name.Substring(0, 3));

        
        
    }

    public void RemoveFromBullseyeList(string rowName)
    {
        foreach (CustomDict child in BullseyeRef)
        {

            if (child.GetKey().name == rowName)
            {

               
                BullseyeRef.Remove(child);
           
                break;
            }
        }
    }

    public void RemoveFromBullseyeSublist(string parentName,string rowName)
    {
        Debug.Log("Parent " + parentName);
        Debug.Log("rowName " + rowName);
        foreach (CustomDict child in BullseyeRef)
        {
            if (child.GetKey().name == parentName)
            {
                foreach (GameObject subchild in child.GetValues())
                {
                    Debug.Log("Child " + subchild.name);
                    if (subchild.gameObject.GetComponent<movement>().getCallSign() == rowName )
                    {
                        if (child.GetValues().Count == 1)
                        {
                            BullseyeRef.Remove(child);
                        }
                        else
                        {
                            child.GetValues().Remove(subchild);
                        }
                        return;
                    }
                }
            }
        }
    }

    public GameObject PopFromBullseyeSublist(string parentName, string rowName)
    {
        GameObject pop = null;
        foreach (CustomDict child in BullseyeRef)
        {
            if (child.GetKey().name == parentName)
            {
                foreach (GameObject subchild in child.GetValues())
                {
                    
                    if (subchild.gameObject.GetComponent<movement>().getCallSign() == rowName)
                    {
                        pop = subchild;
                        child.GetValues().Remove(subchild);
                       
                        return pop;
                    }
                }
            }
        }
        return pop;
    }

    public CustomDict PopFromBullseyeList(string name)
    {
        CustomDict popObject = null;
        foreach (CustomDict child in BullseyeRef)
        {

            if (child.GetKey().name == name)
            {
                popObject = child;
                BullseyeRef.Remove(child);
                break;
            }
        }
        return popObject;
    }

    public CustomDict GetFromBullseyeList(string name)
    {
        CustomDict popObject = null;
        foreach (CustomDict child in BullseyeRef)
        {

            if (child.GetKey().name == name)
            {
                return child;
                break;
            }
        }
        return popObject;
    }

    //public CustomTuple PopFromRefList(string rowName)
    //{
    //    CustomTuple popObject = null;
    //    foreach (CustomTuple child in AircraftReferences)
    //    {

    //        if (child.GetUniqueName() == rowName)
    //        {
    //            popObject = child;
    //            AircraftReferences.Remove(child);
    //            break;
    //        }
    //    }
    //    return popObject;
    //}
    private CustomDict GetBullsEyeListContains(string temp)
    {
        
        foreach (CustomDict child in BullseyeRef)
        {
            if (child.GetKey().name == temp)
            {
                return child;
            }
        }
        return null;
    }

    private GameObject IsInBullseyeListOfList(CustomDict temp,GameObject ref2)
    {

        foreach (CustomDict child in BullseyeRef)
        {
            if (child.GetKey().name == temp.GetKey().name)
            {
                foreach (GameObject subchild in child.GetValues())
                {
                    if(subchild.name == ref2.name)
                    {
                        return subchild;
                    }
                }

            }
        }
        return null;
    }

    private void SetBullList(string name, List<GameObject> list)
    {
        foreach (CustomDict child in BullseyeRef)
        {
            if (child.GetKey().name == name)
            {
                child.SetValues(list);
                break;
            }
        }
    }

    public int GetLastBullseyeListCount()
    {
        if(BullseyeRef.Count==0)
        {
            return 0;
        }
        return BullseyeRef[BullseyeRef.Count - 1].GetValues().Count;
        
    }

    public List<CustomDict> GetBullseyeList()
    {
        return BullseyeRef;
    }

    //public void ClearRefList()
    //{
    //    AircraftReferences.Clear();

    //}
}

public class CustomTuple
{
    string uniqueName = "";
    string DisplayName = "";
    GameObject A = null;
    GameObject B = null;

    public GameObject GetFirst()
    {
        return A;
    }

    public GameObject GetSecond()
    {
        return B;
    }

    public void SetFirst(GameObject a)
    {
        A = a;
    }

    public void SetSecond(GameObject b)
    {
        B = b;
    }

    public string GetDisplayName()
    {
        return DisplayName;
    }

    public string GetUniqueName()
    {
        return uniqueName;
    }

    public void SetDisplayName(string name)
    {
        DisplayName = name;
    }

    public void SetUniqueName(string uniqueNameTemp)
    {
        uniqueName = uniqueNameTemp;
    }
}


public class CustomDict
{
    GameObject primary = null;
    List<GameObject> secondary = new List<GameObject>();
    public GameObject GetKey()
    {
        return primary;
    }

    public List<GameObject> GetValues()
    {
        return secondary;
    }

    public void SetKey(GameObject temp)
    {
        primary = temp;
    }

    public void SetValues(List<GameObject> temp)
    {
        secondary = temp;
    }

    public void AddToList(GameObject temp)
    {
        secondary.Add(temp);
    }

    public void RemoveFromList(string name)
    {
        foreach(GameObject item in secondary)
        {
            if(item.name==name)
            {
                secondary.Remove(item);
                break;
            }
        }
    }
}