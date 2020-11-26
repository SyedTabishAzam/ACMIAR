using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionData : MonoBehaviour {

    private Vector2 LatLong;

    public void SetLatLong(Vector2 latLong)
    {
        LatLong = latLong;
    }

    public Vector2 GetLatLong()
    {
        return LatLong;
    }
}
