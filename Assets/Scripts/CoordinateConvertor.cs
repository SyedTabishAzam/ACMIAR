using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoordinateConvertor : MonoBehaviour {

    private bool isBullEye = false;
    private GameObject bullsEyeRingObject;
    public GameObject BullsEyeManager;
    public GameObject testing;
    public Slider latitudeSlider;
    public Slider longitudeSlider;
    public GameObject LeftCamera;
    public GameObject RightCamera;

    Vector3 aquiredRay;
    Vector3 positionOffset;
    Vector3 middleCamera;
  
    // Use this for initialization
    void Start () {
        aquiredRay = Vector3.zero;
        //  positionOffset = bullsEyePointer.transform.localPosition;
      
    }
	
	// Update is called once per frame
	void Update () {
        if (isBullEye)
        {

        }
       
       
    }

    void OnMouseDown()
    {
       

    }

    public void ActivatePointRegistry(string name)
    {
        isBullEye = true;

        bullsEyeRingObject = GameObject.Find(name);
    }

  

   
}
