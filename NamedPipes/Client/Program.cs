using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите имя сервера");
            NamedPipeClientStream clientStream = new NamedPipeClientStream(Console.ReadLine()); 
            clientStream.Connect();
            Console.WriteLine("Подключение успешно");
            byte[] fileBuffer = new byte[1024 * 1024];
            try
            {
                while (true)
                {
                int length = clientStream.ReadByte();
                byte[] buffer = new byte[length * 2];
                clientStream.Read(buffer, 0, buffer.Length);
                string name = Encoding.Unicode.GetString(buffer);
                clientStream.Read(buffer, 0, buffer.Length);
                long size = BitConverter.ToInt64(buffer, 0);
                Console.WriteLine("Начинается загрузка файла");
                clientStream.WriteByte(1);
                long readed = 0;
                var fs = File.OpenWrite(name);
                 while (readed < size)
                 {
                    int count = clientStream.Read(fileBuffer, 0, fileBuffer.Length);
                    fs.Write(fileBuffer, 0, count);
                    readed += count;
                 }
                fs.Close();
                Console.WriteLine("Файл загружен");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
