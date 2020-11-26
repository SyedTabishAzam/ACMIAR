using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.EventSystems;
using System.Threading;

public class UdpDataReciever : MonoBehaviour
{
    
   public class ACMIdata
    {
        public float Time { get; set; }
        public Int32 TailID  { get; set; }
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

    // <-- This IP address is used for testing/development purpose only. Actual server IP is: 172.27.64.28
    // static string serverIPAddress = "172.27.64.28";// Actual server IP is: 172.27.64.2
    //static string serverIPAddress = "172.25.103.109";
    //static int dataPort = 3960; // UDP Port number

    //static string serverIPAddress = "172.25.103.109";
    static string serverIPAddress = "172.21.57.12";
    static int dataPort = 6666;
  
    
    void Start () {


        Connect();

    }

    public void Connect()
    {
        UdpClient client = new UdpClient();
        IPEndPoint remoteIp = new IPEndPoint(IPAddress.Parse(serverIPAddress), dataPort);


        try
        {
            client.Connect(remoteIp);
            Debug.Log("Connected!");

            //byte[] recdata = client.Receive(ref remoteIp);
            //Debug.Log("Random Recived Data = " + recdata[3]);
            

        }
        catch (Exception e)
        {

            Debug.Log("This Error Occurered while trying connection code: " + e.Message);
            Debug.Log("Not Connected!");


        }
    }
   
}