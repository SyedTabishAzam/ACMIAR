using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{

    public TextAsset textFile;
    private Vector3 targetPosition, startPosition;
    private Quaternion targetRotation, startRotation;
    public string Name;



    private Vector3 initialPosition;
    float timeToReachTarget;
    bool flag = true;
    float t1 = 0, t2, deltat, x, y, z, pitch, roll, heading; //t1 is initial time
    List<List<float>> alldata = new List<List<float>>();
    float scale = 1.0f;
    //string textFilex = "Local time is: 7:17:28, 20-9-20015\nTime Latitude    Longitude BaroAlt Roll Pitch   Heading MachNo  CAS G   AOA TrueAirSpeed    IAS AGL BRM Radar Range\n\n8247.44	38.572398	106.058517	5046.6	0.94	11.24	29.88	0.54	   0	1.14	0.13	 181	 157	 0.0	0	999.6	\n8247.64	38.572742	106.058689	5059.9	0.90	11.35	29.90	0.54	   0	1.19	0.09	 181	 157	 0.0	0	999.6	\n8247.84	38.573085	106.058689	5076.6	0.83	11.44	29.93	0.54	   0	1.21	0.05	 181	 157	 0.0	0	999.6	\n8248.04	38.573428	106.058861	5089.9	0.74	11.54	29.95	0.54	   0	1.19	0.03	 181	 157	 0.0	0	999.6	\n8248.24	38.573600	106.059032	5106.6	0.67	11.61	29.97	0.54	   0	1.19	0.32	 181	 157	 0.0	0	999.6	\n8248.44	38.573943	106.059204	5119.9	0.65	11.66	29.99	0.54	   0	1.21	0.28	 181	 157	 0.0	0	999.6	";

    // Use this for initialization

    void Start()
    {
        //[[8247.4,38.123,123,123,123,123],[8247.6,123,123,123,345,345]]

        string text = textFile.text;
        string[] data = text.Split('\n').Skip(3).ToArray();

        foreach (var item in data)
        {

            string[] datapoints = item.Split('\t');
            List<float> rdata = new List<float>();
            int maxFields = 0;
            foreach (var datapoint in datapoints)
            {

                float num1;
                bool res = float.TryParse(datapoint, out num1);
                if (res)
                {

                    rdata.Add(float.Parse(datapoint));
                    maxFields++;

                    if (maxFields >= 7)
                    {
                        break;
                    }

                }
            }
            //Data Extraction
            alldata.Add(rdata);
        }
        //startPosition = targetPosition = gameObject.transform.localPosition;
        //startRotation = targetRotation = transform.localRotation;
        //t2 = alldata[0][0] - 2;
        //initialPosition = transform.position;
        setInitialpos();

    }
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }
    void FlagOn()
    {
        flag = true;
    }

    public static bool AlmostEqual(Vector3 v1, Vector3 v2, float precision)
    {
        bool equal = true;

        if (Mathf.Abs(v1.x - v2.x) > precision) equal = false;
        if (Mathf.Abs(v1.y - v2.y) > precision) equal = false;
        if (Mathf.Abs(v1.z - v2.z) > precision) equal = false;

        return equal;
    }

    public static bool CheckR(Quaternion v1, Quaternion v2)
    {

        return Quaternion.Angle(v1, v2) < 2;
    }

    public void setInitialpos()
    {
        x = (alldata[0][2] - 100) * 100; //long is x, lat is y
        y = (alldata[0][1] - 38) * 100;
        //x = data[2];
        //y = data[1];
        z = alldata[0][3];
        targetPosition = new Vector3(x, y, z);

        //This is correct and verified
        roll = alldata[0][4];
        pitch = alldata[0][5];
        heading = alldata[0][6];
        targetRotation = Quaternion.Euler(pitch, heading, roll);

        startPosition = targetPosition = gameObject.transform.localPosition;
        startRotation = targetRotation = transform.localRotation;
        t2 = alldata[0][0] - 2;
        initialPosition = transform.position;
        alldata.RemoveAt(0);
    }

    // Update is called once per frame
    void Update()
    {
        //Scaling the aircraft
        if (Input.GetKey(KeyCode.Equals))
        {
            scale += 0.01f;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        if (Input.GetKey(KeyCode.Minus))
        {
            scale -= 0.01f;
            if (scale < 0.01f)
                scale = 0.01f;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        //speed increment and decrement 
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Time.timeScale += 0.04f;
            if (Time.timeScale > 100)
                Time.timeScale = 100;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Time.timeScale -= 0.02f;
            if (Time.timeScale < 0.2)
                Time.timeScale = 0;
        }

        //Object Position and rotation Updated
        if (flag)
        {



            if (alldata.Count != 0)
            {
                // Debug.Log(deltat);
                //AlmostEqual(objectPosition, transform.position, 5f) & (CheckR(target, transform.rotation))
                if ((transform.localRotation == targetRotation) & (transform.localPosition == targetPosition))
                //    if ((transform.position == targetPosition))
                {
                    Debug.Log("Updating position");

                    List<float> data = alldata[0];
                    alldata.RemoveAt(0);
                    t1 = t2;
                    t2 = data[0];


                    x = (data[2] - 100) * 100; //long is x, lat is y
                    y = (data[1] - 38) * 100;
                    //x = data[2];
                    //y = data[1];
                    z = data[3];


                    //This is correct and verified
                    roll = data[4];
                    pitch = data[5];
                    heading = data[6];
                    targetRotation = Quaternion.Euler(pitch, heading, roll);


                    deltat = 0;
                    startPosition = gameObject.transform.localPosition;
                    startRotation = gameObject.transform.localRotation;
                    timeToReachTarget = 0.2f;
                    targetPosition = new Vector3(x, y, z);
                    //Vector3.RotateTowards(transform.forward, targetPosition, 0.2f * Time.deltaTime, 0.0f);
                    // transform.rotat(targetPosition);
                    //transform.rotation = Quaternion.LookRotation(targetPosition);
                }
                //gameObject.transform.position += transform.forward * Time.deltaTime * 2;
                deltat += Time.deltaTime / timeToReachTarget;
                gameObject.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, deltat);
                transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, deltat);

            }
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.localPosition += transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.localPosition -= transform.forward;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(0.0f, -5.0f, 0.0f);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(0.0f, 5.0f, 0.0f);
            }
        }

    }
}
