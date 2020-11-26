using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Globalization;
using UnityEngine.Networking;

public class TimeControl : NetworkBehaviour
{

    [SerializeField]
    public bool start = false;
    public bool pause = false;
    public bool reverse = false;
    public static bool reset = false;



    public GameObject Aircraft_parent;
    public GameObject time;
    public GameObject sliderTime;

    DateTime starttime = DateTime.Parse("23:59:59");
    DateTime endtime = DateTime.Parse("00:00:00");
    DateTime gametime;
    bool increment = false;
    bool decrement = false;
    List<movement> planeChildArr;

    int highestAllDataCount = 0;
    private string originPlaneName;
    private string endPlaneName;
    private Text local, startt, end; 

    private double gameEndTimeInSeconds = 0;
    [Range(0.001f, 1f)]
    public float Time_Control;
   // private float diffMoved;
    private int offset;
    private bool timeControlChanged = false;
    private float startValueOfTimeControl;
    public float sliderval;
    private float startvalueSlider;

    string compiledStartTimeFromUI;

    public void StartManual()
    {
        local = time.transform.GetChild(0).GetComponent<Text>();
        startt = time.transform.GetChild(1).GetComponent<Text>();
        end = time.transform.GetChild(2).GetComponent<Text>();
        
       //Debug.Log(gametime);
        planeChildArr = new List<movement>();
        GameObject originPlaneObject;

        foreach (Transform child in Aircraft_parent.transform)
        {
            movement Movement = child.gameObject.GetComponent<movement>();
            planeChildArr.Add(Movement);
			//get start time from UI
			//get changed start time

            string temp = Movement.GetStartTime(); // g
            string temp2 = Movement.GetEndTime();
			//if UI Value == temp 
           

            string PlaneNameAfterUIInput = PlayerPrefs.GetString("ActvePlaneName").ToString();

            //int temp3 = Movement.GetAllDataLength();
            //Debug.Log(child.name + " " + temp3);


            if (starttime > DateTime.Parse(temp))
            {
                starttime = DateTime.Parse(temp);

                originPlaneName = child.name;
                originPlaneObject = child.gameObject;
            }

            if (originPlaneName == PlaneNameAfterUIInput)
            {

                compiledStartTimeFromUI = PlayerPrefs.GetFloat("StartHour").ToString() + ":" + PlayerPrefs.GetFloat("StartMinute").ToString() + ":" + PlayerPrefs.GetFloat("StartSecond").ToString();
                // set starttime value in temp variable // 

                starttime = DateTime.Parse(compiledStartTimeFromUI);

                // set starttime value in temp variable // 

                //Debug.Log("orignal Plane Name " + originPlaneName);
                //Debug.Log("Plane Name From UI Input " + PlaneNameAfterUIInput);
                //Debug.Log("cheeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeck kro starttime variable ka time jismy playerpref ka saved updated time hy " + starttime);
                //Debug.Log("cheeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeck kro temp variable ka time jismy orignal time set kia hy " + temp);


            }

            if (endtime < DateTime.Parse(temp2))
            {
                endtime = DateTime.Parse(temp2);
                endPlaneName = child.name;

            }

            
           

            child.gameObject.SetActive(false);


            //if(highestAllDataCount < temp3)
            //{
            //    highestAllDataCount = temp3;

            //}



        }


        foreach (Transform child in Aircraft_parent.transform)
        {
            string childTime = child.gameObject.GetComponent<movement>().GetStartTime();

            double time = (DateTime.Parse(childTime) - starttime).TotalSeconds;

            offset = Mathf.CeilToInt((float)time / 0.2f);

            int temp3 = child.gameObject.GetComponent<movement>().GetAllDataLength();
            if (highestAllDataCount < temp3 + offset)
            {
                highestAllDataCount = temp3 + offset;

            }

            child.gameObject.GetComponent<movement>().SetOffsetTime(offset);
        }

        //Duration();
        gametime = starttime;
        startvalueSlider = GetSliderValue();
        
        local.text = "Local Time:\n" + gametime.ToString("HH:mm:ss");
        startt.text = starttime.ToString("HH:mm:ss");
        end.text=endtime.ToString("HH:mm:ss");
        InstantiateSlider();

    }

