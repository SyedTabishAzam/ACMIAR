using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Csharpudp : MonoBehaviour
{

    public string IP = "172.25.103.109";
    public int port = 12345;
    // Thread ReceiveData;

    private void Start()
    {
        ConnectToServer();
    }
    private void ConnectToServer()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);

       // ThreadStart ReceiveData = null;

        Thread receiveThread = new Thread(ReceiveData);

        receiveThread.IsBackground = true;
        receiveThread.Start();

        UdpClient client = new UdpClient();

        try
        {
            string text;
            do
            {
                text = Console.ReadLine();

                if (text.Length != 0)
                {
                    byte[] data = Encoding.UTF8.GetBytes(text);
                    client.Send(data, data.Length, remoteEndPoint);
                }
            } while (text.Length != 0);
        }
        catch (Exception err)
        {
            Console.WriteLine(err.ToString());
        }
        finally
        {
            client.Close();
        }
    }



    private static void ReceiveData()
    {
        UdpClient client = new UdpClient(12345);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                Console.WriteLine(">> " + text);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
        }
    }

}