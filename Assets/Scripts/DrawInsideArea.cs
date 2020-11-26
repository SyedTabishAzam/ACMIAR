using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawInsideArea : MonoBehaviour {

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uv;
    List<Vector3> drawVertices;
    List<Vector2> drawUV;
    List<int> drawTriangle;
    public bool doOneTime = false;
    float maxRayDistance = 0f;
    Vector3 up;
    // Use this for initialization
    void Start () {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        uv = mesh.uv;
        drawVertices = new List<Vector3>();
        drawUV = new List<Vector2>();
        drawTriangle = new List<int>();
        up = transform.TransformDirection(Vector3.up);

        Debug.Log(triangles.Length);
        Debug.Log(vertices.Length);
        Debug.Log(uv.Length);
       
    }

    // Update is called once per frame
    void Update()
    {
        if(doOneTime)
        {
            int triangleCount = 1;
            drawVertices.Clear();
            for (var i = 0; i < vertices.Length; i++)
            {
                if(InDrawArea(vertices[i]))
                {
                    triangleCount++;
                    drawVertices.Add(vertices[i]);
                    drawUV.Add(uv[i]);
                   
                }
                if(triangleCount%4 == 0)
                {
                    drawTriangle.Add(triangles[i-3]);
                }
            }
            doOneTime = false;
            mesh.Clear();
            mesh.vertices = drawVertices.ToArray();
            mesh.triangles = drawTriangle.ToArray();
            mesh.uv = drawUV.ToArray();
        }
        DrawRay();
    }
       
    public void DrawRay()
    {
        for(int i =0;i<drawVertices.Count;i=i+2)
        {
            if (i <= drawVertices.Count)
            {
                Debug.DrawRay(drawVertices[i], up * maxRayDistance,Color.yellow);
            }
        }
    }

    bool InDrawArea(Vector3 vertix)
    {
        
        RaycastHit hit;
        if (Physics.Raycast(vertix, up, out hit))
        {

            if(hit.transform.name=="DrawArea")
            {
               
                if(hit.distance>maxRayDistance)
                {
                    maxRayDistance = hit.distance;
                }
                return true;
            }
            
        }
       return false;
    }
}
