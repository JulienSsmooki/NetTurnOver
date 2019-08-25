using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServ
{
    class Program
    {
        private static Socket _serverSocket = new Socket(
                                                AddressFamily.InterNetwork,
                                                SocketType.Stream,
                                                ProtocolType.Tcp);
        private static List<Socket> _clientSockets = new List<Socket>();
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
            Socket client = _serverSocket.EndAccept(result);
            _clientSockets.Add(client);
            Console.WriteLine("Client n°" + _clientSockets.Count + " is Connected.");
            client.BeginReceive(_buffer,
                                0,
                                _buffer.Length,
                                SocketFlags.None,
                                new AsyncCallback(ReceiveCallback),
                                client);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        private static void ReceiveCallback(IAsyncResult result)
        {
            Socket client = (Socket)result.AsyncState;
            int receive = client.EndReceive(result);
            byte[] dataBuf = new byte[receive];
            Array.Copy(_buffer, dataBuf, receive);

            ProcessDataFromReceive(client, dataBuf);
        }

        private static void ProcessDataFromReceive(Socket client, byte[] dataBuf)
        {
            string debugText = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine("Request receive : " + debugText);

            string request = string.Empty;
            if(debugText.ToLower() != "get time")
            {
                request = "Invalid request";
            }
            else
            {
                request = DateTime.Now.ToLongTimeString();
            }
            byte[] data = Encoding.ASCII.GetBytes(request);
            SendBuffer(client, data);

            client.BeginReceive(_buffer,
                                0,
                                _buffer.Length,
                                SocketFlags.None,
                                new AsyncCallback(ReceiveCallback),
                                client);
        }

        private static void SendBuffer(Socket client, byte[] data)
        {
            client.BeginSend(data,
                             0, data.Length,
                             SocketFlags.None,
                             new AsyncCallback(SendCallback),
                             client);

        }

        private static void SendCallback(IAsyncResult result)
        {
            Socket client = (Socket)result.AsyncState;
            client.EndSend(result);
        }
    }
}
