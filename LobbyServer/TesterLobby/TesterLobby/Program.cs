using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TesterLobby
{
    class Program
    {
        private static Socket _clientSocket = new Socket(
                                                  AddressFamily.InterNetwork,
                                                  SocketType.Stream,
                                                  ProtocolType.Tcp);
        private static byte[] _buffer = new byte[1024];

        static void Main(string[] args)
        {
            Console.Title = "Client";
            LoopConnect(args[0], Convert.ToInt32(args[1]));

            LoopSend();

            Console.ReadLine();
        }

        private static void LoopSend()
        {
            while(true)
            {
                Console.Write("Enter request : ");
                string req = Console.ReadLine();
                byte[] dataBuf = Encoding.ASCII.GetBytes(req);
                _clientSocket.Send(dataBuf);

                byte[] receivedBuf = new byte[1024];
                int dataSize = _clientSocket.Receive(receivedBuf);
                byte[] data = new byte[dataSize];
                Array.Copy(receivedBuf, data, dataSize);
                Console.WriteLine("Received : " + Encoding.ASCII.GetString(data));
            }
        }

        private static void LoopConnect(string servAdress, int servPort)
        {
            int attempts = 0;
            while(!_clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(new IPEndPoint(
                                         (Dns.GetHostAddresses(servAdress))[0],
                                         servPort));
                }
                catch(SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Connection attempts : " + 
                                      attempts +
                                      " | IP : " +
                                      servAdress + 
                                      " | Port : " + 
                                      servPort + 
                                      " |");
                }
            }

            Console.Clear();
            Console.WriteLine("Connected after " + attempts + " attempts.");
        }
    }
}
