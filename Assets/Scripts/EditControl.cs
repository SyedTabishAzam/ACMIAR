using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditControl : MonoBehaviour {

    public enum Modes { DRAW, EDIT };
    public Modes layoutMode;
    public GameObject EditLinePanel;
    public InputField lat1;
    public InputField lat2;
    public InputField long1;
    public InputField long2;
    private GameObject objLine;
    public GameObject drawManager;
    public ListBox lineListBox;
    
    public ListBox bullListBox;
    private GameObject objBullEye;
    public GameObject bullsEyePanel;
    public InputField latBull;
    public InputField longBull;
  
    public InputField width;
    public InputField distanceBetweenRings;
    public InputField numberOfRings;

    public GameObject circlesPanel;
    public InputField latCircle;
    public InputField longCircle;
    public InputField xRadiusCircle;
    public InputField yRadiusCircle;
    
    public GameObject labelPrefab;
    public InputField bullEyelabel;
    public InputField circleLabel;
    public InputField lineLabel;


    void Update()
    {

        bool isAnyPanelOpen = bullsEyePanel.activeInHierarchy || circlesPanel.activeInHierarchy || EditLinePanel.activeInHierarchy;
        if (layoutMode == Modes.EDIT && !isAnyPanelOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (DetectLine() == null)
                {
                    if (DetectBullEye() == null)
                    {
                        DetectCircle();
                    }
                }    
            }
        }
    }

    
    public void setObj(GameObject line)
    {
        objLine = line;
        //Debug.Log("objline is set to " + objLine.name);
    }
    public GameObject getObj()
    {
        //Debug.Log("objline get " + objLine.name);
        return objLine; 
    }
    
    
    public GameObject DetectLine()
    {

        foreach (Transform child in GameObject.Find("LinesParent").transform)
        {
            GameObject obj = child.GetComponent<LineSpecs>().Detected(Input.mousePosition);
            
            if (obj != null)
            {
                List<string> objSpecs = child.GetComponent<LineSpecs>().OnSelection();
                onLineSelection(obj, objSpecs);
                setObj(obj);
                return obj;
            }
        }
        return null;
    }
    
    public GameObject DetectCircle()
    {
        
        foreach (Transform child in GameObject.Find("CirclesParent").transform)
        {
            GameObject obj = child.GetComponent<CircleSpecs>().Detected(Input.mousePosition);
            if (obj!=null)
            {
                List<string> objSpecs = child.GetComponent<CircleSpecs>().OnSelection();
                OnCircleSelection(obj, objSpecs);
                setObj(obj);
                return obj;
            }
        }
        return null;

    }
    public GameObject DetectBullEye()
    {
        foreach (Transform child in GameObject.Find("BullsEyeParent").transform)
        {
            GameObject obj = child.GetComponent<BullSpecs>().Detected();
            if (obj != null)
            {
                Debug.Log(obj.name);
                List<string> objSpecs = child.GetComponent<BullSpecs>().OnSelection();
                OnBullEyeSelection(obj, objSpecs);
                setObj(obj);
                return obj;
            }
        }
        return null;
    }

    
    public string GetMode()
    {
        return layoutMode.ToString();
    }
    

    public void switchMode()
    {
        if (layoutMode == Modes.DRAW)
            layoutMode = Modes.EDIT;
        else
            layoutMode = Modes.DRAW;
    }

    public void SwitchToEdit()
    {
        layoutMode = Modes.EDIT;
    }

    public void SwitchToDraw()
    {
        layoutMode = Modes.DRAW;
    }
    
    
    public void onLineSelection(GameObject obj, List<string> objSpec)
    {
        lat1.text = objSpec[0];
        long1.text = objSpec[1];
        lat2.text = objSpec[2];
        long2.text = objSpec[3];
        EditLinePanel.SetActive(true);
    }


    public void OnBullEyeSelection(GameObject obj, List<string> objSpec)
    {
        latBull.text = objSpec[0];
        longBull.text = objSpec[1];
        //label.text = objSpec[2];
        //color.itemText = objSpec[3];
       //width.text = objSpec[3];
       //unit.text = objSpec[5];
       //distanceBetweenRings.text = objSpec[4];
       //numberOfRings.text = objSpec[5];
        bullsEyePanel.SetActive(true);

    }

    public void OnCircleSelection(GameObject obj, List<string> objSpec)
    {
        
        latCircle.text = objSpec[0];
        longCircle.text = objSpec[1];
        xRadiusCircle.text = objSpec[2];
        yRadiusCircle.text = objSpec[3];
        
        circlesPanel.SetActive(true);

    }

    public void OnBullSave()
    {
        GameObject obj = getObj();
        Debug.Log("updating Bull" + obj.gameObject.name);
        List<string> objSpec = new List<string> { };
        objSpec.Add(latBull.text);
        objSpec.Add(longBull.text);
        //objSpec.Add(bullEyelabel.text);
        obj.GetComponent<BullSpecs>().UpdateBullEyeSpecs(objSpec);

        List<float> mouseCord = new List<float> { };
        float[] point1 = drawManager.GetComponent<DrawControl>().LatLongToXY(Convert.ToDouble(ConvertDegreeAngleToDouble(latBull.text)), Convert.ToDouble(ConvertDegreeAngleToDouble(longBull.text)));
        
        mouseCord.Add(point1[0]);
        mouseCord.Add(point1[1]);
        obj.GetComponent<BullSpecs>().updateMousePos(mouseCord);
    }

    public void OnCircleSave()
    {
        GameObject obj = getObj();
        Debug.Log("updating Circle" + obj.gameObject.name);
        List<string> objSpec = new List<string> { };
        objSpec.Add(latCircle.text);
        objSpec.Add(longCircle.text);
        objSpec.Add(xRadiusCircle.text);
        objSpec.Add(yRadiusCircle.text);
        //objSpec.Add(circleLabel.text);
        obj.GetComponent<CircleSpecs>().UpdateCircleSpec(objSpec);

        Vector2 radiusCalculated = drawManager.GetComponent<DrawControl>().CalculateXYRadius(float.Parse(xRadiusCircle.text),float.Parse(yRadiusCircle.text));

        List<float> mouseCord = new List<float> { };
        float[] point1 = drawManager.GetComponent<DrawControl>().LatLongToXY(Convert.ToDouble(ConvertDegreeAngleToDouble(latCircle.text)), Convert.ToDouble(ConvertDegreeAngleToDouble(longCircle.text)));

        mouseCord.Add(point1[0]);
        mouseCord.Add(point1[1]);
        obj.GetComponent<CircleSpecs>().updateMousePos(mouseCord);


    }

    public void OnLineSave()
    {
        GameObject obj = getObj();
        //Debug.Log("updating line" + obj.gameObject.name);
        List<string> objSpec = new List<string> { };
        objSpec.Add(lat1.text);
        objSpec.Add(long1.text);
        objSpec.Add(lat2.text);
        objSpec.Add(long2.text);
        //objSpec.Add(lineLabel.text);
        //Debug.Log("values to update in xml of line = " + objSpec[0] + " " + objSpec[1] + " " + objSpec[2] + " " + objSpec[3]);
        
        obj.GetComponent<LineSpecs>().updateSpecs(objSpec); 
        List<float> mouseCord = new List<float> { };

        float[] point1 =  drawManager.GetComponent<DrawControl>().LatLongToXY(Convert.ToDouble(ConvertDegreeAngleToDouble(lat1.text)),Convert.ToDouble(ConvertDegreeAngleToDouble(long1.text)));
        float[] point2 =  drawManager.GetComponent<DrawControl>().LatLongToXY(Convert.ToDouble(ConvertDegreeAngleToDouble(lat2.text)),Convert.ToDouble(ConvertDegreeAngleToDouble(long2.text)));

        mouseCord.Add(point1[0]);
        mouseCord.Add(point1[1]);
        mouseCord.Add(point2[0]);
        mouseCord.Add(point2[1]);
        obj.GetComponent<LineSpecs>().updateMousePos(mouseCord);
        
        //return objSpec;
    }

    //line delete function
    public void OnDelete()
    {
        Debug.Log("object deleted");
        lineListBox.RemoveItem(getObj().transform.GetSiblingIndex());
        Destroy(getObj());
        EditLinePanel.SetActive(false);
    }
    
    public void OnBullEyeDelete()
    {
        Debug.Log("object deleted");
        bullListBox.RemoveItem(getObj().transform.GetSiblingIndex());
        Destroy(getObj());
        bullsEyePanel.SetActive(false);
    }

    public void OnCircleDelete()
    {
        Debug.Log("circle deleated");
      //bullListBox.RemoveItem(getObj().transform.GetSiblingIndex());
        Destroy(getObj());
        circlesPanel.SetActive(false);
    }

    public float ConvertDegreeAngleToDouble(string DMS)
    {
        string[] splited = DMS.Split(new char[] { '°', '\'', '"' });

        float Degree = float.Parse(splited[0]);
        float Minute = float.Parse(splited[1]);
        float Second = float.Parse(splited[2]);

        return Degree + (Minute / 60) + (Second / 3600);
    }

    public void CircleLabel()
    {
        GameObject cirRef = drawManager.GetComponent<DrawControl>().GetInstantiatedCircle();
        labelPrefab.GetComponent<Text>().text = circleLabel.text;
        GameObject textPrefabRef = Instantiate(labelPrefab, new Vector3(cirRef.transform.position.x + 50, cirRef.transform.position.y, cirRef.transform.position.z), Quaternion.identity) as GameObject;
        textPrefabRef.transform.parent = cirRef.transform;
    }

    public void LineLabel()
    {
        GameObject LineRef = drawManager.GetComponent<DrawControl>().GetInstantiatedLine();
        labelPrefab.GetComponent<Text>().text = lineLabel.text;
        GameObject textPrefabRef = Instantiate(labelPrefab, new Vector3(LineRef.transform.position.x + 355, LineRef.transform.position.y + 500, LineRef.transform.position.z), Quaternion.identity) as GameObject;
        textPrefabRef.transform.parent = LineRef.transform;
    }

    public void BullEyeLabel()
    {
        GameObject beRef = drawManager.GetComponent<DrawControl>().GetInstantiatedBullEye();
        labelPrefab.GetComponent<Text>().text = bullEyelabel.text;
        GameObject textPrefabRef = Instantiate(labelPrefab, new Vector3(beRef.transform.position.x + 50, beRef.transform.position.y, beRef.transform.position.z), Quaternion.identity) as GameObject;
        textPrefabRef.transform.parent = beRef.transform;
    }
}
