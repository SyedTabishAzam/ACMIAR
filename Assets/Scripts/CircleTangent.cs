using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTangent : MonoBehaviour {

    protected Vector3 GetRotatedTangent(float degree , float Scale)
    {
        double angles = degree * Mathf.Deg2Rad;
        float x = Scale * (float)System.Math.Sin(angles);
        float z = Scale * (float)System.Math.Cos(angles);
        return new Vector3(x, 0, z);
    }




}
