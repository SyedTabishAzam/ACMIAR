using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class ReferenceManager : MonoBehaviour {

    public GameObject rowPrefab;
    public GameObject Aircraft_parent;
	private CustomTable table;
    //float Xoffset = -0.27f;
    //float Yoffset = 0.036f;
    //float Zoffset = 0.009f;
    //float spacing = -0.052f;

    float Xoffset = -0.33f;
    float Yoffset = 0.04f;
    float Zoffset = -0.008f;
    float spacing = -0.052f;

    private int idx = 0;
    private int idy = 0;
    int height = 4;
    int width = 12;


    string[] headings = { "Aspect Angle", "Alt Diff", "Slant Rg", "Bearing", "ROC", "Horiz Diff", "Aspect Angle", "Alt Diff", "Slant Rg", "Bearing", "ROC", "Horiz Diff" }; //{"Bank", "Turn Rate",   "Turn Radius", "EGT", "RPM", "Caged / Uncaged",   "Chaffs",  "Flares" };
    List<GameObject> rows = new List<GameObject>();
    static List<Transform> plane = new List<Transform>();

    public Transform headingTransform;
    // Use this for initialization
    void Start()
    {
        //headingTransform = transform.Find("headingRow");
        PopulateHeadings();
        int i = 0;
        //foreach(Transform child in Aircraft_parent.transform)
        //{


        //    GameObject rowGameObject = Instantiate(rowPrefab, transform) as GameObject;
        //    DashboardRow row = rowGameObject.GetComponent<DashboardRow>();
        //    row.SetIndex(0);
        //    row.SetWidth(width);
        //    row.Init();
        //    //row.SetPlane(child);
        //    //row.SetIdentifierName(child.transform.GetComponent<movement>().callsign);
        //    rowGameObject.transform.position = new Vector3(transform.position.x  + Xoffset, transform.position.y+ Yoffset, transform.position.z + (i * spacing) + Zoffset );

        //    rows.Add(rowGameObject);
        //    i++;
        //}
        //foreach (Transform child in Aircraft_parent.transform)
        //   plane.Add(child);

        while (i != height)
        {
            GameObject rowGameObject = Instantiate(rowPrefab, transform) as GameObject;
            DashboardRow row = rowGameObject.GetComponent<DashboardRow>();
            row.SetIndex(0);
            row.SetWidth(width);
            row.Init();
            //row.SetPlane(plane[i + (height * idy)].transform);
            //rowGameObject.transform.position = new Vector3(transform.position.x + Xoffset, transform.position.y + Yoffset, transform.position.z + (i * spacing) + Zoffset);
            rowGameObject.transform.position = new Vector3(transform.position.x + Xoffset, transform.position.y + (i * spacing) + Yoffset, transform.position.z + Zoffset);
            rows.Add(rowGameObject);
            i++;
        }

        transform.Rotate(new Vector3(75, 0, 0));
        //transform.parent.localScale = new Vector3(transform.parent.localScale.x - 0.2f, transform.parent.localScale.y, transform.parent.localScale.z);
    }

    public GameObject getRow(int i)
    {
        if (i > 4)
            i = i % 4;
        return rows[i];
    }

    public static void AddPlane(Transform Aplane)
    //create a list of plane transforms in order
    {
        //foreach (Transform child in Aircraft_parent.transform)
        plane.Add(Aplane);
    }
    
    public void emptyRow(int count)
    {
        for (int i = count; i<rows.Count; i++)
        {
            rows[count].GetComponent<DashboardRow>().EmptyRow(); 
        }
        
    }

	public void SetCustomTable(CustomTable _table)
	{
		table = _table;
	}

    public int GetHeight()
    {
        return height;
    }

    public int GetPage()
    {
        return idy;
    }

    public void PopulateRow()
    {
        
        foreach (GameObject row in rows)
        {

            //0 - 3 == idy = 0
            // 3 -7 == idy = 1

            //planeCount = 6, j == 6
            int j = rows.IndexOf(row);// + (height * idy);
            DashboardRow dashboardrow = row.GetComponent<DashboardRow>();
            if(plane.Count != table.RowCount())
            {
                emptyRow(table.RowCount()); 
                
            }
			if (j > table.GetTable().Count)
			{

				emptyRow(j);
				//dashboardrow.SetIndex(idx);
			}

			if (j<table.GetTable().Count ) {

                
				List<string> data = table.GetTable () [j];
				
				//dashboardrow.SetPlane(plane[j + (height * idy)]);
				dashboardrow.ChangeIdentifier (data [0]);

				dashboardrow.SetData (data.Skip (1).ToArray ());
				dashboardrow.SetIndex (idx);
			} 



        }

    }

    public void PopulateHeadings()
    {
        int i = 0;

        foreach (Transform child in headingTransform)
        {

            if (child.name.Contains("Cube"))
            {
                TextMesh temp = child.GetChild(0).GetComponent<TextMesh>();
                temp.text = "";
                //idx==2
                //2*5=10, i
 
                if (i + (idx * width) < headings.Length)
                {

                    temp.text = headings[i + (idx * width)];
                    temp.fontSize = 28;
                    i++;
                    //Debug.Log(i + " " + temp.text);
                }
            }
        }
    }

    public void IncreaseIndex()
    {
        idx++;
        idx = Mathf.Clamp(idx, 0, 3);
        PopulateHeadings();
    }

    public void DecreaseIndex()
    {
        idx--;
        idx = Mathf.Clamp(idx, 0, 3);
        PopulateHeadings();
    }
    public void resetIdy()
    {
        idy = 0;
    }
    //vertical up and down
    public void IncreaseRows()
    {
        idy++;
        double m = plane.Count / height;
        int pages = (int)Math.Ceiling(m);
        //Math.Ceiling(pages);
        idy = idy % (pages + 1);

    }

    public void DecreaseRows()
    {
        idy--;

        double m = plane.Count / height;
        int pages = (int)Math.Ceiling(m);
        if (idy < 0)
            idy = pages;
        idy = idy % (pages + 1);

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Transform child in Aircraft_parent.transform)
        {
            if (child.gameObject.activeInHierarchy & !plane.Contains(child))
            {
                plane.Add(child);
            }
            else if (!child.gameObject.activeInHierarchy & plane.Contains(child))
            {
                plane.Remove(child);
            }   
        }
        //Update display
        //for each row in childtransform
        //Debug.Log(plane.Count);
        //PopulateRow();
        //int i = 0;
        double m = plane.Count / height;
        //int pages = (int)Math.Ceiling(m);
        //Debug.Log("vertical"+ pages);
        if (plane.Count != 0)
        {
           // PopulateRow();
        }
    }

    public void Reset()
    {
        resetIdy();
        plane.Clear();
        foreach (GameObject row in rows)
        {
            DashboardRow dashboardrow = row.GetComponent<DashboardRow>();
            dashboardrow.EmptyRow();
            dashboardrow.SetIndex(0);
        }
        transform.GetComponent<ReferenceSelection>().resetAll();

    }
}
