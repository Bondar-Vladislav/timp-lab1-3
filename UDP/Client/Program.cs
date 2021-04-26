using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Порт: ");
            int portNumber = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Адрес");
            var address = Console.ReadLine();
            UdpClient client = new UdpClient();
            client.Client.SendBufferSize = client.Client.ReceiveBufferSize = 1000 * 64;
            var endPoint = new IPEndPoint(IPAddress.Parse(address), portNumber);
            client.Send(new byte[1] { 1 }, 1, endPoint);
            var res = client.Receive(ref endPoint);
            endPoint.Port = BitConverter.ToInt32(res, 0);
            client.Connect(endPoint);
            while (true)
            {
                res = client.Receive(ref endPoint);
                var size = BitConverter.ToInt64(res, 0);
                res = client.Receive(ref endPoint);
                var name = Encoding.Unicode.GetString(res);
                    client.Send(new byte[1] { 1 }, 1);
                    long readed = 0;
                    var fs = File.OpenWrite(name);
                    client.Client.ReceiveTimeout = 1000;
                    try
                    {
                        while (readed < size)
                        {
                            res = client.Receive(ref endPoint);
                            fs.Write(res, 0, res.Length);
                            readed += res.Length;
                        }
                    }
                    catch (SocketException exc) when (exc.SocketErrorCode == SocketError.TimedOut)
                    { }
                    fs.Close();
                    client.Client.ReceiveTimeout = 0;
                    Console.WriteLine($"Файл {name} скачан");
            }
        }
    }
}

