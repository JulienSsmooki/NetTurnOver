using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public InputField ipField;
    public InputField portField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(IsConnected())
            LoopSend();
    }

    private static Socket _clientSocket = new Socket(
                                                  AddressFamily.InterNetwork,
                                                  SocketType.Stream,
                                                  ProtocolType.Tcp);
    private static byte[] _buffer = new byte[1024];

    public bool Connect()
    {
        return LoopConnect(ipField.text, Convert.ToInt32(portField.text));
    }

    private void LoopSend()
    {
        Debug.Log("Enter request : ");
        string req = "get time";
        byte[] dataBuf = Encoding.ASCII.GetBytes(req);
        _clientSocket.Send(dataBuf);

        byte[] receivedBuf = new byte[1024];
        int dataSize = _clientSocket.Receive(receivedBuf);
        byte[] data = new byte[dataSize];
        Array.Copy(receivedBuf, data, dataSize);
        Debug.Log("Received : " + Encoding.ASCII.GetString(data));
    }

    private bool LoopConnect(string servAdress, int servPort)
    {
        int attempts = 0;
        if (!IsConnected())
        {
            try
            {
                attempts++;
                _clientSocket.Connect(new IPEndPoint(
                                     (Dns.GetHostAddresses(servAdress))[0],
                                     servPort));
            }
            catch (SocketException)
            {
                Debug.ClearDeveloperConsole();
                Debug.Log("Connection attempts : " +
                                  attempts +
                                  " | IP : " +
                                  servAdress +
                                  " | Port : " +
                                  servPort +
                                  " |");
            }
        }

        Debug.ClearDeveloperConsole();
        Debug.Log("Connected after " + attempts + " attempts.");
        return true;
    }

    public bool IsConnected()
    {
        return _clientSocket.Connected;
    }

}
