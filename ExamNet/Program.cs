using FluentFTP;
using System.IO.Compression;
using System.Net;

namespace ExamNet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Task 2
            //FTP.UploadArchive();
            FTP.DownloadAndExtractArchive();
        }

        class FTP
        {
            static string url = "ftp://ftp.dlptest.com/";
            static string user = "dlpuser";
            static string password = "rNrKYTX9g7z3RgJRmxWuGHbeu";
            static string sourceDirectory = @"C:\Tomiris\Courses\Step\ExamNetFile";
            static string archiveName = "files.zip";

            public static void UploadArchive()
            {
                // Указываем путь к файлам и имя архива
                string sourceDirectory = @"C:\Tomiris\Courses\Step\ExamNetFile";
                string archivePath = Path.Combine(sourceDirectory, "files.zip");

                // Создаем архив из файлов
                using (FileStream zipToOpen = new FileStream(archivePath, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                    {
                        string[] files = { "sample1.txt", "sample2.txt", "sample3.txt", "sample4.txt" };
                        foreach (string file in files)
                        {
                            string filePath = Path.Combine(sourceDirectory, file);
                            archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                        }
                    }
                }


                // Отправляем архив на FTP-сервер
                FtpClient client = new FtpClient();
                client.Host = url;
                client.Credentials = new NetworkCredential(user, password);
                client.Connect();
                var status = client.UploadFile(archivePath, $"/{archiveName}");

                Console.WriteLine("Архив загружен на FTP сервер.");
            }

            public static void DownloadAndExtractArchive()
            {
                // Указываем путь для сохранения архива и извлеченных файлов
                string destinationDirectory = @"C:\Tomiris\Courses\Step\ExamNet";
                string archivePath = Path.Combine(destinationDirectory, "downloaded_files.zip");

                // Скачиваем архив с FTP-сервера
                FtpClient client = new FtpClient();
                client.Host = url;
                client.Credentials = new NetworkCredential(user, password);
                client.Connect();
                var status = client.DownloadFile(archivePath, $"/{archiveName}");
                Console.WriteLine("DownloadFile");

                // Разархивируем файл
                ZipFile.ExtractToDirectory(archivePath, destinationDirectory);
                Console.WriteLine("Файлы успешно извлечены.");
            }

        }
    }
}