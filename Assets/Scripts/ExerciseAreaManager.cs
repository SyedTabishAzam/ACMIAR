using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExerciseAreaManager : MonoBehaviour {

    List<List<float>> point = new List<List<float>> { };
    public GameObject line;
    public GameObject circle;
    public GameObject bullseye;
    RectTransform rtOfCircle;
    RectTransform rtOfBullEye;
    List<GameObject> lines;
    public GameObject exerciseAreaParent;
    public GameObject drawAreaManager;
    bool drawn = false;
    bool showExerciseArea = false;

    public GameObject labelPrefab;

    private void Start()
    {
        SetShowExerciseAreaTest(true);
        if(exerciseAreaParent==null)
        {
            exerciseAreaParent = GameObject.Find("ExerciseArea");
        }
    }

    public void Drawline(List<List<float>> points)
    {
        //format long1, long2, lat1, lat2
        foreach (List<float> point in points)
        {
            GameObject line1 = Instantiate(line, exerciseAreaParent.transform);
            LineRenderer currentLineRenderer = line1.GetComponent<LineRenderer>();
            //line1.transform.parent = GameObject.Find("ExerciseArea").transform;
            currentLineRenderer.positionCount = 2;
            currentLineRenderer.SetPosition(0, new Vector3(point[4],0,point[2]));
            currentLineRenderer.SetPosition(1, new Vector3(point[5],0, point[3]));
            // = new Material(Shader.Find("Custom\"));
           
            Color newColor = HexToColor(ColorIntToHex((int)point[0]));
            currentLineRenderer.material.color = newColor;
            drawAreaManager.GetComponent<ClippingPlane>().AddObjectToList(line1);
        }
    }

    public void DrawCircle(List<List<float>> points)
    {
        foreach (List<float> point in points)
        {
     
            GameObject circle1 = Instantiate(circle, exerciseAreaParent.transform);

            rtOfCircle = circle1.GetComponent<RectTransform>();

            rtOfCircle.transform.localPosition = new Vector3(point[0], 0f, point[1]);
            circle1.AddComponent<PolygonCollider2D>();
            circle1.transform.localScale = new Vector3(point[2] / 500, point[3] / 500, 0);

            drawAreaManager.GetComponent<ClippingPlane>().AddObjectToList(circle1);

            //Debug.Log("value at index 6 of point is = " + point[6].ToString());
            //labelPrefab.GetComponent<Text>().text = point[6].ToString();
            //GameObject textPrefabRef = Instantiate(labelPrefab, new Vector3(circle1.transform.position.x + 50, circle1.transform.position.y, circle1.transform.position.z), Quaternion.identity) as GameObject;
            //textPrefabRef.transform.parent = circle1.transform;

        }
    }

    public void DrawBullEye(List<List<float>> points)
    {
        foreach (List<float> point in points)
        {

            GameObject bulleye = Instantiate(bullseye, exerciseAreaParent.transform);

            SpriteRenderer spriteRend = bulleye.GetComponent<SpriteRenderer>();
            rtOfBullEye = bulleye.GetComponent<RectTransform>();

            rtOfBullEye.transform.localPosition = new Vector3(point[0], 0f, point[1]);
            rtOfBullEye.transform.localRotation = Quaternion.Euler(-90, 0, 0);

            bulleye.AddComponent<CircleCollider2D>();

            Color newColor = HexToColor(ColorIntToHex((int)point[2]));
            spriteRend.material.color = newColor;

            drawAreaManager.GetComponent<ClippingPlane>().AddObjectToList(bulleye);

        }
    }




    string ColorIntToHex(int colorValue)
    {
        return colorValue.ToString("X8");
    }

    Color HexToColor(string hex)
    {

        Color col = new Color();
        string red = hex.Substring(0,2);
        string green = hex.Substring(2, 2);
        string blue = hex.Substring(4, 2);
        string alpha = hex.Substring(6, 2);
        col.r = int.Parse(red,System.Globalization.NumberStyles.AllowHexSpecifier);
        col.g = int.Parse(green, System.Globalization.NumberStyles.AllowHexSpecifier);
        col.b = int.Parse(blue, System.Globalization.NumberStyles.AllowHexSpecifier);
        col.a = int.Parse(alpha, System.Globalization.NumberStyles.AllowHexSpecifier);

        return col;
    }

    private void Update()
    {
        exerciseAreaParent.SetActive (showExerciseArea);
    }

    public void SetShowExerciseArea()
    {
        showExerciseArea = !showExerciseArea;
    }

    public void SetShowExerciseAreaTest(bool val)
    {
        showExerciseArea = val;
    }
}
