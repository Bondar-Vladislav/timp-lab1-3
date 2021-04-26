using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {

        static async Task Main (string [] args)
        {
            Console.Write ("1. Запуск Server:");
            Console.WriteLine("Введите номер порта");
            int portNumber = Convert.ToInt32(Console.ReadLine());
            Server server = new Server (portNumber);
            var _ = Task.Factory.StartNew (server.Start, TaskCreationOptions.LongRunning);
            server.ClientConnected += () =>
            {
                Console.Clear ();
                for (int i = 0; i < server.Clients.Count; i++)
                {
                    var client = server.Clients [i];
                }
            };
            Console.Write ("Ожидание подключения клиента");
            Process myProcess = new Process ();
            myProcess.StartInfo.FileName = @"C:\Users\3000v\OneDrive\Рабочий стол\TCP - без winapi\Client\bin\Debug\Client.exe";
            myProcess.Start ();
            await server.WaitClientConnection ();
            Console.WriteLine("Введите номер клиента");
            int clientIndex = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Укажите путь к файлу");
            string pathToFile = Console.ReadLine();
            await server.Clients [clientIndex].SendFile (pathToFile);
            myProcess.Kill ();
        }
    }
}