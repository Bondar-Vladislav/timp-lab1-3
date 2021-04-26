using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Diagnostics;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите имя сервера:");
            NamedPipeServerStream server = new NamedPipeServerStream(Console.ReadLine(), PipeDirection.InOut);
            Process myProcess = new Process();
            myProcess.StartInfo.FileName = @"C:\Users\3000v\OneDrive\Рабочий стол\NamedPipes\Client\bin\Debug\Client.exe";
            myProcess.Start();
            server.WaitForConnection();
            Console.WriteLine("Клиент подключен");
            try
            {
                while (true)
                {
                    Console.WriteLine("Введите путь к файлу:");
                    var file = Console.ReadLine();
                    if (!File.Exists(file))
                        continue;
                    string fileName = Path.GetFileName(file);
                    server.WriteByte((byte)fileName.Length);
                    var fileNameBytes = Encoding.Unicode.GetBytes(fileName);
                    server.Write(fileNameBytes, 0, fileNameBytes.Length);
                    using (var stream = File.OpenRead(file))
                    {
                        var lengthBytes = BitConverter.GetBytes(stream.Length);
                        server.Write(lengthBytes, 0, lengthBytes.Length);
                        if (server.ReadByte() == 1)
                        {
                            Console.WriteLine("Отправка файла");
                            while (stream.Position < stream.Length)
                            {
                                stream.CopyTo(server, 1024 * 1024);
                            }
                            Console.WriteLine("Файл отправлен");
                        }
                }   }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
