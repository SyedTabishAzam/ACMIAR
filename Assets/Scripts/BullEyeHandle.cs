using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BullEyeHandle : MonoBehaviour
{
    //[Range(0.00f, 2f)]
    public float horizontalMove;
    int counter = 0;

    //[Range(0.00f, 2f)]
    public float verticalMove;

    public InputField _degreeText;
    public InputField _numofcircleText;
    public InputField _distanceText;

    public Transform bullsEyePanel;
    public Slider latitudeSlider;
    public Slider longitudeSlider;
    public InputField latitudeInputField;
    public InputField longitudeInputField;
    public Slider ringSlider;
    public GameObject exerciseAreaFileParser;
    public bool doBullEye = false;
    public bool selectPoint = false;
    private Vector3 targetPosition;
    private bool isBullseyeComplete = true;
    private float maximumX = 10240;
    private float maximumY = 10240;
    private float metaMouseXwOfset = 0;
    private float metaMouseZwOfset = 0;
    private int currentNumber = 0;
    private int prevNumber = 0;

    public TextMesh ringsCount;
    public GameObject GameManager;
    public GameObject selectBullseyeText;
    public bool planB = false;
    public bool getValuesFromSlider = false;
    public GameObject selectedAircraft;
    public bool IsMarkerSettled = true;
    public GameObject metaCanvas;
    public GameObject DrawAreaManager;
    public GameObject BullsEyeRingPrefab;
    public GameObject bullEyeRingsSprite;
    public GameObject BullsEyeParent;
    private GameObject bullsEyeObject;
    public GameObject Aircraft_parent;
    public GameObject Terrain;
    public GameObject latitude, longitude;
    public GameObject ViewBullPanel;
    public GameObject xmlPanel;
    public ReferencingData refData;
    public Dropdown Ddown;
    public Text counterObj;
    List<List<float>> storedBullsEye;
    public bool isSimple = false;
    float zoomedInFactor = 1;
    bool tweakFromSliders;
    int val1 = 0;
    private Vector2 LatLong;
    Meta.Mouse.MetaMouse metaMouse;
    Scene activeScene;

    // Use this for initialization
    void Start()
    {       
        activeScene = SceneManager.GetActiveScene();

        tweakFromSliders = true;
        if (!isSimple)
        {
            metaMouse = GameObject.Find("MetaMouse(Clone)").GetComponent<Meta.Mouse.MetaMouse>();

            if (!metaMouse)
                getValuesFromSlider = true;
        }
        
       
        latitudeSlider.interactable = tweakFromSliders;
        longitudeSlider.interactable = tweakFromSliders;
        
    }

    public void StoreExererciseAreasBullseye(List<List<float>> list)
    {
        storedBullsEye = list;
        xmlPanel.GetComponent<PopulateFromXML>().PopulateRows();
    }

    public List<List<float>> GetXMLData()
    {
        return storedBullsEye;
    }


    public GameObject referencedObject()
    {
        Debug.Log("TEXT DROPDOWN ::" + Ddown.GetComponentInChildren<Text>().text);

        string selected = Ddown.GetComponentInChildren<Text>().text; //Mairage1
        GameObject selectedObj = null;

        if (activeScene.name == "ARScene")
        {
            foreach (Transform child in Aircraft_parent.transform)
            {
                if (child.gameObject.GetComponent<movement>().getCallSign() == selected)//check with childs callsign
                                                                                        //if(child.gameObject.GetComponent<movement>().getCallSign() == selected)
                {
                    selectedObj = child.gameObject;
                }
            }
        }

        else if (activeScene.name == "LiveData")
        {
            foreach (Transform child in Aircraft_parent.transform)
            {
                string[] nameOfChildAircraft = child.gameObject.name.Split(' ');
                //Debug.Log("tail id of active aircraft is = " + nameOfChildAircraft[1]);
                if (nameOfChildAircraft[1] == selected)
                {
                    selectedObj = child.gameObject;

                }
            }
        }

        return selectedObj;
    }


    public void ActivateBullseyePanel()
    {
        InstantiateBullseyeMarker();
    }

    // Update is called once per frame
    void Update()
    {
        //Go here only if there is a bullseye object still in limbo
        if (bullsEyeObject && !isBullseyeComplete)
        {
            //Go only if the marker is in moving state
            if (!IsMarkerSettled)
            {
               
                MovePointerWithMouse();

                //Value of Horizontal move and Vertical move is value of metaMouse OR value got from Simple Mouse
                RecalibrateValues();

                //Get lat long from position of bullsEye object and store in var LatLong (Hm Vm is from 0 to 2)
                GetLatLongFromHmVm(bullsEyeObject.transform.localPosition.x, bullsEyeObject.transform.localPosition.z);
                UpdateLabels();

                //Convert from Hm Vm to position
                bullsEyeObject.transform.position = new Vector3(horizontalMove, bullsEyeObject.transform.localPosition.y, verticalMove);

                //Clamp in the local space
				bullsEyeObject.transform.localPosition = new Vector3 (Mathf.Clamp (bullsEyeObject.transform.localPosition.x, 0, 2), 0, Mathf.Clamp (bullsEyeObject.transform.localPosition.z, 0, 2));

                //Make slider values the value between 0 to 2 (Value from bullseye object)
                longitudeSlider.value = bullsEyeObject.transform.localPosition.x;
                latitudeSlider.value = bullsEyeObject.transform.localPosition.z;

                if ( Input.GetMouseButtonDown(0))
                {
                    //On mouse click save the gathered latLong
                    OnMouseButtonClick();
                    bullsEyePanel.gameObject.SetActive(true);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    DeleteDangling();
                    bullsEyePanel.gameObject.SetActive(true);
                }

            }

        }

        if(bullsEyeObject && tweakFromSliders)
        {

            if (IsMarkerSettled)
            {

                horizontalMove = longitudeSlider.value;
                verticalMove = latitudeSlider.value;
                GetLatLongFromHmVm(horizontalMove, verticalMove);
                UpdateLabels();
                bullsEyeObject.GetComponent<PositionData>().SetLatLong(LatLong);
               // Debug.Log(horizontalMove + " " + verticalMove);
                bullsEyeObject.transform.localPosition = new Vector3(horizontalMove, bullsEyeObject.transform.localPosition.y, verticalMove);
             }
        }

        //DrawAreaManager.SetActive(IsMarkerSettled);
        IsolateMarket(IsMarkerSettled);
       
       
    }

    private void RecalibrateValues()
    {
        zoomedInFactor = GameManager.GetComponent<ZoomCollection>().scale;

        //float offsetX = Mathf.Abs((zoomedInFactor-1) / 2f) - (GameManager.GetComponent<ZoomCollection>().zoomable.transform.localPosition.x/zoomedInFactor);
        horizontalMove = metaMouseXwOfset;// (((1f / (zoomedInFactor)) * metaMouseXwOfset) + offsetX);
        
        //float offsetZ = 0f - (GameManager.GetComponent<ZoomCollection>().zoomable.transform.localPosition.z / zoomedInFactor);
        verticalMove = metaMouseZwOfset;//((2f / (zoomedInFactor)) * metaMouseZwOfset) + offsetZ;
    }

  

    public void IsolateMarket(bool isMarkerSettled)
    {
        metaCanvas.SetActive(isMarkerSettled);
        selectBullseyeText.SetActive(!isMarkerSettled);
    }

    void MovePointerWithMouse()
    {
       // testing.GetComponent<MetaDebug>().Log(metaMouse.transform.position.ToString());
       
        if(isSimple)
        {
            Vector3 mc = Input.mousePosition;
            mc.z = 10f;
            Vector3 pos = Camera.main.ScreenToWorldPoint(mc);
            metaMouseXwOfset = pos.x;
            metaMouseZwOfset = pos.y;
        }
        else
        {

           if(!metaMouse)
                metaMouse = GameObject.Find("MetaMouse(Clone)").GetComponent<Meta.Mouse.MetaMouse>();
		    metaMouseXwOfset = metaMouse.transform.position.x;//Mathf.Clamp(metaMouse.transform.position.x + 0.5f, 0, 2);
		    metaMouseZwOfset = metaMouse.transform.position.z ;//Mathf.Clamp(metaMouse.transform.position.z - 0.96f, 0, 2);
        }
    }

    public void ClearBullsEye()
    {
        doBullEye = false;
        // selectedAircraft = null;
        ClearPanel();
        GetComponent<ReferenceCalculator>().ClearOneToOnePR();
        Destroy(bullsEyeObject);
    }

    public void ClearPanel()
    {
        Ddown.value = 0;
        latitude.GetComponent<TextMesh>().text = "NaN";
		longitude.GetComponent<TextMesh> ().text = "NaN";
		//ringSlider.value = 0;
        counterObj.text = 0.ToString();
    }

    public void InstantiateBullseyeMarker()
    {
        if(isBullseyeComplete)
        {
            ClearPanel();
            isBullseyeComplete = false;
            IsMarkerSettled = false;


            bullsEyeObject = Instantiate(BullsEyeRingPrefab, BullsEyeParent.transform);
            prevNumber = 0;
           
            bullsEyeObject.name = "bullseye" + " " + counter;
          //  Terrain.GetComponent<CoordinateConvertor>().ActivatePointRegistry(bullsEyeObject.name);
            counter++; 

        }
    }

    public void OnMouseButtonClick()
    {
        DoBullEyeCalculation();
        
        IsMarkerSettled = true;
    }

    public void OnGetFromExerciseAreaClick()
    {

        if (storedBullsEye != null)
        {
            if (storedBullsEye.Count == 1)
            {
                SelectFromXMLIndex(0);
            }
            else
            {
                xmlPanel.SetActive(true);
            }

        }
    }

    public void SelectFromXMLIndex(int i)
    {
        if(storedBullsEye!=null)
        {
            if (!bullsEyeObject)
            {
                InstantiateBullseyeMarker();
            }

            float lat = storedBullsEye[i][0];
            float lon = storedBullsEye[i][1];
            float noOfCir = storedBullsEye[i][2];

            LatLong.x = lat;
            LatLong.y = lon;
            DoBullEyeCalculation();
            
            bullsEyeObject.transform.localPosition = targetPosition;
            bullsEyeObject.GetComponent<PositionData>().SetLatLong(LatLong);
            GetLatLongFromHmVm(bullsEyeObject.transform.localPosition.x, bullsEyeObject.transform.localPosition.z);
            UpdateLabels();
            longitudeSlider.value = bullsEyeObject.transform.localPosition.x;
            latitudeSlider.value = bullsEyeObject.transform.localPosition.z;
            IsMarkerSettled = true;
            bullsEyePanel.gameObject.SetActive(true);
            
        }
    }

    public void OnSelectButtonClick()
    {
        //Draw line
        //Inc counter
        
        selectedAircraft = referencedObject();
        if(selectedAircraft && selectedAircraft.activeInHierarchy && IsMarkerSettled && bullsEyeObject)
        {
            
            refData.AddToBullseyeList(bullsEyeObject, selectedAircraft);
            counterObj.text = refData.GetLastBullseyeListCount().ToString();

            ViewBullPanel.GetComponent<PopulateAirRef>().PopulateBull();           //commented for test case
            isBullseyeComplete = true;
        }
    }

    public void OnNewButtonClick()
    {
        
        if(isBullseyeComplete)
        {
            InstantiateBullseyeMarker();
            ClearPanel();
            
            isBullseyeComplete = false;
            IsMarkerSettled = false;
        }
    }

    public void DeleteDangling()
    {
        if(!isBullseyeComplete)
        {
            Destroy(bullsEyeObject);

        }
        isBullseyeComplete = true;
        IsMarkerSettled = true;
    }

    public void OnClearSignClick(GameObject row)
    {
        if(row.name.Contains("bullseye"))
        {
            Debug.Log(row.name);
            CustomDict temp = refData.GetFromBullseyeList(row.name);
            Debug.Log(temp.GetKey().name);
            
            GetComponent<ReferenceCalculator>().ClearAllBullseyeLinesAndCalculations(temp);
            refData.RemoveFromBullseyeList(row.name);
            Destroy(BullsEyeParent.transform.Find(row.name).gameObject);
        }
        else
        {
            CustomDict temp = refData.GetFromBullseyeList(row.transform.parent.name);
            Debug.Log(row.name);
            Debug.Log(temp.GetKey());
            GameObject aircraft = refData.PopFromBullseyeSublist(row.transform.parent.name,row.name);
            GetComponent<ReferenceCalculator>().ClearLine(aircraft, temp.GetKey());
            GetComponent<ReferenceCalculator>().ClearCalculationsText(aircraft, temp.GetKey());
            if (temp.GetValues().Count <= 0)
            {
                if(bullsEyeObject)
                {
                    if(temp.GetKey().name ==bullsEyeObject.name)
                    {
                        DeleteDangling();
                    }
                }

                Destroy(BullsEyeParent.transform.Find(temp.GetKey().name).gameObject);
                refData.RemoveFromBullseyeList(temp.GetKey().name);
            }

        }
        counterObj.text = refData.GetLastBullseyeListCount().ToString();
        ViewBullPanel.GetComponent<PopulateAirRef>().PopulateBull();
    }

    public void OnClearAllClick()
    {
        List<GameObject> grandChildren = new List<GameObject>();
        foreach (Transform child in ViewBullPanel.transform.Find("Rows"))
        {
            Debug.Log(child.name);
            grandChildren.Add(child.GetChild(1).gameObject);

        }

        foreach (GameObject child in grandChildren)
        {
            child.GetComponent<ClearSignLogic>().InvokeButtonPressed();
        }
    }
    public void GenerateRings()
    {
        GameObject ring = Instantiate(bullEyeRingsSprite/*, bullsEyeObject.transform*/);
        
        ring.transform.parent = bullsEyeObject.transform;

        TangentCircles script = ring.GetComponentInChildren<TangentCircles>();
        script._degree = float.Parse(_degreeText.text);
        script._numberOfCircles = float.Parse(_numofcircleText.text);
        script._distanceBetweenCircles = float.Parse(_distanceText.text);
        script.drawCricleAndlines();

        ring.transform.localPosition = Vector3.zero;
        ring.transform.localRotation = Quaternion.identity;
        ring.transform.localScale = Vector3.one;
    }


    //public void GenerateRings1(Slider numberSlider)
    //{
    //    //Slider value is in KM
    //    int number = (int)numberSlider.value;
    //    ringsCount.text = number.ToString() + " NM";
    //    if (bullsEyeObject)
    //    {
    //         number = Mathf.Clamp(number, 0, 15);

    //        if (prevNumber < number)
    //        {
    //            for (int x = prevNumber; x < number; x++)
    //            {
    //                GameObject ring = Instantiate(bullEyeRingsSprite, bullsEyeObject.transform);
    //                //ring.GetComponent<SpriteRenderer>().color = Color.red;
    //                float meters = x * 1000;
    //                float distanceSkewedScale = meters * (ring.transform.localScale.x / MathCalculations.MaxBounds()[0]);
    //                //TODO - KM to NM conversion
    //                // Debug.Log(MathCalculations.MaxBounds()[0]);
    //                ring.transform.localScale = new Vector3(distanceSkewedScale, distanceSkewedScale, 0);
    //            }
    //            prevNumber = number;
    //        }
    //        else if (prevNumber > number)
    //        {
    //            while (prevNumber > number)
    //            {

    //                Destroy(bullsEyeObject.transform.GetChild(1 + (prevNumber - 1)).gameObject);
    //                prevNumber--;
    //            }
    //            prevNumber = number;
    //        }

    //    }
    //}


    public void MouseToBullsEye(float x, float y)
    {
        //Recieve a value from 0 to 2
        horizontalMove = x;
        verticalMove = y;
        bullsEyeObject.transform.parent = BullsEyeParent.transform;
        selectPoint = true;
      
    }

    public void GetLatLongFromHmVm(float horizontalMove, float verticalMove)
    {
        //Convert value on scale of 0 to 2 to Actual resolution
        float x = (horizontalMove / 2f) * maximumX;
        float y = (verticalMove / 2f) * maximumY;
      
        //Generate LatLong from the gained value
        LatLong = MathCalculations.XYtoLatLong(x, y);

    }

    private void UpdateLabels()
    {
        string textMaker = LatLong.x.ToString() + "N " + LatLong.y.ToString() + "W";
        bullsEyeObject.transform.GetChild(0).GetComponent<TextMesh>().text = textMaker;
        //latitude.GetComponent<TextMesh>().text = LatLong.x.ToString();
        //longitude.GetComponent<TextMesh>().text = LatLong.y.ToString();
        //latitude.GetComponent<TextMesh>().text = DecimalDegToDMS(LatLong.x);
        //longitude.GetComponent<TextMesh>().text = DecimalDegToDMS(LatLong.y);

        latitudeInputField.text = DecimalDegToDMS(LatLong.x);
        longitudeInputField.text = DecimalDegToDMS(LatLong.y);
        
    }

    public void DoBullEyeCalculation()
    {
        float altitude = 1000;
        float[] xzy = MathCalculations.Convert(LatLong.x, LatLong.y, altitude); //lat long

        float x = xzy[0];
        float z = xzy[1];
       //float y = xzy[2];
        targetPosition = new Vector3(x, 0, z);

        bullsEyeObject.GetComponent<PositionData>().SetLatLong(LatLong);
    }

    public Vector2 GetLatLong()
    {
        return LatLong;
    }

    public string DecimalDegToDMS(float decimalDegree)
    {
        // set decimal_degrees value here
        double degree = Math.Truncate(decimalDegree);
        double minutes = (decimalDegree - Math.Floor(decimalDegree)) * 60.0;
        double seconds = (minutes - Math.Floor(minutes)) * 60.0;
        //double tenths = (seconds - Math.Floor(seconds)) * 10.0;


        // get rid of fractional part
        minutes = Math.Floor(minutes);
        seconds = Math.Floor(seconds);
        //tenths = Math.Floor(tenths);

        string DMS = degree + "°" + minutes + "'" + seconds + "\"";

        return DMS;
    }
}
