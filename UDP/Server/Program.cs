using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Порт: ");
            int portNumber = Convert.ToInt32(Console.ReadLine());
            Server server = new Server(portNumber);
            server.ClientConnected += () =>
            {
                Console.Clear();
                for (int i = 0; i < server.Clients.Count; i++)
                {
                    var client = server.Clients[i];
                }
            };
            _ = Task.Factory.StartNew(server.Start, TaskCreationOptions.LongRunning);
            Console.WriteLine("Ожидание подключения клиентов");
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = @"C:\Users\3000v\OneDrive\Рабочий стол\UDP\Client\bin\Debug\Client.exe";
                myProcess.Start();
                await server.WaitClientConnection();
                Console.WriteLine("Клиент: ");
                int clientIndex = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Путь к файлу: ");
                string pathToFile = Console.ReadLine();
                await server.SendFile(server.Clients[clientIndex], pathToFile);
        }
    }
}
