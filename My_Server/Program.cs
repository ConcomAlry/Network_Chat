using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace My_Server
{  
    class Program
    {
        static int ClientConnections = 0;
        static NetworkStream[] streams = new NetworkStream[ClientConnections];

        static void SendMessageToClient(int ID)
        {           
            byte[] data = new byte[1024];
            int bytes;
            for (; ; Thread.Sleep(75))
            {
                if (streams[ID].DataAvailable)
                {
                    StringBuilder builder = new StringBuilder();
                    do
                    {
                        bytes = streams[ID].Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (streams[ID].DataAvailable);
                    string message = builder.ToString();
                    Console.WriteLine(message);
                    data = Encoding.Unicode.GetBytes(message);
                    for (int i = 0; i < ClientConnections; i++)
                    {
                        streams[i].Write(data, 0, data.Length);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            TcpListener listeningSocket = new TcpListener(IPAddress.Any, 7000);
            listeningSocket.Start();
            Console.WriteLine("Server started");

            for (; ; Thread.Sleep(75))
            {
                TcpClient mySocket = listeningSocket.AcceptTcpClient();
                Console.WriteLine("Client connected");
                NetworkStream stream = mySocket.GetStream();
                string ConnectMessage = "Connect...\n";
                byte[] BytesWrite = Encoding.Unicode.GetBytes(ConnectMessage);
                stream.Write(BytesWrite, 0, BytesWrite.Length);
                ClientConnections++;
                Array.Resize<NetworkStream>(ref streams, ClientConnections);
                streams[ClientConnections - 1] = stream;
                Thread th = new Thread(delegate () { SendMessageToClient(ClientConnections - 1); });
                th.Start();
            }
        }
    }
}
