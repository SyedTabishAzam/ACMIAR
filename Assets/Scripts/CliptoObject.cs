using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliptoObject : MonoBehaviour {
   [SerializeField]
    public int indx;
    public GameObject cameraa;
    public float smooth = 4f;
	// Use this for initialization
	void Start () {
        if (!cameraa)
            cameraa = Camera.main.gameObject;
        
        

    }
	
	// Update is called once per frame
	void FixedUpdate () {

    
        CheckAndUpdateIndex();

        float parentRotation = transform.parent.rotation.eulerAngles.y;
        //indx = (indx - Mathf.FloorToInt(parentRotation / 90)) % 4;

        Quaternion target = Quaternion.Euler(0, indx * 90 + parentRotation, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

    }

    void CheckAndUpdateIndex()
    {
        //float rotationY = cameraa.transform.rotation.eulerAngles.y;


        ////Check to maintain the range of angle (no confusion on negative values and values greater than 360)


        //if (Mathf.Clamp(rotationY, -45f, 45f) == rotationY)
        //{
        //    indx = 0;
        //}

        //if (Mathf.Clamp(rotationY, 45f, 135f) == rotationY)
        //{
        //    indx = 1; //On parent 90 this should be 0
        //}

        //if (Mathf.Clamp(rotationY, 135f, 225f) == rotationY)
        //{
        //    indx = 2; //On parent 90 this should be 1
        //}

        //if (Mathf.Clamp(rotationY, 225f, 315f) == rotationY)
        //{
        //    indx = 3; //On parent 90 this should be 2
        //}

        //if (Mathf.Clamp(rotationY, 315f, 405f) == rotationY)
        //{
        //    indx = 0; //On parent 90 this should be 3
        //}

       


    }


}
