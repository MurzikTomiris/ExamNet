using FluentFTP;
using FluentFTP.Rules;
using Renci.SshNet;
using System.IO;
using System.Net;

namespace Task3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Task3.DownloadAndSave();
            //Task3.DownloadAndUploadFtp();
            //Task3.DownloadAndUploadSftp(); rebex access denied 
        }

        class Task3
        {
            static string url = "https://beeline.kz/binaries/content/assets/public_offer/public_offer_ru.pdf";


            public static void DownloadAndSave()
            {
                string savePath = @"C:\Tomiris\Courses\Step\public_offer_ru.pdf";
                using (WebClient client = new WebClient())
                {
                    // Загружаем файл и сохраняем на диск
                    client.DownloadFile(url, savePath);

                    Console.WriteLine("Файл сохранен на диск.");
                }
            }

            public static void DownloadAndUploadFtp()
            {
                string ftpUrl = "ftp://ftp.dlptest.com/public_offer_ru.pdf";
                string ftpUser = "dlpuser";
                string ftpPassword = "rNrKYTX9g7z3RgJRmxWuGHbeu";

                using (WebClient client = new WebClient())
                {
                    // Устанавливаем учетные данные для FTP
                    client.Credentials = new NetworkCredential(ftpUser, ftpPassword);

                    // Загружаем файл как массив байтов
                    byte[] fileBytes = client.DownloadData(url);

                    // Загружаем файл на FTP сервер
                    client.UploadData(ftpUrl, WebRequestMethods.Ftp.UploadFile, fileBytes);

                    Console.WriteLine("Файл отправлен на FTP сервер.");
                }
            }

            public static void DownloadAndUploadSftp()
            {
                string url = "https://beeline.kz/binaries/content/assets/public_offer/public_offer_ru.pdf";
                string sftpUrl = "test.rebex.net";
                string sftpUser = "demo";
                string sftpPassword = "password";
                int sftpPort = 22;
                string sftpRemotePath = "/pub/example/public_offer_ru.pdf";

                using (WebClient client = new WebClient())
                {
                    // Загружаем файл как массив байтов
                    byte[] fileBytes = client.DownloadData(url);

                    using (MemoryStream ms = new MemoryStream(fileBytes))
                    {
                        using (var sftp = new SftpClient(sftpUrl, sftpPort, sftpUser, sftpPassword))
                        {
                            sftp.Connect();

                            // Загружаем файл на SFTP сервер из потока
                            sftp.UploadFile(ms, sftpRemotePath);

                            Console.WriteLine("Файл отправлен на SFTP сервер.");
                        }
                    }
                }
            }
        }
    }
}