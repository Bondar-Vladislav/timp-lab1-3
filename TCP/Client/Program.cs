using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{

    class Program
    {
        static async Task Main (string [] args)
        {
            Console.Write ("Запуск Client:");
            Console.WriteLine("Введите номер порта");
            int portNumber = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите IP");
            var address =Console.ReadLine();
            TcpClient client = new TcpClient ();
            try
            {
                client.Connect (address, portNumber);
            }
            catch (Exception)
            {
                Console.WriteLine ("Не удалось подключиться к серверу.");
                return;
            }
            var stream = client.GetStream ();
            client.ReceiveBufferSize = 1024 * 1024;
            var buffer = new byte [client.ReceiveBufferSize];
            var lengthBuffer = new byte [8];
            try
            {
                while (client.Connected)
                {
                    await stream.ReadAsync (lengthBuffer, 0, lengthBuffer.Length);
                    var size = BitConverter.ToInt64 (lengthBuffer, 0);
                        stream.WriteByte (1);
                        await stream.ReadAsync (lengthBuffer, 0, lengthBuffer.Length);
                        var c = await stream.ReadAsync (buffer, 0, (int) BitConverter.ToInt64 (lengthBuffer, 0));
                        var name = Encoding.Unicode.GetString (buffer, 0, c);
                        Console.Write ("Введите путь для сохранения файла: ");
                        string pathFile = Console.ReadLine ();
                        DirectoryInfo createFolder = new DirectoryInfo (pathFile);
                        createFolder.Create ();
                        var fs = File.OpenWrite ($"{pathFile}" + @"\" + name);
                        long readed = 0;
                        while (readed < size)
                        {
                            var count = await stream.ReadAsync (buffer, 0, buffer.Length);
                            readed += count;
                            await fs.WriteAsync (buffer, 0, count);
                        }
                        fs.Close ();
                        Console.Write ($"Файл {name} сохранен.");
                }
            }
            catch (SocketException)
            {
                Console.Write ("Сервер прервал соединие");
            }
        }
    }
}