using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlane : MonoBehaviour
{

    public Material[] mat;
    public GameObject[] planeWalls;//,plane2Wall,plane3Wall,plane4Wall;
    Vector4[] planeRepresentation = new Vector4[4];

    List<GameObject> ObjectsToClip;
    public GameObject[] Terrain;
    private bool init = false;

    string state;
    //execute every frame
    private void Awake()
    {
        Shader.WarmupAllShaders();

        if (!init)
        {
            ObjectsToClip = new List<GameObject>();
            init = true;
        }
        //  InitMaterials();
        state = "IDLE";
        foreach (GameObject go in Terrain)
        {

            AddObjectToList(go);
        }
        RecreateClippedMaterial();
    }

    public void AddObjectToList(GameObject obj)
    {


        if (!init)
        {
            ObjectsToClip = new List<GameObject>();
            init = true;
        }
        ObjectsToClip.Add(obj);
    }

    private void RecreateClippedMaterial()
    {
        foreach (GameObject go in ObjectsToClip)
        {
            if (go)
            {
                if (!go.name.Contains("Terrain"))
                {
                    Color col = go.GetComponent<Renderer>().material.color;
                    float brightness = go.GetComponent<Renderer>().material.GetFloat("_myRange");
                    AddNewMaterialToGO(go, go.GetComponent<Renderer>().material.shader.name);
                    go.GetComponent<Renderer>().material.color = col;
                    go.GetComponent<Renderer>().material.SetFloat("_myRange", brightness);

                }
                else
                {
                    AddNewMaterialToGO(go, go.GetComponent<Renderer>().material.shader.name);
                    go.GetComponent<Renderer>().material.mainTexture = Resources.Load("NewTerrain/sargodha_terrain") as Texture;
                    go.GetComponent<Renderer>().material.SetColor("Cutoff Color", Color.black);
                }
            }
        }
    }

    private void AddNewMaterialToGO(GameObject clippingObject, string ShaderName)
    {
        Renderer rend = clippingObject.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find(ShaderName));
        int index = 0;
        foreach (GameObject wall in planeWalls)
        {

            Plane plane = new Plane(wall.transform.up, wall.transform.position);
            //transfer values from plane to vector4
            planeRepresentation[index] = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
            index++;
        }
        rend.material.SetVectorArray("_Plane", planeRepresentation);
    }

    public void InitMaterials()
    {
        int index = 0;
        foreach (GameObject wall in planeWalls)
        {

            Plane plane = new Plane(wall.transform.up, wall.transform.position);
            //transfer values from plane to vector4
            planeRepresentation[index] = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
            index++;

            //pass vector to shader
        }

        foreach (Material m in mat)
        {
            //Debug.Log(m.GetVectorArray("_Plane"));
            m.SetVectorArray("_Plane", planeRepresentation);
        }

        //Debug.Log(planeRepresentation[0]);
    }

    void Update()
    {

        //create plane

        if (state == "ISPLACED")
        {
            RecreateClippedMaterial();
            state = "IDLE";
        }

        if (state == "REFRESH")
        {
            RecreateClippedMaterial();
            state = "IDLE";
        }

        if (state == "ISMOVING")
        {
            //   HideAllObjects();
        }


    }

    void HideAllObjects()
    {

    }

    public void ChangeState(string _state)
    {
        state = _state;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("bullseye"))
        {
            other.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
            other.gameObject.layer = 8;
            foreach (Transform child in other.transform)
            {
                child.gameObject.SetActive(false);
            }

        }
        Debug.Log(other.name);

        if (other.tag == "Aircrafts")
        {
            //Debug name


            //Deactive the aircraft model
            other.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;

            //deadsymbol
            if (other.transform.childCount != 0)
                other.transform.GetChild(0).gameObject.SetActive(false);

            //Deactive aircraft callsign
            //other.transform.GetChild(0).gameObject.SetActive(false);

            //Deactive aircraft trail renderer (its in the parent)
            other.GetComponentInParent<TrailRenderer>().enabled = false;

            //Deactivate aircraft planeInfo
            //if (other.transform.childCount > 1)
            //{
            //    other.transform.GetChild(1).gameObject.SetActive(false);
            //}

            //Deactivate aircraft Calculations for airRef
            for (int i = 1; i < other.transform.parent.childCount; i++)
            {
                other.transform.parent.GetChild(i).gameObject.SetActive(false);
                Debug.Log(other.transform.parent.GetChild(i).gameObject.name + " " + other.transform.parent.childCount + " " + i);
            }


            //Switch aircraft layer to non touchable
            other.gameObject.layer = 8;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("bullseye"))
        {
            other.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
            other.gameObject.layer = 0;
            foreach (Transform child in other.transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        if (!other.transform.parent)
            return;

        if ( other.transform.parent.name.Contains("Display"))
        {
            other.gameObject.SetActive(true);
        }
        if (other.tag == "Aircrafts")
        {
            //Activate the aircraft model
            other.transform.gameObject.GetComponent<MeshRenderer>().enabled = true;

            //Set plane info to what it was before going out of view
            other.transform.parent.GetChild(1).gameObject.SetActive(other.transform.parent.gameObject.GetComponent<movement>().GetSelected());

            //Set callsign to true
            //Set other children of parent (specially calculations) to true
            for (int i = 2; i < other.transform.parent.childCount; i++)
            {
                other.transform.parent.GetChild(i).gameObject.SetActive(true);
            }

            //deadsymbol
            //if(other.transform.childCount>0)
            if (other.transform.childCount != 0)
                other.transform.GetChild(0).gameObject.SetActive(true);

            //Set trail renderer in parent to true
            other.GetComponentInParent<TrailRenderer>().enabled = true;

            //Set layer of aircraft to touchable
            other.gameObject.layer = 0;
        }
    }
}
