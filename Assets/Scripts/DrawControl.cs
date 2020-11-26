using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawControl : MonoBehaviour
{

    enum Modes { NONE, CIRCLE, LINE, POINT, BULLSEYE };

    float leftlong;
    float bottomlat;
    float rightlong;
    float toplat;
    bool init = false;
    bool firstClick = false;
    const double resolutionY = 1000;
    const double resolutionX = 1000;

    public GameObject terrainManager;
    public GameObject EditManager;
    public Text latlongtext;
    public ListBox displaybar;
    public ListBox lineListBox;
    private int count = 0;

    public GameObject linePrefab;
    public Transform lineParent;
    private GameObject currentLine;
    private Modes currentMode;

    private int countB = 0;
    public ListBox BullListBox;
    private GameObject currentpoint;
    public GameObject PointPrefab;
    public Transform BullEyeParent;


    private int countC = 0; //Added
    public ListBox CircleListBox; //Added
    public GameObject circlePrefab;
    public Transform circleParent;
    private GameObject circleObject;
    bool startDraw = false;
    bool registerMouse = false;
    Vector3 initialPosition;
    Vector3 initialScale;

    public Dropdown LineDropdown;
    private Renderer linePrefabMat;

    public Dropdown BullEyeDropdown;
    private SpriteRenderer bullEyePrefabMat;

    public static float verticalDistance;
    public static float horizontalDistance;
    public static double scaledDownX;
    public static double scaledDownY;
    Line lineDetails;

    LineRenderer lndr;
    public InputField lineWidthInput;
    public Slider LineWidth;
    GameObject line;
    GameObject point;
    //public Text LabelPrefab;
    //public InputField LineLabel;
    //public InputField BullEyeLabel;
    //public InputField CircleLabel;

    void Start()
    {
        Init();

        currentMode = Modes.NONE;
    }


    void Init()
    {
        Vector4 terrainSpecs = terrainManager.GetComponent<TerrainControl>().GetTerrainSpecs();
        leftlong = terrainSpecs[0];
        bottomlat = terrainSpecs[1];
        rightlong = terrainSpecs[2];
        toplat = terrainSpecs[3];
        init = true;

        verticalDistance = Calc((float)bottomlat, (float)leftlong, (float)toplat, (float)leftlong);
        horizontalDistance = Calc((float)bottomlat, (float)leftlong, (float)bottomlat, (float)rightlong);
        scaledDownX = 1.0f / resolutionX;
        scaledDownY = 1.0f / resolutionY;
        //Debug.Log("INIT TRUE");
    }

    public void setCurrentMode(int i)
    {
        if (i == 0)
        {
            currentMode = Modes.LINE;
        }

        else if (i == 1)
        {
            currentMode = Modes.BULLSEYE;
        }
        else if (i == 2)
        {
            currentMode = Modes.CIRCLE;
        }
    }
    void Update()
    {

        Vector3 unClampedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 pos = ClampMousePos(unClampedPos);
        Vector2 latLong = XYtoLatLong(pos.x, pos.y);
        latLong = ClampLatLong(latLong);
        UpdateText(latLong);

        if (EditManager.GetComponent<EditControl>().GetMode() == "DRAW")
        {
            //Pos is clamped world position
            if (currentMode == Modes.LINE)
            {
                TackleDrawLine(pos);
            }

            if (currentMode == Modes.BULLSEYE)
            {
                TackleDrawBullEye(pos);
            }
            if (currentMode == Modes.CIRCLE)
            {
                TackleDrawCircle(pos);
            }

        }


    }

    string GetClickedObjectName()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000f))
        {
            return hit.transform.name;
        }
        return "";
    }

    public GameObject GetInstantiatedLine()
    {
        return line;
    }

    void TackleDrawLine(Vector3 posCalculated)
    {
        Vector3 pos = posCalculated;
        pos = new Vector3(pos.x, pos.y, -1);

        if (Input.GetMouseButtonDown(0) && GetClickedObjectName().Equals("Terrain2D"))
        {

            line = Instantiate(linePrefab, lineParent);
            line.name = "line" + count.ToString();
          
            lndr = line.GetComponent<LineRenderer>();
            //lndr.startWidth = 5f;
            lndr.startWidth = LineWidth.value;


            linePrefabMat = line.GetComponent<LineRenderer>();
            Color UpdatedlineColor = LineColorChange();
            

            lineListBox.AddItem(new ListBox.ListItem(line.name));

            currentLine = line;
            count++;


            currentLine.GetComponent<LineSpecs>().SetLineStart(pos);
            currentLine.GetComponent<LineSpecs>().SetStart(XYtoLatLong(pos.x, pos.y));
            currentLine.GetComponent<LineSpecs>().SetLineColor(UpdatedlineColor);
        }

        if (Input.GetMouseButton(0) && GetClickedObjectName().Equals("Terrain2D"))
        {

            currentLine.GetComponent<LineSpecs>().SetLineEnd(pos);
            currentLine.GetComponent<LineSpecs>().SetEnd(XYtoLatLong(pos.x, pos.y));


        }

        if (Input.GetMouseButtonUp(0) && GetClickedObjectName().Equals("Terrain2D"))
        {
            currentLine.GetComponent<LineSpecs>().UpdateCollider();
        }
    }

    public GameObject GetInstantiatedBullEye()
    {
        return point;
    }

    void TackleDrawBullEye(Vector3 posCalculated)
    {
        Vector3 pos = posCalculated;
        pos = new Vector3(pos.x, pos.y, -1);

        if (Input.GetMouseButtonDown(0) && GetClickedObjectName().Equals("Terrain2D"))
        {

            point = Instantiate(PointPrefab, BullEyeParent);
            point.transform.position = pos;
            point.name = "BullEye" + countB.ToString();


            bullEyePrefabMat = point.GetComponent<SpriteRenderer>();

            Color UpdatedBullEyeColor = BullEyeColorChange();


            BullListBox.AddItem(new ListBox.ListItem(point.name));

            currentpoint = point;
            countB++;
            currentpoint.GetComponent<BullSpecs>().SetPointLatLong(XYtoLatLong(pos.x, pos.y));
            currentpoint.GetComponent<BullSpecs>().SetBullEyeColor(UpdatedBullEyeColor);


        }
    }

    public GameObject GetInstantiatedCircle()
    {
        return circleObject;
    }

    void TackleDrawCircle(Vector3 posCalculated)
    {
        Vector3 pos = posCalculated;
        pos = new Vector3(pos.x, pos.y, -1);

        //Check if mouse button is pressed on terrain
        if (Input.GetMouseButtonDown(0) && GetClickedObjectName().Equals("Terrain2D"))
        {
            circleObject = Instantiate(circlePrefab, circleParent);

            circleObject.name = "Circle" + countC.ToString(); //Added
            circleObject.transform.position = pos;
            initialPosition = pos;

            initialScale = circleObject.transform.localScale;
            startDraw = true;
            registerMouse = true;
            CircleListBox.AddItem(new ListBox.ListItem(circleObject.name)); //Added
            countC++;

        }
        if (registerMouse)
        {
            if (Input.GetMouseButtonUp(0))
            {
                startDraw = false;
                registerMouse = false;

                circleObject.GetComponent<CircleSpecs>().SetPointLatLong(XYtoLatLong(initialPosition.x, initialPosition.y));
                circleObject.AddComponent<PolygonCollider2D>();
            }
            if (startDraw)
            {

                Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - initialPosition;
                circleObject.transform.localScale = new Vector3(initialScale.x + difference.x, initialScale.y + difference.y);
                circleObject.GetComponent<CircleSpecs>().SetXYRadius(initialScale.x + difference.x, initialScale.y + difference.y);

            }
        }
    }

    public void UpdateText(Vector2 latLong)
    {
        string textMaker = "Latitude: " + latLong.x.ToString("F4") + "\nLongitude: " + latLong.y.ToString("F4");
        latlongtext.text = textMaker;
    }

    private Vector3 ClampMousePos(Vector3 mPos)
    {
        return new Vector3(Mathf.Clamp(mPos.x, 0, (float)resolutionX), Mathf.Clamp(mPos.y, 0, (float)resolutionY), mPos.z);
    }
    private Vector2 ClampLatLong(Vector2 mLatLong)
    {
        return new Vector2(Mathf.Clamp(mLatLong.x, bottomlat, toplat), Mathf.Clamp(mLatLong.y, leftlong, rightlong));
    }

    public Vector2 XYtoLatLong(float x, float y)
    {
        if (!init)
            Init();

        double percentageX = x / resolutionX;
        double differenceX = rightlong - leftlong;
        double valueLong = leftlong + (percentageX * differenceX);

        double percentageY = y / resolutionY;
        double differenceY = toplat - bottomlat;
        double valueLat = bottomlat + (percentageY * differenceY);

        return new Vector2((float)valueLat, (float)valueLong);
    }


    public float Calc(float lat1, float lon1, float lat2, float lon2)
    {

        var R = 6378.137; // Radius of earth in KM
        var dLat = lat2 * Mathf.PI / 180 - lat1 * Mathf.PI / 180;
        var dLon = lon2 * Mathf.PI / 180 - lon1 * Mathf.PI / 180;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
        Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) *
        Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        double distance = R * c;
        distance = distance * 1000f; // meters
                                     //set the distance text on the canvas
                                     //distanceTextObject.GetComponent<Text>().text = "Distance: " + distance;
                                     //convert distance from double to float
        float distanceFloat = (float)distance;
        //Debug.Log(distanceFloat);
        return distanceFloat;

    }

    public float[] LatLongToXY(double lat1, double long1)
    {
        if (!init)
            Init();
        Debug.Log(" Lat : " + lat1 + " long" + long1);
        double percentage1 = (lat1 - bottomlat) / (toplat - bottomlat);
        double YinTerrain = resolutionY * percentage1;

        double percentage2 = (long1 - leftlong) / (rightlong - leftlong);
        double XinTerrain = resolutionX * percentage2;
        Debug.Log(XinTerrain + " " + YinTerrain);

        float[] rxy = new float[2];
        rxy[0] = (float)XinTerrain;
        rxy[1] = (float)YinTerrain;
        Debug.Log(" X : " + rxy[0] + " Y" + rxy[1]);
        return rxy;
    }

    public Vector2 CalculateXYRadius(float xRadius, float yRadius)
    {

        return circleObject.transform.localScale = new Vector3(xRadius , yRadius , 0);  // radius * 2

    }

    public Color LineColorChange()
    {
        if (LineDropdown.value == 0)
            return linePrefabMat.material.color = Color.blue;

        else if (LineDropdown.value == 1)
            return linePrefabMat.material.color = Color.green;

        else if (LineDropdown.value == 2)
            return linePrefabMat.material.color = Color.magenta;

        else if (LineDropdown.value == 3)
            return linePrefabMat.material.color = Color.red;

        else if (LineDropdown.value == 4)
            return linePrefabMat.material.color = Color.black;


        return Color.clear;
    }

    public Color BullEyeColorChange()
    {
        if (BullEyeDropdown.value == 0)
            return bullEyePrefabMat.color = Color.red;

        else if (BullEyeDropdown.value == 1)
            return bullEyePrefabMat.color = Color.blue;

        else if (BullEyeDropdown.value == 2)
            return bullEyePrefabMat.color = Color.green;

        else if (BullEyeDropdown.value == 3)
            return bullEyePrefabMat.color = Color.magenta;

        else if (BullEyeDropdown.value == 4)
            return bullEyePrefabMat.color = Color.black;

        return Color.clear;

    }

}

