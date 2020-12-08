using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class ServerManager : NetworkBehaviour
{

    bool hostStarted = false;
    bool clientStarted = false;
    private string _iniFilePath;
    public GameObject serverPanel;
    public GameObject localTimeHolder;
    private string _ip;
    private int _port;

    private void ExtractServerSettings()
    {
        _iniFilePath = Application.dataPath + "\\Resources\\ServerData\\ServerSettings.csv";
        string fileData = System.IO.File.ReadAllText(_iniFilePath);
        string[] lines = fileData.Split("\n"[0]);

        for (int i = 0; i < lines.Length; i++)
        {

            string[] lineData = (lines[i].Trim()).Split(':');

            if (lineData[0].Trim() == "ServerAddress")
            {
                _ip = lineData[1].Trim();

            }
            else if (lineData[0].Trim() == "ServerPort")
            {
                _port = int.Parse(lineData[1].Trim());
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        ExtractServerSettings();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartHost()
    {
        if (!hostStarted && !clientStarted)
        {
            gameObject.GetComponent<NetworkManager>().StartHost();
            hostStarted = true;
        }
    }

    public void StartClient()
    {
        ExtractServerSettings();
        if (!clientStarted)
        {
            gameObject.GetComponent<NetworkManager>().networkAddress = _ip;
            gameObject.GetComponent<NetworkManager>().networkPort = _port;
            gameObject.GetComponent<NetworkManager>().StartClient();
            serverPanel.SetActive(false);
            localTimeHolder.GetComponent<Text>().text = "Connected to Server";
            clientStarted = true;
        }
        else
        {
            gameObject.GetComponent<NetworkManager>().StopClient();
            serverPanel.SetActive(true);
            localTimeHolder.GetComponent<Text>().text = "Disconnected from Server";
            clientStarted = false;
        }

    }

    public override void OnStartClient()
    {
        localTimeHolder.GetComponent<Text>().text = "Connected to Server";
    }

    private void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        localTimeHolder.GetComponent<Text>().text = "Disconnected from Server";
    }

    

}
