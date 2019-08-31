using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public InputField ipField;
    public InputField portField;
    private Thread ReceiveThread;

    // Start is called before the first frame update
    void Start()
    {
        ipField.text = "127.0.0.1";
        portField.text = "12345";
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private static Socket _clientSocket = new Socket(
                                                  AddressFamily.InterNetwork,
                                                  SocketType.Stream,
                                                  ProtocolType.Tcp);
    private static byte[] _buffer = new byte[1024];

    public void Connect()
    {
       StartCoroutine(CoroutineConnect(ipField.text, Convert.ToInt32(portField.text)));
        ReceiveThread = new Thread(AsyncReceive) { Name = "LobbyClientReceiveThread" };
        ReceiveThread.Start();
    }

    private void AsyncReceive()
    {
        while (IsConnected())
        {
            byte[] receivedBuf = new byte[1024];
            int dataSize = _clientSocket.Receive(receivedBuf);
            byte[] data = new byte[dataSize];
            Array.Copy(receivedBuf, data, dataSize);
            Debug.Log("Received : " + SerializeUtils.DeserializeMsg(data));
            Thread.Sleep(100);
        }
        ReceiveThread.Abort();
    }

    public void SendGetTime()
    {
        Debug.Log("Send connection setting request.");
        NetMessage newMsg = new NetMessage();
        newMsg.head.lobbyProto = LobbyProto.Connection;
        newMsg.body = BitConverter.GetBytes(-1);
        _clientSocket.Send(SerializeUtils.SerializeMsg(newMsg));
    }


    

    private IEnumerator CoroutineConnect(string servAdress, int servPort)
    {
        if (!IsConnected())
        {
            try
            {
                _clientSocket.Connect(new IPEndPoint(
                                     (Dns.GetHostAddresses(servAdress))[0],
                                     servPort));
            }
            catch(SocketException)
            {
                Debug.Log("Not connected to server, try again...");
            }
            yield return new WaitForSeconds(.1f);
        }

        Debug.Log("Connected to server!");
        yield break; 
    }

    public bool IsConnected()
    {
        return _clientSocket.Connected;
    }

}