    public float GetSliderValue()
    {
        return sliderTime.GetComponent<Slider>().value;
    }

    
    public void InstantiateSlider()
    {
        //Time_Control is current slider value
        //slider start value should be 0.01
        startValueOfTimeControl = Time_Control;
        originPlaneName = GetOriginPlaneName();
        endPlaneName = GetEndPlaneName();

        //Debug.Log("Start Time : " + starttime);
        //Debug.Log("End Time : " + endtime);
        //End Time Calculations
        int highestAllDataCounter = GetHighestAllDataCount();

        TimeSpan totalLength = GetEndTime() - GetStartTime();
        //Debug.Log("This is the total length of game " + totalLength);
        gameEndTimeInSeconds = highestAllDataCounter * 0.2f;//totalLength.TotalSeconds;
        //Debug.Log("This is the total length of game CALCULATED USING SAMPLE LENGTH " + gameEndTimeInSeconds);
       // diffMoved = Time.fixedDeltaTime / (float)gameEndTimeInSeconds;
        //Debug.Log(diffMoved);
    }


    public void JumptoEvent(float value)
    {
        sliderTime.GetComponent<Slider>().value = value;
    }

    public void JumptoTime()
    {
        //sliderval = Time_Control;//sliderTime.gameObject.GetComponent<HoloToolkit.Examples.InteractiveElements.SliderGestureControl>().getSliderValue(); //get value of slider
        sliderval = sliderTime.GetComponent<Slider>().value;
        float differenceMoved = 0;
        // TrailControl.crTrail = false;
        if (startValueOfTimeControl != sliderval)
        {
            timeControlChanged = true;
            differenceMoved = sliderval - startValueOfTimeControl;
            startValueOfTimeControl = sliderval;
        }

        if (timeControlChanged)
        {
            //For each aircraft
            //Set Cad to what it was on given time
            //Minus offset
            TrailControl.crTrail = true;
            //for every aircraft in movement
            originPlaneName = GetOriginPlaneName();
            foreach (Transform child in Aircraft_parent.transform)
            {

                if (child.gameObject.activeSelf)
                {

                    child.GetComponent<movement>().SetCounterAllDataBySeconds(sliderval * gameEndTimeInSeconds);
                }

            }
            float differece = (float)differenceMoved * (float)gameEndTimeInSeconds;

            UpdateGameTime(differece);
            timeControlChanged = false;


        }

    }

    public int GetHighestAllDataCount()
    {
        return highestAllDataCount;
    }
    //public void SetGameTime(DateTime time)
    //{
    //    gametime = time;

    //}


    public void UpdateGameTime(double seconds)
    {

        gametime = gametime.AddSeconds(seconds);
        //sliderval //update value of slider

    }

    public string GetOriginPlaneName()
    {
        return originPlaneName;
    }

    public string GetEndPlaneName()
    {
        return endPlaneName;
    }

    public DateTime GetEndTime()
    {
        return endtime;
    }

