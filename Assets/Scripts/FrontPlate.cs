using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontPlate : MonoBehaviour
{

    GameObject child;
    // Use this for initialization
    void Awake()
    {
        child = transform.GetChild(0).gameObject;
        child.GetComponent<TextMesh>().text = " " ;
        

    }

    public void ChangeText(string text)
    {

        child.GetComponent<TextMesh>().text = text;
    

    }
}
