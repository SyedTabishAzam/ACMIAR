using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Globalization;

public class MathCalculations : MonoBehaviour
{

    // Use this for initialization
    //HorizontalDistance equals 207.99136 NM
    //Or 385.2 Km
    const double leftlong = 70;
    const double bottomlat = 30;
    const double rightlong = 74;
    const double toplat = 34;


    const double resolutionY = 10240;
    static bool init = false;
    const double resolutionX = 10240;
    DateTimeOffset dto;
    public static float verticalDistance;
    public static float horizontalDistance;
    public static double scaledDownX;
    public static double scaledDownY;
    public static double scaledDownZ;
    public static string missionStart = "10:25 AM"; 
    List<List<float>> drraw = new List<List<float>>();

    //Return distance in meters
    public static float Calc(float lat1, float lon1, float lat2, float lon2)
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

    public static void ExercieArea(float[] point)
    { 


        float[] meh =Convert(point[0], point[1], 1000f);
        Debug.Log(meh[0].ToString() + " "  +meh[1].ToString() +" " +  meh[2].ToString());
        
    }


    private static void Init()
    {
        init = true;
        verticalDistance = Calc((float)bottomlat, (float)leftlong, (float)toplat, (float)leftlong);
        horizontalDistance = Calc((float)bottomlat, (float)leftlong, (float)bottomlat, (float)rightlong);
        scaledDownX = 2.0f / resolutionX;
        scaledDownY =  2.0f / resolutionY;
        scaledDownZ =  2.0f/ resolutionY;
    }
    void Start()
    {
        Init();

    }

    public static float[] MaxBounds()
    {
        if (!init)
            Init();
        float[] xy = new float[2];
        xy[0] = horizontalDistance;
        xy[1] = verticalDistance;
        return xy;
    }

    //given a lat long, calculate distance coordinates from origon (dx, dy).
    //given distance coordinates , returan coordinates in terms of resolution array.
    //ScaledDownX scales the resolution of 10240 to scale of 2 (left to right is 2 unit)
    public static float[] Convert(double lat1, double long1, double alt)
    {
        if (!init)
            Init();
        float dx = Calc((float)bottomlat, (float)leftlong, (float)bottomlat, (float)long1);
        float dy = Calc((float)bottomlat, (float)leftlong, (float)lat1, (float)leftlong);
        float dz = ConvertFeettoMeter(alt);
        //DistanceToLatlong(new Vector2(dy/1000, dx/1000)); 

        float[] rxy = new float[3];
        rxy[0] = (float)scaledDownX *((float)resolutionX * dx / horizontalDistance);
        rxy[1] = (float)scaledDownY *((float)resolutionY * dy / verticalDistance);
        rxy[2] = (float)scaledDownZ * ConvertMetertoPixel(dz, false) * 5;
        //DistanceToLatlong(new Vector2(rxy[0], rxy[1]));

        XYtoLatLong(rxy[0],rxy[1]);
        //Debug.Log(resolutionY.ToString() + " " + verticalDistance.ToString() + " " + resolutionX.ToString() + " "+ horizontalDistance.ToString());
        //Debug.Log(rxy[0].ToString() + " , " + rxy[1].ToString() + "," + rxy[2].ToString());
        return rxy;
    }


    public static float ConvertFeettoMeter(double feet)
    {
        return (float)feet / 3.2808399f;
    }

    public static float ConvertMetertoPixel(float meter, bool isVertical)
    {
        if (isVertical)
        {
            return meter * (float)resolutionY / verticalDistance;

        }
        else
        {
            //Debug.Log((float)resolutionX / horizontalDistance);
            return meter * (float)resolutionX / horizontalDistance;
        }
    }

    public static float ConvertUnityToMeters(float unityPos, bool isVertical)
    {
       // float newRange = (unityPos + 1f) * 0.5f; // Convert from -1 to 1 Range to 0 to 1 range
        if(isVertical)
        {
            return unityPos * verticalDistance;
        }
        else
        {
            return unityPos * horizontalDistance;
        }
    }

    public static float ConvertMetersToUnity(float meters,bool isVertical)
    {
        float meterPercent;
        if (isVertical)
        {
            meterPercent = meters / verticalDistance;
        }
        else
        {
            meterPercent = meters / horizontalDistance;
        }
        return meterPercent;
    }
   

    public static string ConvertTime(double t, int offset=0)
    {
        
       DateTimeOffset dto = DateTimeOffsetExtensions.FromUnixTimeSeconds((long)t).AddHours(offset);
       
        if (dto.Hour < DateTime.Parse(missionStart).Hour)
        {
            dto = dto.AddHours(5); //GMT + 5 offset 
        }
        DateTimeOffset dt = DateTimeOffset.Parse(dto.ToString());
        return dt.ToString("HH:mm:ss");
    }

    /*void OutputAltitude(double feet)
    {
        var meter = ConvertFeettoMeter(feet);
        var pixel = ConvertMetertoPixel(meter,false) *5;

    }*/

    //-------------------------------------------------------//
    //assuming i have mouse bounds around the terrain 
    //resolution is 1024  
    float leftx, rightx, topy, bottomy;
     

    //calculate percentage of click 
    public Vector2 PercentageCink(Vector3 mouseinput)
    {
        float totalrangex = rightx - leftx;
        float totalrangey = topy - bottomy;
        float diffx = mouseinput.x - leftx;
        float diffy = mouseinput.y - bottomy;

        Vector2 percentage = new Vector2(diffx / totalrangex * 100, diffy / totalrangey * 100);
        return percentage;
    }

    public Vector2 PercentageToDistance(Vector3 percentage)
    {
        Vector2 distance = new Vector2(percentage.x *(float)horizontalDistance, percentage.y * (float)verticalDistance);
        return distance;
    }

    public static Vector2 XYtoLatLong(float x, float y)
    {
        if (!init)
            Init();

        double percentageX = x / resolutionX;
        double differenceX = rightlong - leftlong;
        double valueLong = leftlong + (percentageX * differenceX);

        double percentageY = y / resolutionY;
        double differenceY = toplat - bottomlat;
        double valueLat = bottomlat + (percentageY * differenceY);

        //Debug.Log("this was Lat long " + (float)valueLat + " " + (float)valueLong);

        return new Vector2((float)valueLat, (float)valueLong);
    }

  
}


  
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset FromUnixTimeSeconds(this long seconds)
        {
            var dateTimeOffset = new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            dateTimeOffset = dateTimeOffset.AddSeconds(seconds);
            return dateTimeOffset;
        }

        public static long ToUnixTimeSeconds(this DateTimeOffset dateTimeOffset)
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (dateTimeOffset.ToUniversalTime() - unixStart).Ticks;
            return unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }
    }



