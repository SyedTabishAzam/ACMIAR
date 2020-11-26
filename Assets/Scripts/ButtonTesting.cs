using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class ButtonTesting : MonoBehaviour {

    public GameObject MetaCameraRig;
    public GameObject InpModule;
    public GameObject Camera;
    public GameObject Everything;
    public GameObject ImageLocker;
    public GameObject ARCamera;
    public GameObject MetaCanvas;
    public GameObject EventCamera;

    public bool StartButton = false;
    private bool startCheck = false;

    //public bool StopButton = false;
    public bool PauseButton = false;
    private bool pauseCheck = false;

    public bool ResetButton = false;
    private bool resetCheck = false;
    public bool SelectBullsEye = false;
   // public bool CreateNewBullsEye = false;
    public bool ClearBullsEye = false;
    public bool DrawExerciseArea = false;
    public bool ClearExerciseArea = false;
    public bool SpeedUpButton = false;
    public bool ZoomInButton = false;
    public bool ZoomOutButton = false;
    public bool Pan = false;
    public string PanDirection;
    public bool ScaleUpButton = false;
    public bool IncrementButton = false;
    public bool DecrementButton = false;
    public bool IncRow = false;
    public bool decRow = false;
    public bool AddNewRefConnection = false;
    public bool RemoveConnection = false;


    public bool RotateLeft = false;
    public bool RotateRight = false;
    public bool AddNewBullseyeConnection = false;
    public bool RemoveBullseyeConnection = false;
    public bool CloseButton = false;
    public bool CheckDeadSymnbol = false;

    public int rings;
    public string state;

    public GameObject BullseyeRowToDel;
    public GameObject rowToDel;
    public GameObject DrawAreaManager;
    public GameObject AircraftRefManager;
    public GameObject GameManager;
    public GameObject dashboadManager;
    public GameObject referenceManager;
    public GameObject bullseyeManager;
    public GameObject exerciseAreaManager;
    public GameObject AirCraftParent;
    public GameObject datadashboard;
    public GameObject DeadSymbolPanel;
    public GameObject TerrainParent;

    public int MetaMode = 1;
    private bool callFixedUpdate = false;
   // private string originPlaneName;
   // private string endPlaneName;
    private double gameEndTimeInSeconds = 0;
    DateTime startTime;
    DateTime endTime;
    [Range(0.001f, 1f)]
    public float TimeControl;
    private bool timeControlChanged = false;
    private float startValueOfTimeControl;
    // Use this for initialization

    private void Awake()
    {
        if (MetaMode == 1)
        {
            ToggleMetaOptions();
        }
        else if (MetaMode == 0)
        {
            ToggleSimpleOptions();
        }
    }

    void Start () {
        startValueOfTimeControl = TimeControl;
       // originPlaneName= GameManager.GetComponent<TimeControl>().GetOriginPlaneName();
       // endPlaneName = GameManager.GetComponent<TimeControl>().GetEndPlaneName();
     

        //End Time Calculations
        startTime = GameManager.GetComponent<TimeControl>().GetStartTime();
        endTime = GameManager.GetComponent<TimeControl>().GetEndTime();
        int highestAllDataCounter = GameManager.GetComponent<TimeControl>().GetHighestAllDataCount();

        //TimeSpan totalLength = endTime - startTime;
        //Debug.Log("This is the total length of game " + totalLength);
        gameEndTimeInSeconds = highestAllDataCounter * 0.2f;//totalLength.TotalSeconds;
        //Debug.Log("This is the total length of game CALCULATED USING SAMPLE LENGTH " + gameEndTimeInSeconds);

    }

    void ToggleMetaOptions()
    {
        MetaCameraRig.SetActive (true);
        InpModule.SetActive(true);
        Camera.GetComponent<CameraZoom>().enabled = false;
        ImageLocker.SetActive(true);
        Everything.GetComponent<MoveTerrain>().enabled = true;
        Camera.GetComponent<Camera>().targetDisplay = 1;
        ARCamera.GetComponent<Camera>().targetDisplay = 0;
        MetaCanvas.GetComponent<Canvas>().worldCamera = EventCamera.GetComponent<Camera>();
        bullseyeManager.GetComponent<BullEyeHandle>().isSimple = false;
        Camera.GetComponent<AudioListener>().enabled = false;
        GameManager.GetComponent<ZoomCollection>().SetMeta(true);
    }

    void ToggleSimpleOptions()
    {
        MetaCameraRig.SetActive(false);
        InpModule.SetActive(false);
        Camera.SetActive(true);
        Camera.GetComponent<CameraZoom>().enabled = true;
        Everything.GetComponent<MoveTerrain>().enabled = false;
        ImageLocker.SetActive(false);
        Camera.GetComponent<Camera>().targetDisplay = 0;
        ARCamera.GetComponent<Camera>().targetDisplay = 1;
        MetaCanvas.GetComponent<Canvas>().worldCamera = Camera.GetComponent<Camera>() ;
        bullseyeManager.GetComponent<BullEyeHandle>().isSimple = true;
        Camera.GetComponent<AudioListener>().enabled = true;
        GameManager.GetComponent<ZoomCollection>().SetMeta(false);
    }

    // Update is called once per frame
    void Update () {

      

        //DrawAreaManager.GetComponent<ClippingPlane>().ChangeState(state);
        

        if (AddNewRefConnection)
        {
            AircraftRefManager.GetComponent<ReferenceSelection>().AddNew();
            AddNewRefConnection = false;
        }

        if(RemoveConnection)
        {
            if(rowToDel)
            {

                AircraftRefManager.GetComponent<ReferenceSelection>().RemoveSelected(rowToDel);
                RemoveConnection = false;
            }

        }

        if (AddNewBullseyeConnection)
        {
            bullseyeManager.GetComponent<BullEyeHandle>().OnNewButtonClick();
            AddNewBullseyeConnection = false;
        }

        if (RotateLeft)
        {
            TerrainParent.GetComponent<SmoothRotation>().Rotate("01");
            RotateLeft = false;
        }

        if (RotateRight)
        {
            TerrainParent.GetComponent<SmoothRotation>().Rotate("11");
            RotateRight = false;
        }

        if (RemoveBullseyeConnection)
        {
            if (BullseyeRowToDel)
            {

                bullseyeManager.GetComponent<BullEyeHandle>().OnClearSignClick(BullseyeRowToDel);
                RemoveBullseyeConnection = false;
            }

        }
        if (CheckDeadSymnbol)
        {
            DeadSymbolPanel.GetComponent<DeadSymbol>().OnSelectAircraftClick();
            CheckDeadSymnbol = false;
        }

        if (CloseButton)
        {
            bullseyeManager.GetComponent<BullEyeHandle>().DeleteDangling();
         
            CloseButton = false;
        }
        float differenceMoved = 0;
       // TrailControl.crTrail = false;
        if (startValueOfTimeControl != TimeControl)
        {
            timeControlChanged = true;
            differenceMoved = TimeControl - startValueOfTimeControl;
            startValueOfTimeControl = TimeControl;
        }

		if(StartButton && !startCheck)
        {
            SelectOnStartButtonEvent();
            callFixedUpdate = true;
        }

        if (!StartButton)
            startCheck = false;

        if (PauseButton && !pauseCheck)
        {
            SelectOnPauseButtonEvent();
            callFixedUpdate = true;
        }

        if (!PauseButton)
            pauseCheck = false;

        if(ResetButton && !resetCheck)
        {
            SelectOnResetButtonEvent();
            
        }

        if (!ResetButton)
            resetCheck = false;

        if(SpeedUpButton)
        {
            SelectOnSpeedUpEvent();
        }
        if (ZoomInButton)
        {
            SelectOnZoomInEvent();
        }
        if (ZoomOutButton)
        {
            SelectOnZoomOutEvent();
        }
        if (ScaleUpButton)
        {
            SelectOnScaleUpEvent();
        }
        if (IncrementButton)
        {
            IncrementIndexTest();
            IncrementButton = false;
        }

        if (DecrementButton)
        {
            DecrementIndexTest();
            DecrementButton = false;
        }
        //int originPlaneCount = 0;
        if(timeControlChanged)
        {
            //For each aircraft
            //Set Cad to what it was on given time
            //Minus offset
            TrailControl.crTrail = true;
            //for every aircraft in movement
            //originPlaneName = GameManager.GetComponent<TimeControl>().GetOriginPlaneName();
            foreach (Transform child in AirCraftParent.transform)
            {
              
                //if (child.name == originPlaneName)
                //{
                   
                //    GameManager.GetComponent<TimeControl>().SetGameTime(child.GetComponent<movement>().GetLocalTime());
                //}

                
                
                if(child.gameObject.activeSelf)
                {
                    
                    child.GetComponent<movement>().SetCounterAllDataBySeconds(TimeControl * gameEndTimeInSeconds);
                }

    
                //float t = curTime * 24f;
                //float hours = Mathf.Floor(t);
                //t *= 60;
                //float minutes = Mathf.Floor(t % 60);
                //t *= 60;
                //float seconds = Mathf.Floor(t % 60);
                //Debug.Log(hours + ":" + minutes + ":" + seconds);

                //GameManager.GetComponent<TimeControl>().SetGameTime(DateTime.Parse();




            }
            float differece = (float)differenceMoved * (float)gameEndTimeInSeconds;
            //Debug.Log(differece);
            GameManager.GetComponent<TimeControl>().UpdateGameTime(differece);
            timeControlChanged = false;


        }

     

        if(SelectBullsEye)
        {

            bullseyeManager.GetComponent<BullEyeHandle>().OnSelectButtonClick();
            SelectBullsEye = false;
        }


        if (ClearBullsEye)
        {
            bullseyeManager.GetComponent<BullEyeHandle>().ClearBullsEye();
            ClearBullsEye = false;
        }

        //bullseyeManager.GetComponent<BullEyeHandle>().GenerateRings(rings);

        if(DrawExerciseArea)
        {

            exerciseAreaManager.GetComponent<ExerciseAreaManager>().SetShowExerciseAreaTest(true);
            DrawExerciseArea = false;
        }
        if(ClearExerciseArea)
        {
            exerciseAreaManager.GetComponent<ExerciseAreaManager>().SetShowExerciseAreaTest(false);
            ClearExerciseArea = false;
        }

        if(IncRow)
        {
            referenceManager.GetComponent<DashboardManager>().IncreaseRows();
            IncRow = false;
        }

        if (decRow)
        {
			referenceManager.GetComponent<DashboardManager>().DecreaseRows();
            decRow = false;
        }

        if(Pan)
        {
            GameManager.GetComponent<ZoomCollection>().PanContinuous(PanDirection);
            Pan = false;
        }


    }

    public void FixedUpdate()
    {
        //if(callFixedUpdate)
        //{

        //    TimeControl += (Time.deltaTime / (float)gameEndTimeInSeconds);
        //    Debug.Log(gameEndTimeInSeconds);
        //    startValueOfTimeControl = TimeControl;
        //}
    }

    public void IncrementIndexTest()
    {
        dashboadManager.GetComponent<DashboardManager>().IncreaseIndex();
    }

    public void DecrementIndexTest()
    {
        dashboadManager.GetComponent<DashboardManager>().DecreaseIndex();
    }
    void SelectOnSpeedUpEvent()
    {
        GameManager.GetComponent<TrailControl>().SpeedPrefab(0);
        SpeedUpButton = false;
    }
    void SelectOnZoomInEvent()
    {
        GameManager.GetComponent<ZoomCollection>().RpcZoomIn();
        GameManager.GetComponent<TrailControl>().ClearTrail();
        ZoomInButton = false;
    }

    void SelectOnZoomOutEvent()
    {
        GameManager.GetComponent<ZoomCollection>().RpcZoomOut();
        GameManager.GetComponent<TrailControl>().ClearTrail();
        ZoomOutButton = false;
    }

    void SelectOnScaleUpEvent()
    {
        GameManager.GetComponent<TrailControl>().ScalePrefab(0);
        ScaleUpButton = false;
    }
    void SelectOnStartButtonEvent()
    {
        GameManager.GetComponent<TimeControl>().setStart();
        startCheck = true;
    }

    void SelectOnResetButtonEvent()
       
    {
        GameManager.GetComponent<TimeControl>().MissionReset();
        dashboadManager.GetComponent<DashboardManager>().Reset();
        referenceManager.GetComponent<ReferenceManager>().Reset();
        bullseyeManager.GetComponent<BullEyeHandle>().ClearBullsEye();
        resetCheck = true;
    }

    void SelectOnPauseButtonEvent()

    {
        GameManager.GetComponent<TimeControl>().MissionPause();
        pauseCheck = true;
    }
}