public class Line
{
    public string Latitude_End;
    public string Latitude_Start;
    public string Longitude_End;
    public string Longitude_Start;
    public string ID;
    public string Name;
    public string Transparency;
    public string LineColor;
    public string LineStyle;
    public string LineWidth;
    public string Label;
}


public class BullEye
{
    public string ID = "";
    public string Latitude = "";
    public string Longitude = "";
    public string LineAngle = "";
    public string LineColor;
    public string LineThickness = "0.1";
    public string Name = "";
    public string NumOfCircles = "7";
    public string Radius = "20.0";
    public string Unit = "Nm";
    public string Label;
}

public class Circle
{
    public string ID = "";
    public string Latitude = "";
    public string Longitude = "";
    public string XRadius = "";
    public string YRadius = "";
    public string LineColor = "";
    public string LineThickness = "0.1";
    public string Name = "";
    public string Unit = "Nm";
    public string Label;
}

public class GGeometry
{
    public string FillColor = "";
    public string HorizontalRadius = "";
    public string ID = "";
    public string Latitude = "";
    public string LineColor = "";
    public string LineThickness = "";
    public string Longitude = "";
    public string Name = "";
    public string Rotation = "";
    public string Unit = "Nm";
    public string Transparency = "";
    public string Type = "Ellipse";
    public string VerticalRadius = "";
}

