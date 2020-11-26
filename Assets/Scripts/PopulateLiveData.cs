using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.UI;

public class PopulateLiveData : MonoBehaviour, IliveDataRecieved
{
    public GameObject C130Prefab;
    public GameObject F7Prefab;
    public GameObject F16Prefab;
    public GameObject JF17Prefab;
    public GameObject MiragePrefab;
    public GameObject AirCraftParent;
    public GameObject[] DropDownReferences;

    List<float> dashboardData = new List<float>();
    LiveAcmiDataReciever.ACMIdata data;
    Dictionary<Int32, GameObject> aircrafts = new Dictionary<Int32, GameObject>();

    public static List<string> AllActivetailIds = new List<string>();

    Vector3 updatedPosition;
    Quaternion updatedRotation;

    float Time;
    Int32 TailID;
    Int32 AircraftType;
    float Latitude;
    float Longitude;
    float Altitude;
    float Roll;
    float Pitch;
    float Heading;
    float MachNo;
    float Speed;
    float G;
    float Aoa;


    void FixedUpdate()
    {
        liveDataRecieved(data);
    }


    public void liveDataRecieved(LiveAcmiDataReciever.ACMIdata liveData)
    {

        if (LiveAcmiDataReciever.receivingUdpClient.Available > 0)
        {
            Time = liveData.Time;
            TailID = liveData.TailID;
            AircraftType = liveData.AircraftType;
            Latitude = liveData.Latitude;
            Longitude = liveData.Longitude;
            Altitude = liveData.Altitude;
            Roll = liveData.Roll;
            Pitch = liveData.Pitch;
            Heading = liveData.Heading;
            MachNo = liveData.MachNo;
            Speed = liveData.Speed;
            G = liveData.G;
            Aoa = liveData.Aoa;
            SpawnAircraftLive(liveData);

        }

    }


    public void SpawnAircraftLive(LiveAcmiDataReciever.ACMIdata liveData)
    {
        //DICTIONARY KA KAM
        var key = liveData.TailID;
        GameObject aircraft;

        var hasObject = aircrafts.TryGetValue(key, out aircraft);

        if (hasObject == false)
        {

            //instantiate new 
            aircraft = InstantiateAircraft(liveData);
            aircrafts.Add(key, aircraft);

            //List with Active Tail ids
            AllActivetailIds.Add(key.ToString());

        }
        else
        {
            // update lat long on aircraft gameobject 
            SetAircraftsPosition(liveData, aircraft);

        }

        aircraft.GetComponent<AircraftDataManager>().liveDataPacket = liveData;

        //For ALL Refrences
        foreach (GameObject reference in DropDownReferences)
        {

            reference.GetComponent<Dropdown>().ClearOptions();
            reference.GetComponent<Dropdown>().AddOptions(AllActivetailIds);

        }

    }

    public GameObject InstantiateAircraft(LiveAcmiDataReciever.ACMIdata liveData)
    {
        var aircraftType = liveData.AircraftType;
        var tailId = liveData.TailID;
        GameObject instantiatedAirCraft = null;

        switch (aircraftType)
        {
            case 1:
                instantiatedAirCraft = Instantiate(F16Prefab, transform.position, Quaternion.identity) as GameObject;
                instantiatedAirCraft.transform.parent = AirCraftParent.transform;
                instantiatedAirCraft.name = "F16 " + tailId;
                break;
            case 3:
                instantiatedAirCraft = Instantiate(MiragePrefab, transform.position, Quaternion.identity) as GameObject;
                instantiatedAirCraft.transform.parent = AirCraftParent.transform;
                instantiatedAirCraft.name = "Mirage " + tailId;
                break;
            case 5:
                instantiatedAirCraft = Instantiate(JF17Prefab, transform.position, Quaternion.identity) as GameObject;
                instantiatedAirCraft.transform.parent = AirCraftParent.transform;
                instantiatedAirCraft.name = "JF-17 " + tailId;
                break;
            case 8:
                instantiatedAirCraft = Instantiate(C130Prefab, transform.position, Quaternion.identity) as GameObject;
                instantiatedAirCraft.transform.parent = AirCraftParent.transform;
                instantiatedAirCraft.name = "C130 " + tailId;
                break;

            default:
                instantiatedAirCraft = Instantiate(F7Prefab, transform.position, Quaternion.identity) as GameObject;
                instantiatedAirCraft.transform.parent = AirCraftParent.transform;
                instantiatedAirCraft.name = "F7 " + tailId;
                break;
        }


        return instantiatedAirCraft;

    }


    public void SetAircraftsPosition(LiveAcmiDataReciever.ACMIdata liveData, GameObject aircraft)
    {

        float[] xzy = MathCalculations.Convert(liveData.Latitude, liveData.Longitude, liveData.Altitude);

        float x = xzy[0];
        float z = xzy[1];
        float y = xzy[2];

        updatedPosition = new Vector3(x, y, z);
        aircraft.transform.localPosition = updatedPosition;


        float roll = liveData.Roll;
        float pitch = liveData.Pitch;
        float heading = liveData.Heading;

        heading = heading - 180;
        updatedRotation = Quaternion.Euler(pitch, heading, -roll);
        aircraft.transform.localRotation = updatedRotation;

    }

}
