using System.Net.Sockets;
using System.Net;

namespace Task1Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string folderPath = @"C:\Tomiris\Courses\Step\ExamNetFile";

            // Файлы, которые нужно отправить
            string[] files = {
                Path.Combine(folderPath, "sample1.txt"),
                Path.Combine(folderPath, "sample2.txt"),
                Path.Combine(folderPath, "sample3.txt"),
                Path.Combine(folderPath, "sample4.txt")
            };
            string server = "127.0.0.1"; // IP-адрес сервера
            int port = 9000; // Порт сервера

            using (TcpClient client = new TcpClient(server, port))
            using (NetworkStream stream = client.GetStream())
            {
                foreach (var file in files)
                {
                    byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(Path.GetFileName(file));
                    byte[] fileNameLenBytes = BitConverter.GetBytes(fileNameBytes.Length);

                    byte[] fileContent = File.ReadAllBytes(file);
                    byte[] fileContentLenBytes = BitConverter.GetBytes(fileContent.Length);

                    // Отправляем длину имени файла, имя файла, длину содержимого и содержимое файла
                    stream.Write(fileNameLenBytes, 0, fileNameLenBytes.Length);
                    stream.Write(fileNameBytes, 0, fileNameBytes.Length);
                    stream.Write(fileContentLenBytes, 0, fileContentLenBytes.Length);
                    stream.Write(fileContent, 0, fileContent.Length);
                }

                // Получаем статус сохранения от сервера
                byte[] response = new byte[256];
                int bytesRead = stream.Read(response, 0, response.Length);
                string status = System.Text.Encoding.UTF8.GetString(response, 0, bytesRead);

                Console.WriteLine("Ответ сервера: " + status);
            }
        }
    }
}