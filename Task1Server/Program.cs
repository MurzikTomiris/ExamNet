using System.Net.Sockets;
using System.Net;

namespace Task1Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int port = 9000;
            TcpListener server = new TcpListener(IPAddress.Any, port);

            server.Start();
            Console.WriteLine("Сервер запущен, ожидаем подключения...");

            using (TcpClient client = server.AcceptTcpClient())
            using (NetworkStream stream = client.GetStream())
            {
                for (int i = 0; i < 4; i++) // Ожидаем 4 файла
                {
                    // Получаем длину имени файла
                    byte[] fileNameLenBytes = new byte[4];
                    stream.Read(fileNameLenBytes, 0, 4);
                    int fileNameLen = BitConverter.ToInt32(fileNameLenBytes, 0);

                    // Получаем имя файла
                    byte[] fileNameBytes = new byte[fileNameLen];
                    stream.Read(fileNameBytes, 0, fileNameBytes.Length);
                    string fileName = System.Text.Encoding.UTF8.GetString(fileNameBytes);

                    // Получаем длину содержимого файла
                    byte[] fileContentLenBytes = new byte[4];
                    stream.Read(fileContentLenBytes, 0, 4);
                    int fileContentLen = BitConverter.ToInt32(fileContentLenBytes, 0);

                    // Получаем содержимое файла
                    byte[] fileContent = new byte[fileContentLen];
                    stream.Read(fileContent, 0, fileContent.Length);

                    // Сохраняем файл на диске
                    File.WriteAllBytes(fileName, fileContent);
                    Console.WriteLine($"Файл {fileName} сохранен.");
                }

                // Отправляем клиенту статус сохранения
                byte[] status = System.Text.Encoding.UTF8.GetBytes("Файлы успешно сохранены.");
                stream.Write(status, 0, status.Length);
            }

            server.Stop();
        }
    }
}