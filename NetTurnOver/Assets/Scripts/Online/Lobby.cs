///////////////////////////////////////////////////////////////////////////////
//                                                                           //
//@Autor : Julien Lopez                                                      //
//@Date : 01/09/2019                                                         //
//@Description : Lobby.cs                                                    //
//               Provide some function to connect socket to the lobby server //
//               and comminicate with it.                                    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
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
            Type type = typeof(NetMessage);
            Debug.Log("Received message.");
            ProcessReceiveMsg((NetMessage)SerializeUtils.DeserializeMsg(data));
            Thread.Sleep(100);
        }
        ReceiveThread.Abort();
    }

    private void ProcessReceiveMsg(NetMessage message)
    {
        switch(message.head.lobbyProto)
        {
            case LobbyProto.Connection:
                break;
            default:break;
        }
    }

    public void SendMsg(NetMessage message)
    {
        Debug.Log("Message sended.");
        _clientSocket.Send(SerializeUtils.SerializeMsg(message));
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
        SendMsg(new NetMessage(
                               new HeadMsg(LobbyProto.Connection),
                               BitConverter.GetBytes(-1)));
        yield break; 
    }

    public bool IsConnected()
    {
        return _clientSocket.Connected;
    }

}
