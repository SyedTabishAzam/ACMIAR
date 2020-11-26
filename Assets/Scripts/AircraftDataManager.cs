using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftDataManager : MonoBehaviour {


	public LiveAcmiDataReciever.ACMIdata liveDataPacket;
    List<float> dashboardData = new List<float>();

    public string ACtype;
    
    public string[] GetData()
	{
        //Time Latitude    Longitude BaroAlt Roll Pitch   Heading MachNo  CAS G   AOA TrueAirSpeed    IAS AGL BRM Radar Range
        // "A/C Type", "Heading", "Altitude", "CAS", "TAS", "IAS", "Mach No.", "G", "AOA", "Latitude", "Longitude", "AGL", "Pitch"


        Debug.Log("Tail id / Type / Name :: :: ::" + liveDataPacket.TailID.ToString()+"   " + liveDataPacket.AircraftType.ToString() + "   " + gameObject.name);

        switch (liveDataPacket.AircraftType)
        {
            case 1:
                ACtype = "F16 ";
                break;

            case 3:
                ACtype = "Mirage ";
                break;

            case 5:
                ACtype = "JF-17 ";
                break;

            case 8:
                ACtype = "C130 " ;
                break;

            default:
                ACtype = "F7 " ;
                break;
        }

		List<string> temp = new List<string>();

		temp.Add(liveDataPacket.TailID.ToString());
        temp.Add(ACtype.ToString());
        temp.Add(liveDataPacket.Heading.ToString());
		temp.Add(liveDataPacket.Altitude.ToString());
		temp.Add("0");
		temp.Add("0");
		temp.Add("0"); 
		temp.Add(liveDataPacket.MachNo.ToString());
		temp.Add(liveDataPacket.G.ToString());
		temp.Add(liveDataPacket.Aoa.ToString());
		temp.Add(liveDataPacket.Latitude.ToString());
		temp.Add(liveDataPacket.Longitude.ToString());
		temp.Add("0");
		temp.Add(liveDataPacket.Pitch.ToString());
        temp.Add(liveDataPacket.Speed.ToString());
        //
        // int[] lst = { 6, 3, 8, 11, 12, 7, 9, 10, 1, 2, 13, 5 };

        //if (dashboardData.Count == 15)
        //{
        //    foreach (int x in lst)
        //    {
        //        temp.Add(dashboardData[x].ToString());
        //    }
        //}

        //return temp.ToArray();
        //
        gameObject.GetComponentInChildren<SpriteTextFactory>().ChangeText(liveDataPacket.TailID.ToString());
        gameObject.GetComponentInChildren<SpriteTextFactory>().DrawText();

        return temp.ToArray();
	}
    //For DeadSymbol scale
   


}