    public DateTime GetStartTime()
    {
        return starttime;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        JumptoTime();
        if (increment)
        {
            IncreaseGameTime();
            CheckPlaneSpawnTime();
            sliderTime.GetComponent<Slider>().value += (Time.fixedDeltaTime / (float)gameEndTimeInSeconds);
            startValueOfTimeControl = sliderTime.GetComponent<Slider>().value;

            
            //sliderTime.GetComponent<Slider>().value= Time_Control;
        }
        if (decrement)
        {
            DecreaseGameTime();
            CheckPlaneSpawnTime();
            sliderTime.GetComponent<Slider>().value -= (Time.fixedDeltaTime / (float)gameEndTimeInSeconds);
            startValueOfTimeControl = sliderTime.GetComponent<Slider>().value;
        }
        if (reset)
        {
            MissionPause();
            reset = false;
        }
        if (start)
        {
            startvalueSlider = GetSliderValue();
            Debug.Log(startvalueSlider);
            MissionStart();
            start = false;
            SetPlayDirection(0);
        }
        if (pause)
        {
            MissionPause();
            pause = false;
        }
       
        if (reverse)
        {
            MissionReverse();
            reverse = false;
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("The left button was pressed" + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //}
        //files();
    }

    public long Duration()
    //total time of journey in seconds
    {
        Debug.Log((endtime - starttime).TotalSeconds.ToString());
        return (long)(endtime - starttime).TotalSeconds;
    }

    public void SetPlayDirection(int i)
    {
        if (i == 0)
        {
            decrement = false;
            increment = true;
        }
        else if (i == 1)
        {
            decrement = true;
            increment = false;
        }
        else
        {
            decrement = false;
            increment = false;
        }
    }

    void IncreaseGameTime()
    {

        gametime = gametime.AddSeconds(Time.fixedDeltaTime);
        local.text = "Local Time:\n" + gametime.ToString("HH:mm:ss");
        //Debug.Log(Time.fixedDeltaTime);
    }
    void DecreaseGameTime()
    {

        gametime = gametime.AddSeconds(-Time.fixedDeltaTime);
        local.text = "Local Time:\n" + gametime.ToString("HH:mm:ss");
        //Debug.Log(Time.fixedDeltaTime);
    }

    public void MissionReset()
    {
        //error.GetComponent<TextMesh>().text = "Debug: restarting";
        
        //gametime = starttime;
        //local.text = "Local Time:\n" + gametime.ToString("HH:mm:ss");
        ////foreach (Transform child in Aircraft_parent.transform)
        ////{
        ////    if (child.gameObject.activeInHierarchy)
        ////    {
        ////        child.gameObject.SetActive(false);
        ////        child.gameObject.GetComponent<movement>().ResetObject();
        ////    }
        ////}

        ////error.GetComponent<TextMesh>().text = "Debug: restart done";
        //JumptoEvent(startvalueSlider);
        //Debug.Log(startvalueSlider);
        //foreach (movement x in planeChildArr)
        //{
        //    x.ResetObject();
        //}
      

        //Clear All Reference to reference
        GameObject.Find("AircraftRefManager").GetComponent<ReferenceSelection>().ClearAllConnections();
       
        ////Clear all bulls eye
        GameObject.Find("BullsEyeManager").GetComponent<BullEyeHandle>().OnClearAllClick();

        //Zoom to default
        gameObject.GetComponent<ZoomCollection>().ResetZoom();


        //Speed default
        gameObject.GetComponent<TrailControl>().ResetSpeed();
        //Scale default
        gameObject.GetComponent<TrailControl>().ResetScale();

        //Exercise area disable

        //Dead symbol clear
        //

        sliderTime.GetComponent<Slider>().value = 0;

        reset = true;
}

    public void MissionReverse()
    {
        SetPlayDirection(1);
        foreach (movement x in planeChildArr)
        {
            x.movementDirection(false);
        }

    }
    public void setStart()
    {
        start = true;                                                                                                                                                                                                                                                                   
    }

    public void MissionPause()
    {
        //foreach (Transform child in Aircraft_parent.transform)
        //{
        //    if (child.gameObject.activeInHierarchy)
        //    {
        //        child.gameObject.GetComponent<movement>().FlagOff();
        //        child.gameObject.GetComponent<movement>().setPaused(true);
        //    }

        //}
        foreach (movement x in planeChildArr)
        {
            if (x.gameObject.activeInHierarchy)
            {
                x.FlagOff();
                x.setPaused(true);
            }
        }
        //increment = false;
        //decrement = false;
        SetPlayDirection(2);
    }

    public void MissionStart()
    {
        //error.GetComponent<TextMesh>().text = "Debug: inside MissionStart";
        increment = true;

        foreach (movement x in planeChildArr)
        {
            x.FlagOn();
            x.setPaused(false);
            x.movementDirection(true);
            x.PlaneInfoOff();
        }
    }

    void CheckPlaneSpawnTime()
    {

        foreach (movement script in planeChildArr)
        {

            string temp = script.GetStartTime();

            // string temp2 = "10:50:45";

            //Debug.Log("start time from file check this this this this!!! " + temp);

            if (gametime >= DateTime.Parse(temp)) //if gametime is more than
            {
                script.StartReal(true);
            }

            if (gametime <= DateTime.Parse(temp))
            {
                script.StartReal(false);

            }
        }
    }

}


