using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TangentCircles : CircleTangent
{

    public GameObject _circlePrefab; 

    //private GameObject _innerCircleGO, _outerCircleGO, _TangentCircleGO;
    //private Vector4 _innerCircle, _outerCircle;
    //private float _tangentCircleRadius;


    public float _degree;
    public float _numberOfCircles;
    public float _distanceBetweenCircles;
    
    public GameObject _linePrefab;
    public GameObject linesParent;
    
    public GameObject circlesParent;
    private float outerCircleRadius;
    private GameObject outerCircle;
    BullEyeHandle buleyehand;


    //public bool redraw;
    // Use this for initialization
    void Start () {
        //_innerCircleGO = (GameObject)Instantiate(_circlePrefab);
        //_outerCircleGO = (GameObject)Instantiate(_circlePrefab);
        //_TangentCircleGO = (GameObject)Instantiate(_circlePrefab);
    }

    // Update is called once per frame
    void Update () {

        //if (redraw == false) {
        //    return;
        //}

        //_innerCircleGO.transform.position = new Vector3(_innerCircle.x,_innerCircle.y,_innerCircle.z);
        //_innerCircleGO.transform.localScale = new Vector3(_innerCircle.w, _innerCircle.w, _innerCircle.w) * 2;

        //_outerCircleGO.transform.position = new Vector3(_outerCircle.x, _outerCircle.y, _outerCircle.z);
        //_outerCircleGO.transform.localScale = new Vector3(_outerCircle.w, _outerCircle.w, _outerCircle.w) * 2;

        //_TangentCircleGO.transform.position = GetRotatedTangent(_degree, _outerCircle.w) + _outerCircleGO.transform.position;
        //_TangentCircleGO.transform.localScale = new Vector3(_tangentCircleRadius,_tangentCircleRadius,_tangentCircleRadius * 2);
        //deleteAllCircles();
        //drawCircles();
        //deleteAllLines();
        //drawLines();
    }

   public void deleteAllLines() {
        foreach (Transform child in linesParent.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

   public void drawLines()
    {

        if (_degree <= 0)
        {
            return;
        }
        int numberOfLines = (int)(360 / _degree);
        var degree = _degree;
        for (int i = 0; i < numberOfLines; i++)
        {
            var line = (GameObject)Instantiate(_linePrefab);
            var lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, Vector3.zero);
            var position = GetRotatedTangent(degree, outerCircleRadius); /*+ new Vector3(outerCircleRadius, outerCircleRadius, outerCircleRadius)*/
            lineRenderer.SetPosition(1, position);
            degree += _degree;
            line.transform.SetParent(linesParent.transform);
            
        }
    }
    //Create function here and use degree and outercircleRadius for scale angle

   public void deleteAllCircles()
    {
        foreach (Transform child in circlesParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void drawCricleAndlines()
    { 
        drawCircles();
        drawLines();
    }
   public void drawCircles()
    {
        //NM Conversion
        _distanceBetweenCircles = _distanceBetweenCircles / 20;

        var distanceBetweenCircles = _distanceBetweenCircles;

        for (int i = 0; i < _numberOfCircles; i++)
        {
            var circle = (GameObject)Instantiate(_circlePrefab);
            circle.transform.position = Vector3.zero;
            circle.transform.localScale = new Vector3(distanceBetweenCircles, distanceBetweenCircles, distanceBetweenCircles) * 2;
            
            circle.transform.SetParent(circlesParent.transform);
            if (i == _numberOfCircles - 1) {
                outerCircle = circle;
                outerCircleRadius = distanceBetweenCircles;
            }
        distanceBetweenCircles += _distanceBetweenCircles;
    }

    }
}
