using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReferenceCalculator : MonoBehaviour {

    //List<List<float>> point = new List<List<float>> { };
    private bool instantiated = false;
    public bool isParameterReference;
    public GameObject line;
    public GameObject lineParent;
    public Transform displayParent;
    public GameObject aircraftParent;
    public GameObject DisplayInfo;

    List<GameObject> lines;
    private Transform ref1Transform;
    private Transform ref2Transform;
    public GameObject drawAreaManager;
    public ReferencingData refData;
	//public GameObject everyThing;
 //   private List<CustomTuple> refList;
    private GameObject DisplayText;
    LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
       
        lineRenderer = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if(name== "AircraftRefManager")
        {

            foreach(CustomTuple connection in refData.GetRefList())
            {
                OneToOneParameterReference(connection.GetFirst(), connection.GetSecond());
            }

        }

        if(name== "BullsEyeManager")
        {
            foreach (CustomDict bullseyeCon in refData.GetBullseyeList())
            {
                foreach(GameObject aircrafts in bullseyeCon.GetValues())
                {
                    OneToOneParameterReference(aircrafts, bullseyeCon.GetKey(), true);
                }
            }
        }

    }

    void DrawLine(LineRenderer line= null)
    {
        //This method draws the line from ref1 to ref2
        
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, ref1Transform.position);
            lineRenderer.SetPosition(1, ref2Transform.position);

    }
    
    Color DrawLine2(GameObject air1, GameObject air2)
    {
        string nameMaker = air1.name + "-" + air2.name;
        GameObject line1;
        LineRenderer currentLineRenderer;
        if (lineParent.transform.Find(nameMaker)== null)
        {
            line1 = Instantiate(line, lineParent.transform);
			line1.name = nameMaker;
            currentLineRenderer = line1.GetComponent<LineRenderer>();
            Color newColor = Random.ColorHSV();
            currentLineRenderer.material.color = newColor;
            currentLineRenderer.startWidth= 0.002f;
            currentLineRenderer.endWidth = 0.002f;
            drawAreaManager.GetComponent<ClippingPlane>().AddObjectToList(line1);
			drawAreaManager.GetComponent<ClippingPlane> ().ChangeState ("REFRESH");
        }
        else
        {
            line1 = lineParent.transform.Find(nameMaker).gameObject;
			//line1.transform.localRotation = Quaternion.Euler(new Vector3(line1.transform.localRotation.eulerAngles.x-lineParent.transform.rotation.eulerAngles.x,line1.transform.localRotation.eulerAngles.y-lineParent.transform.rotation.eulerAngles.y,line1.transform.localRotation.eulerAngles.z-lineParent.transform.rotation.eulerAngles.z));
            currentLineRenderer = line1.GetComponent<LineRenderer>();
        }
        //line1.transform.parent = GameObject.Find("ExerciseArea").transform;
        currentLineRenderer.positionCount = 2;
		currentLineRenderer.SetPosition(0, air1.transform.position);
		currentLineRenderer.SetPosition(1, air2.transform.position);
        
        // = new Material(Shader.Find("Custom\"));

        return currentLineRenderer.material.color;
    }

    void DisplayCalc(string name, GameObject refAircraft, List<string> retVal,Color drawColor, bool isBullseye)
    {
        if (refAircraft.transform.Find(name) == null)
        {
           DisplayText = Instantiate(DisplayInfo, refAircraft.transform);
           DisplayText.name = name;
        }
        else
        {
            DisplayText = refAircraft.transform.Find(name).gameObject;
        }


        DisplayText.transform.localPosition = Vector3.up * ((DisplayText.transform.GetSiblingIndex() - 1) * 0.02f) + (Vector3.up * 0.02f);
      //  DisplayText.transform.localScale = new Vector3(10f, 10f, 10f);
       // DisplayText.SetActive(true);

       
        if(isBullseye)
        {
            string textMaker = retVal[0] + " \\ " + retVal[1];
            DisplayText.GetComponent<TextMesh>().text = textMaker;
            DisplayText.GetComponent<TextMesh>().color = Color.red;

            //DisplayText.GetComponent<TextMesh>().color = Color.red;
        }
        else
        {
            string textMaker = retVal[2] + " \\ " + retVal[3];
            DisplayText.GetComponent<TextMesh>().text = textMaker;
           // DisplayText.GetComponent<SpriteTextFactory>().DrawText();
            DisplayText.GetComponent<TextMesh>().color = drawColor;
         }
        
    }

    private List<string> DoCalculations(GameObject ref1, GameObject ref2,bool isBullEye=false)
    {
        //Method to do all calculations in isolated mode and return a list
        List<string> calculations = new List<string>();
        if (isBullEye)
        {
            float aspectAngle = CalculateAngle(ref1, ref2);
            string unit = "Nm";
            double distance = CalculateDistance(ref1, ref2, unit,isBullEye);

            calculations.Add(aspectAngle.ToString("F1") + "DEG");
            calculations.Add(distance.ToString("F1") + unit);
        }
        else
        {

            //TODO - Add Units
            float aspectAngle = CalculateAngle(ref1, ref2);
            string unit = "Nm";
            double distance = CalculateDistance(ref1, ref2,unit);
            float bearing = CalculateBearing(ref1, ref2);
            double altitudeDifference = CalculateAltitudeDifference(ref1, ref2);
            double ROC = CalculateROC(ref1, ref2);
            //Todo - Add ROC Calculations
       

            //Converts all calculations to list
            calculations.Add(aspectAngle.ToString("F1") + "DEG");
            calculations.Add(altitudeDifference.ToString("F1") + "ft");
            calculations.Add(distance.ToString("F1") + unit);
            calculations.Add(bearing.ToString("F1") + "DEG");
            calculations.Add(ROC.ToString("F1") + "DEG");
            calculations.Add(distance.ToString("F1") + unit);
       
        }
        return calculations;
    }

    public void OneToOneParameterReference(GameObject ref1, GameObject ref2,bool isBullEye = false)
    {
        //This method will be called from another script passing Ref1 and Ref2 as parameters - tethered line will be drawn between these objects

        if(ref1.activeInHierarchy)
            ref1Transform = ref1.transform;

        if(ref2.activeInHierarchy)
            ref2Transform = ref2.transform;
      
        if(ref1.activeInHierarchy && ref2.activeInHierarchy)
        {
            List<string> retVal = new List<string>();

           
            retVal = DoCalculations(ref1, ref2, isBullEye);

            Color drawColor = DrawLine2(ref1, ref2);
            string name = ref1.name + "-" + ref2.name;
            DisplayCalc(name, ref1, retVal, drawColor ,isBullEye);
            
        }
        else
        {
            ClearLine(ref1, ref2);
        }

    }

    public void ClearVisuals(bool isBullsEye)
    {
        foreach(Transform child in lineParent.transform)
        {
            if(isBullsEye)
            {
                if(child.name.Contains("bullseye"))
                    Destroy(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Transform child in displayParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearLine(GameObject ref1, GameObject ref2)
    {

        
        string nameMaker = ref1.name + "-" + ref2.name;
        if (lineParent.transform.Find(nameMaker))
        {
            Destroy(lineParent.transform.Find(nameMaker).gameObject);
        }
    }

    public void ClearCalculationsText(GameObject ref1, GameObject ref2)
    {

        string nameMaker = ref1.name + "-" + ref2.name;
        if (ref1.transform.Find(nameMaker))
        {
            Destroy(ref1.transform.Find(nameMaker).gameObject);
        }
    }

    public void ClearBullseyeLine(GameObject ref1, GameObject ref2)
    {
        string nameMaker = ref1.name + "-" + ref2.name;
        if (lineParent.transform.Find(nameMaker))
        {
            Destroy(lineParent.transform.Find(nameMaker).gameObject);
        }
    }

    public void ClearAllBullseyeLinesAndCalculations(CustomDict row)
    {
        foreach(GameObject child in row.GetValues())
        {
            if(child)
            {

                ClearLine( child, row.GetKey());
                ClearCalculationsText(child, row.GetKey());
            }
        }
    }

    public void ClearOneToOnePR()
    {
        Debug.Log("it is called");
        //This method clears the tethered line
        //if(lineRenderer)
        lineRenderer.positionCount = 0;
        if (DisplayText)
            DisplayText.SetActive(false);
        ref1Transform = null;
        ref2Transform = null;
    }

    public CustomTable OneToManyParameterReference(GameObject ref1, GameObject ref2,int currentPage,int height)
    {

        //List of CustomTable ( new class ) created.


        //returnTable[0] will have all rows for selection 1 of drop down
        //returnTable[1] will have all rows for selection 2 of drop down
        CustomTable refTable = new CustomTable();
        //  CustomTable ref2Table = new CustomTable();


        if (ref1 == null && ref2 == null)
        {
            return refTable;
        }
        else
        {
            int inactivePlanes = 0;
            foreach (Transform aircraft in aircraftParent.transform)
            {

                if (aircraft.gameObject.activeInHierarchy)
                {
                    int SiblingIndex = aircraft.GetSiblingIndex() - inactivePlanes;

                    //Lies between the rows to be calculated
                    if (SiblingIndex >= (currentPage * height) && SiblingIndex < (currentPage* height) + height)
                    {

                        //Iterate through each aircraft in parent object and doo the calculations
                        List<string> calculations = new List<string>();

                        //create new row - add aircrat name at index 0 and remaining calculations after it
                        calculations.Add(aircraft.gameObject.GetComponent<movement>().callsign);


                        if (ref1)
                        {
                            if (!ref1.activeInHierarchy)
                            {
                                string[] inActiveStatusArray = { "NaN", "NaN", "NaN", "NaN", "NaN", "NaN" };
                                calculations.AddRange(inActiveStatusArray);
                            }
                            else
                            {
                                calculations.AddRange(DoCalculations(aircraft.gameObject, ref1));
                            }
                            //Add row method also keeps the counter. So we can check if a table is empty if ref1Table.RowCount = 0

                        }
                        else
                        {
                            string[] emptyArray = { " ", " ", " ", " ", " ", " " };
                            calculations.AddRange(emptyArray);
                        }

                        if (ref2)
                        {
                            if (!ref2.activeInHierarchy)
                            {
                                string[] inActiveStatusArray = { "NaN", "NaN", "NaN", "NaN", "NaN", "NaN" };
                                calculations.AddRange(inActiveStatusArray);
                            }
                            else
                            {
                                calculations.AddRange(DoCalculations(aircraft.gameObject, ref2));
                            }

                        }
                        else
                        {
                            string[] emptyArray = { " ", " ", " ", " ", " ", " " };
                            calculations.AddRange(emptyArray);
                        }

                        refTable.AddRow(calculations);
                    }
                }
                else
                {
                    inactivePlanes++;
                }

            }

            //will add these tables to return table. Please see CustomTable methods to retrieve these rows.


            return refTable;
        }
    }

    private double CalculateAltitudeDifference(GameObject ref1, GameObject ref2)
    {
        //Get altitude of ref objects
        double altitude1 = ref1.GetComponent<movement>().GetCurrentAltitude() ;
        double altitude2 = ref2.GetComponent<movement>().GetCurrentAltitude();

        return Mathf.Abs((float)(altitude1 - altitude2));
    }

    private float CalculateAngle(GameObject ref1, GameObject ref2)
    {
        //Simple pythogoras
     
        double lat1 = ref2.transform.localPosition.z;//38.582011;
        double lon1 = ref2.transform.localPosition.x;
        double lat2 = ref1.transform.localPosition.z;
        double lon2 = ref1.transform.localPosition.x;
        //Debug.Log(lat1 + " " + lat2);

        double dLon = (lon2 - lon1);
  

        //float bearing = Mathf.Rad2Deg * Mathf.Tan((float)dLat/ (float)dLon);

        //if(lat1>lat2 && lon1 < lon2)
        //{
        //    bearing= bearing + 270;
        //}
        //else if (lat1 < lat2 && lon1 < lon2)
        //{
        //    bearing = bearing + 180;
        //}
        //else if (lat1 < lat2 && lon1 > lon2)
        //{
        //    bearing = bearing +90;
        //}
        //return bearing;
        double y = Mathf.Sin((float)dLon) * Mathf.Cos((float)lat2);
        double x = Mathf.Cos((float)lat1) * Mathf.Sin((float)lat2) - Mathf.Sin((float)lat1) * Mathf.Cos((float)lat2) * Mathf.Cos((float)dLon);
        float bearing = Mathf.Rad2Deg * (Mathf.Atan2((float)y, (float)x));
        bearing = (((bearing + 360) % 360));

      

           return bearing;
     

    }

    private double CalculateDistance(GameObject ref1, GameObject ref2, string unit = "Nm",bool isBullEye = false)
    {
        //TODO- Get lat long of reference objects at given time
        movement ref1script = ref1.GetComponent<movement>();
        double lat1 = ref1script.GetCurrentLatitue();//38.582011;
        double lon1 = ref1script.GetCurrentLongitude();// 106.050793;
        double lat2; 
        double lon2;
        if (isBullEye)
        {
           Vector2 ref2LatLong = ref2.GetComponent<PositionData>().GetLatLong();
            lat2 = ref2LatLong.x;
            lon2 = ref2LatLong.y;
        }
        else
        {
            movement ref2script = ref2.GetComponent<movement>();
             lat2 = ref2script.GetCurrentLatitue();// 39.582011;
             lon2 = ref2script.GetCurrentLongitude();// 106.050793;
        }

        if ((lat1 == lat2) && (lon1 == lon2))
        {
            return 0;
        }
        else
        {
            //Heavy maths calculations
            double theta = lon1 - lon2;
            double dist = Mathf.Sin((float)(Mathf.Deg2Rad * lat1)) * Mathf.Sin((float)(Mathf.Deg2Rad * lat2)) + Mathf.Cos((float)(Mathf.Deg2Rad * lat1)) * Mathf.Cos((float)(Mathf.Deg2Rad * lat2)) * Mathf.Cos((float)(Mathf.Deg2Rad * theta));
            dist = Mathf.Acos((float)dist);
            dist = Mathf.Rad2Deg * dist;
            dist = dist * 60 * 1.1515;
            if (unit == "K")
            {
                dist = dist * 1.609344;
            }
            else if (unit == "Nm")
            {
                dist = dist * 0.8684;
            }
            return dist;
        }
    }

    private float CalculateBearing(GameObject ref1, GameObject ref2, string unit = "D")
    {
        //TODO-Get lat long of reference objects at given time
        movement ref1script = ref1.GetComponent<movement>();
        movement ref2script = ref2.GetComponent<movement>();
        double lat1 = ref1script.GetCurrentLatitue();//38.582011;
        double lon1 = ref1script.GetCurrentLongitude();// 106.050793;
        double lat2 = ref2script.GetCurrentLatitue();// 39.582011;
        double lon2 = ref2script.GetCurrentLongitude();// 106.050793;

        double dLon = (lon2 - lon1);
        //double dLat = (lat2 - lat1);

        //float bearing = Mathf.Rad2Deg * Mathf.Tan((float)dLat/ (float)dLon);

        //if(lat1>lat2 && lon1 < lon2)
        //{
        //    bearing= bearing + 270;
        //}
        //else if (lat1 < lat2 && lon1 < lon2)
        //{
        //    bearing = bearing + 180;
        //}
        //else if (lat1 < lat2 && lon1 > lon2)
        //{
        //    bearing = bearing +90;
        //}
        //return bearing;
        double y = Mathf.Sin((float)dLon) * Mathf.Cos((float)lat2);
        double x = Mathf.Cos((float)lat1) * Mathf.Sin((float)lat2) - Mathf.Sin((float)lat1) * Mathf.Cos((float)lat2) * Mathf.Cos((float)dLon);
        float bearing = Mathf.Rad2Deg * (Mathf.Atan2((float)y, (float)x));
        bearing = (((bearing + 360) % 360));

        if (unit == "R")
        {
            return Mathf.Deg2Rad * bearing;
        }
        else if (unit == "D")
        {

            return bearing;
        }
        else
        {
            return -1;
        }
    }

    private double CalculateROC(GameObject ref1, GameObject ref2)
    {
        movement ref1script = ref1.GetComponent<movement>();
        movement ref2script = ref2.GetComponent<movement>();
        double speed1 = ref1script.GetCurrentSpeed();
        double speed2 = ref2script.GetCurrentSpeed();

        return speed1 - speed2;
      
    }
}

public class CustomTable
{
    //Custom table class to create a list of list of strings (table content)
    int rowCount = 0;
    List<List<string>> rows = new List<List<string>>();

    public List<string> GetRow(string name)
    {
        //GetRow can be used to retrieve a specific row starting with a plane name
        foreach (List<string> x in rows)
        {
            if (x[0] == name)
            {
                return x;
            }
        }

        return null;
    }

    public List<List<string>> GetTable()
    {
        //Return entire table
        return rows;
    }

    public void AddRow(List<string> row)
    {
        //Add a new row to table
        rowCount++;
        rows.Add(row);
    }

    public int RowCount()
    {
        return rowCount;
    }

    public List<string> GetHeadings()
    {
        //Can be called to crosscheck the headings and their values on the corresponding index
        List<string> headings = new List<string>();
        headings.Add("Name");
        headings.Add("Aspect Angle");
        headings.Add("Distance");
        headings.Add("Bearing");
        headings.Add("AltitudeDifference");
        return headings;
    }
}


