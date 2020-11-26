using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.EventSystems;
using System.Threading;
using UnityEngine.UI;


public class LiveAcmiDataReciever : MonoBehaviour
{
 
    public  class ACMIdata
    {
        public float Time { get; set; }
        public Int32 TailID { get; set; }
        public Int32 AircraftType { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Altitude { get; set; }
        public float Roll { get; set; }
        public float Pitch { get; set; }
        public float Heading { get; set; }
        public float MachNo { get; set; }
        public float Speed { get; set; }
        public float G { get; set; }
        public float Aoa { get; set; }

    }

    public Text localTimeText;
    //public static ACMIdata liveData; // 20 Aug
    public static UdpClient receivingUdpClient;
    public IPEndPoint RemoteIpEndPoint;
    //string ipAddress = "172.25.103.109";
    string ipAddress = "192.168.0.110";
    bool isDataSent;
    public static Dictionary<Int32, ACMIdata> liveDataPackets = new Dictionary<Int32, ACMIdata>();

    void Start()
    {
        receivingUdpClient = new UdpClient(12345);
        RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 0);
        //liveData = new ACMIdata(); //20 Aug
    }

    void Update()
    {
        isDataSent = IsDataBeingSentCheck();
        if (isDataSent)
            RecieveData();

    }

    public bool IsDataBeingSentCheck()
    {
        if (receivingUdpClient.Available > 0 )
        {
          //  Debug.Log("Data is Being Sent From AirCrafts...");
            return true;
        }

      //  Debug.Log("No Data Being Sent From AirCrafts!");
        return false;
    }

    public void RecieveData()
    {
        Byte[] recData = receivingUdpClient.Receive(ref RemoteIpEndPoint);

        ACMIdata liveData = new ACMIdata();

        if (recData.Length == 289)
        {
            liveData.Time = BitConverter.ToSingle(recData, 5);
            liveData.TailID = BitConverter.ToInt32(recData, 9);
            liveData.AircraftType = BitConverter.ToInt32(recData, 13);
            liveData.Latitude = BitConverter.ToSingle(recData, 17);
            liveData.Longitude = BitConverter.ToSingle(recData, 21);
            liveData.Altitude = BitConverter.ToSingle(recData, 25);
            liveData.Roll = BitConverter.ToSingle(recData, 29);
            liveData.Pitch = BitConverter.ToSingle(recData, 33);
            liveData.Heading = BitConverter.ToSingle(recData, 37);
            liveData.MachNo = BitConverter.ToSingle(recData, 41);
            liveData.Speed = BitConverter.ToSingle(recData, 45);
            liveData.G = BitConverter.ToSingle(recData, 49);
            liveData.Aoa = BitConverter.ToSingle(recData, 53);

            //Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<DATA PACKET>>>>>>>>>>>>>>>>>>>>>\n");
            //Debug.Log("Time:\t\t" + MathCalculations.ConvertTime(liveData.Time));

            //Debug.Log("Tail ID:\t\t" + liveData.TailID);
            //Debug.Log("Aircraft Type:\t\t " + liveData.AircraftType);
            //Debug.Log("Latitude:\t\t" + liveData.Latitude);
            //Debug.Log("Longitude:\t\t" + liveData.Longitude);
            //Debug.Log("Altitude:\t\t" + liveData.Altitude);
            //Debug.Log("Roll:\t\t" + liveData.Roll);
            //Debug.Log("Pitch:\t\t" + liveData.Pitch);
            //Debug.Log("Heading:\t\t" + liveData.Heading);
            //Debug.Log("Mach No:\t\t" + liveData.MachNo);
            //Debug.Log("Speed:\t\t" + liveData.Speed);
            //Debug.Log("G:\t\t" + liveData.G);
            //Debug.Log("Aoa:\t\t" + liveData.Aoa);
            //Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<DATA PACKET>>>>>>>>>>>>>>>>>>>>>\n");

            //DICTIONARY KA KAM
            var key = liveData.TailID;
            liveDataPackets[key] = liveData;


            ExecuteEvents.Execute<IliveDataRecieved>(GameObject.Find("LiveDataManager"), null, (x, y) => x.liveDataRecieved(liveDataPackets[key]));
            localTimeText.text = MathCalculations.ConvertTime(liveDataPackets[key].Time);
        }
        
             
       
    }

}

