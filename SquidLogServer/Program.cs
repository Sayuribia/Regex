using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace EventBusFuncion
{
    class Program
    {
        public static void Main()
        {
            Program main = new Program();
            var list = new List<Object>();
            TcpListener server = null;
            try
            {
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                server.Start();

                Byte[] bytes = new Byte[15000000];
                List<Object> data = null;

                while (true)
                {
                    Console.Write("Connecting... ");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Has been connected!");

                    data = null;

                    NetworkStream stream = client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var mStream = new MemoryStream();
                        var binFormatter = new BinaryFormatter();

                        mStream.Write(bytes, 0, bytes.Length);
                        mStream.Position = 0;

                        data = binFormatter.Deserialize(mStream) as List<Object>;
                        Console.WriteLine("Received: {0}", data);

                        break;
                    }
                    client.Close();

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            server.Stop();


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}
