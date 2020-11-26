using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
public class DashboardManager : MonoBehaviour
{
    public GameObject rowPrefab;
    public GameObject Aircraft_parent;
    //float Xoffset = -0.27f;
    //float Yoffset = 0.036f;
    //float Zoffset = 0.009f;
    //float spacing = -0.052f;

    float Xoffset = -0.27f;
    float Yoffset = 0.04f;
    float Zoffset = -0.007f;
    float spacing = -0.052f;

    private int idx = 0;
    private int idy = 0;
    int height = 4;
    int width = 5;
    int siblingIndex = 0;

    string[] headings = { "A/C Type", "Heading", "Altitude", "CAS", "TAS", "IAS", "Mach No.", "G", "AOA", "Latitude", "Longitude", "AGL", "Pitch" }; //{"Bank", "Turn Rate",   "Turn Radius", "EGT", "RPM", "Caged / Uncaged",   "Chaffs",  "Flares" };
    List<GameObject> rows = new List<GameObject>();
    static List<Transform> plane = new List<Transform>();

    Transform headingTransform;
    // Use this for initialization
    void Start()
    {
        headingTransform = transform.Find("headingRow");
        PopulateHeadings();
        int i = 0;


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
            
    }
    public GameObject getRow(int i)
    {
        if (i > 4)
            i = i % 4;
        return rows[i];
    }

    //public  void AddPlane(Transform Aplane)
    ////create a list of plane transforms in order
    //{
    //    //foreach (Transform child in Aircraft_parent.transform)
    //    plane.Add(Aplane);
    //}

    public void emptyRow(int count)
    {
        for (int i = count; i < rows.Count; i++)
        {
            rows[count].GetComponent<DashboardRow>().EmptyRow();
        }

    }

    public void PopulateRow()
    {
      
        foreach (GameObject row in rows)
        {
            //string [] data =Aircraft_parent.transform.GetChild(i).GetComponent<movement>().GetData();
            int j = rows.IndexOf(row);

            DashboardRow dashboardrow = row.GetComponent<DashboardRow>();

            if (j + (height * idy) > plane.Count - 1)
            {

                dashboardrow.EmptyRow();
                dashboardrow.SetIndex(idx);
                
            }
            else
            {
                
                //Debug.Log("Here");
                dashboardrow.SetPlane(plane[j + (height * idy)]);

                //Debug.Log("Plane======================="+ plane[0]);

                var scene = SceneManager.GetActiveScene().name;

                if (scene == "ARScene")
                {
                    string[] data = dashboardrow.GetPlane().GetComponent<movement>().GetData();

                    dashboardrow.ChangeIdentifier(data[0]); // SET TAIL ID
                    dashboardrow.SetData(data.Skip(1).ToArray()); //
                    dashboardrow.SetIndex(idx);
                }
                else if (scene == "LiveData")
                {
                    string[] data = dashboardrow.GetPlane().GetComponent<AircraftDataManager>().GetData();


                    dashboardrow.ChangeIdentifier(data[0]); // SET TAIL ID
                    dashboardrow.SetData(data.Skip(1).ToArray()); //
                    dashboardrow.SetIndex(idx);
                }

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
                    i++;
                }
            }
        }
    }

    public void IncreaseIndex()
    {
        idx++;
        idx = idx % 3;
        PopulateHeadings();
    }

    public void DecreaseIndex()
    {
        idx--;
        if(idx<0)
        {
            idx = 2;
        }
        idx = idx % 3;
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
        idy = idy % (pages+1);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Transform child in Aircraft_parent.transform)
        {
            if (child.gameObject.activeInHierarchy & !plane.Contains(child))
            {
                plane.Add(child);
                child.SetSiblingIndex(siblingIndex);
                siblingIndex++;
            }
            else if (!child.gameObject.activeInHierarchy & plane.Contains(child))
            {
                plane.Remove(child);
                siblingIndex--;
            }
        }

        string textMaker = "Total aircrafts: " + plane.Count;

        GameObject.Find("ACCounter").GetComponent<TextMesh>().text = textMaker;
        //Update display
        //for each row in childtransform
        //Debug.Log(plane.Count);
        //PopulateRow();
        int i = 0;
        double m = plane.Count / height;
        int pages = (int)Math.Ceiling(m);
        //Debug.Log("vertical"+ pages);
        if (plane.Count != 0)
        {
            PopulateRow();
        }

    }
}
