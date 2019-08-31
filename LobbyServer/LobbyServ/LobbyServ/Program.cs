using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LobbyServ
{
    class Program
    {
        private static Socket _serverSocket = new Socket(
                                                AddressFamily.InterNetwork,
                                                SocketType.Stream,
                                                ProtocolType.Tcp);
        public struct Client
        {
            public Socket clientSocket;
            public long uid;
            public LobbyProto lobbyState; 
        }
        private static List<Client> _clients = new List<Client>();
        private static byte[] _buffer = new byte[1024];

        static void Main(string[] args)
        {
            Console.Title = "Server";
            SetupServer(Convert.ToInt32(args[0]));
            Console.ReadLine();
        }

        public static void SetupServer(int port)
        {
            Console.WriteLine("Setting up server on port " + port + " ...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            _serverSocket.Listen(1);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        private static void AcceptCallBack(IAsyncResult result)
        {
            Socket clientSocket = _serverSocket.EndAccept(result);
            Client newClient;
            newClient.clientSocket = clientSocket;
            newClient.lobbyState = LobbyProto.Unknown;
            newClient.uid = _clients.Count > 0 ? _clients[_clients.Count - 1].uid + 1 : 0;
            _clients.Add(newClient);
            Console.WriteLine("Client: " + _clients.Count + " n°" + newClient.uid + " is Connected.");
            newClient.clientSocket.BeginReceive(_buffer,
                                0,
                                _buffer.Length,
                                SocketFlags.None,
                                new AsyncCallback(ReceiveCallback),
                                newClient);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        private static void ReceiveCallback(IAsyncResult result)
        {
            Client client = (Client)result.AsyncState;
            int receive = client.clientSocket.EndReceive(result);
            byte[] dataBuf = new byte[receive];
            Array.Copy(_buffer, dataBuf, receive);

            ProcessDataFromReceive(client, dataBuf);
        }

        private static void ProcessDataFromReceive(Client client, byte[] dataBuf)
        {
            string debugText = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine("Request receive : " + debugText);

            NetMessage newMsg = new NetMessage();
            newMsg = SerializeUtils.DeserializeMsg(dataBuf);

            byte[] tmpdata = new byte[0];
            switch (newMsg.head.lobbyProto)
            {
                case LobbyProto.Connection:
                    tmpdata = ConnectionReceived(client);
                    break;
               
                default: break;
            }
            
            
            SendBuffer(client, tmpdata);

            client.clientSocket.BeginReceive(_buffer,
                                0,
                                _buffer.Length,
                                SocketFlags.None,
                                new AsyncCallback(ReceiveCallback),
                                client);
        }

        private static void SendBuffer(Client client, byte[] data)
        {
            client.clientSocket.BeginSend(data,
                             0, data.Length,
                             SocketFlags.None,
                             new AsyncCallback(SendCallback),
                             client);

        }

        private static void SendCallback(IAsyncResult result)
        {
            Client client = (Client)result.AsyncState;
            client.clientSocket.EndSend(result);
        }

        private static byte[] ConnectionReceived(Client client)
        {
            NetMessage sendBackMsg = new NetMessage();
            sendBackMsg.head.lobbyProto = LobbyProto.Connection;
            sendBackMsg.body = BitConverter.GetBytes(client.uid);

            return SerializeUtils.SerializeMsg(sendBackMsg);
        }
    }
}
