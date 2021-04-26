using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        public List<Client> Clients { get; set; }
        private UdpClient client;
        private UdpClient sendClient;
        private byte[] fileBuffer;
        public event Action ClientConnected;
        public Server(int port)
        {
            client = new UdpClient(port);
            Clients = new List<Client>();
            sendClient = new UdpClient(0);
            sendClient.Client.SendBufferSize = sendClient.Client.ReceiveBufferSize = 1000 * 64;
            fileBuffer = new byte[sendClient.Client.SendBufferSize];
        }
        public async Task SendFile(Client client, string fileName)
        {
            using (Stream stream = File.OpenRead(fileName))
            {
                byte[] lengthBuffer = BitConverter.GetBytes(stream.Length);
                await sendClient.SendAsync(lengthBuffer, 8, client.EndPoint);
                var namebytes = Encoding.Unicode.GetBytes(Path.GetFileName(fileName));
                await sendClient.SendAsync(namebytes, namebytes.Length, client.EndPoint); 
                if ((await sendClient.ReceiveAsync()).Buffer[0] == 1)
                {
                    Console.WriteLine("Отправка файла клиенту");
                    while (stream.Position < stream.Length)
                    {
                        int count = await stream.ReadAsync(fileBuffer, 0, fileBuffer.Length);
                        await sendClient.SendAsync(fileBuffer, count, client.EndPoint); 
                    }
                    Console.WriteLine("Файл отправлен");
                }
                else
                {
                    Console.WriteLine("Клиент не подтвердил отправку файла");
                }
            }

        }
        public async Task Start()
        {
            while (true)
            {
                var result = await client.ReceiveAsync();

                var c = new Client(result.RemoteEndPoint);
                var port = BitConverter.GetBytes((sendClient.Client.LocalEndPoint as IPEndPoint).Port);
                await client.SendAsync(port, 4, c.EndPoint);
                Clients.Add(c);
                ClientConnected?.Invoke();
            }
        }
        public async Task WaitClientConnection()
        {
            while (true)
            {
                if (Clients.Count > 0)
                {
                    return;
                }
                await Task.Delay(50);
            }
        }
    }
}
