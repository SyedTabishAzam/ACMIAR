using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
public class SmoothRotation : MonoBehaviour {
 
    [Range(0, 360)] public float angle;//Specify Angle For Rotation
    [Range(0, 360)] public float yAngle;
    float tempAngle;//Temporary Angle Measurement Variable
    bool horizontalFlag;//Check For Horizontal Roation
    bool verticalFlag;//Check For Vertical Roation
    bool isRotating;//Check Whether Currently Object is Rotating Or Not.
    int Direction;//Direction Of Rotation
    public Transform pointT;
	public Transform DummyPivot;
    public GameObject compass;
    float CurrentAngle = 0;
    public Slider tiltSlid;
	public GameObject terrain3D;
	public GameObject terrain2D;
    //Called For Initialization
    void Start()
    {
        horizontalFlag = verticalFlag = isRotating = false;
    }

	public void SetOn2DTerrainClick(bool val)
	{
		terrain3D.SetActive (val);
		terrain2D.SetActive (!val);
	}

    
    public void Rotate(string directionAndHorizontal)
    {
        //00 == direction = -1,, horizontal = false
        //01 == direction = -1, horizontal = true
        //10 == direction = 1, horizontal = false
        //11 == direction = 1, horizontal = true
        //Doing this because UI buttons cant call function with 2 args

        int direction = 1;
        bool isHorizontal = true;
        string code = directionAndHorizontal.ToString();
        if (code == "00")
        {
            direction = -1;
            isHorizontal = false;
        }
        if (code == "01")
        {
            direction = -1;
            isHorizontal = true;
        }
        if (code == "10")
        {
            direction = 1;
            isHorizontal = false;
        }
        if (code == "11")
        {
            direction = 1;
            isHorizontal = true;
        }

        if (!isRotating)
        {
            Direction = direction;
            isRotating = true;
            if (isHorizontal)
                horizontalFlag = true;
            else
                verticalFlag = true;
            TrailControl.crTrail = true;
        }
    }
    public void RotateAllInZoomables()
    {
        foreach(Transform child in transform.Find("Zoomables"))
        {
            child.RotateAround(pointT.position, child.transform.up, angle * Time.fixedDeltaTime * Direction);
        }
        compass.transform.Rotate(Vector3.forward, -angle * Time.fixedDeltaTime * Direction);
    }
    public void RotateAroundAxis(bool isHorizontal)
    {
        if(isHorizontal)
        {
            RotateAllInZoomables();
            
        }
        else
        {
            //Might not work in zoom
          
        }

        tempAngle += angle * Time.fixedDeltaTime;
        if (tempAngle >= angle)
        {
            tempAngle = 0;
            isRotating = false;

            if (isHorizontal)
                horizontalFlag = false;
            else
                verticalFlag = false;

            TrailControl.crTrail = false;
        }
    }


    void Update()
    {

        if (horizontalFlag)
            RotateAroundAxis(true);

        //if (verticalFlag)
            //RotateAroundAxis(false);

        //Transform zoomableTrans = transform.Find("Zoomables");
        //zoomableTrans.localRotation = Quaternion.Euler(new Vector3(tiltSlid.value * -1, zoomableTrans.localRotation.y, zoomableTrans.localRotation.z));

    }
}