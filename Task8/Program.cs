using System.IO.Compression;
using System.Net.Mail;
using System.Net;
using Renci.SshNet;
using FluentFTP;
using System;

namespace Task8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string sftpHost = "test.rebex.net";
            string sftpUser = "demo";
            string sftpPassword = "password";
            string sftpRemoteFile = "readme.txt";

            string emailTo = "murzik.tomiris@mail.ru";
            string smtpHost = "smtp.mail.ru";
            string smtpUser = "murzik.@mail.ru";
            string smtpPassword = "code";

            string ftpHost = "ftp.dlptest.com";
            string ftpUser = "dlpuser";
            string ftpPassword = "rNrKYTX9g7z3RgJRmxWuGHbeu";
            string ftpRemotePath = "readme.zip";

            using (var sftpClient = new SftpClient(sftpHost, 22, sftpUser, sftpPassword))
            {
                sftpClient.Connect();

                using (var sftpStream = sftpClient.OpenRead(sftpRemoteFile))
                using (var memoryStream = new MemoryStream())
                {
                    // Архивируем файл в памяти
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        var zipEntry = zipArchive.CreateEntry(Path.GetFileName(sftpRemoteFile));

                        using (var entryStream = zipEntry.Open())
                        {
                            sftpStream.CopyTo(entryStream);
                        }
                    }


                    // Отправляем заархивированный файл по SMTP
                    SendEmailWithAttachment(emailTo, smtpHost, smtpUser, smtpPassword, memoryStream, "readme.zip");

                    // Отправляем заархивированный файл по FTP
                    byte[] fileBytes = memoryStream.ToArray();
                    UploadToFtp(ftpHost, ftpUser, ftpPassword, ftpRemotePath, fileBytes);
                }

                sftpClient.Disconnect();
            }
        }

        static void SendEmailWithAttachment(string to, string smtpHost, string smtpUser, string smtpPassword, MemoryStream attachmentStream, string attachmentName)
        {
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(smtpUser);
                mail.To.Add(to);
                mail.Subject = "Загруженный и заархивированный файл";
                mail.Body = "Файл был загружен с SFTP, заархивирован и отправлен вам по электронной почте.";

                // Добавляем файл как вложение
                attachmentStream.Position = 0;
                var attachment = new Attachment(attachmentStream, attachmentName);
                mail.Attachments.Add(attachment);

                using (var smtpClient = new SmtpClient(smtpHost, 587))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mail);
                }
            }
        }

        static void UploadToFtp(string ftpHost, string ftpUser, string ftpPassword, string remotePath, byte[] fileBytes)
        {
            FtpClient ftpClient = new FtpClient();
            ftpClient.Host = ftpHost;
            ftpClient.Credentials = new NetworkCredential(ftpUser, ftpPassword);
            ftpClient.Connect();
            var status = ftpClient.UploadBytes(fileBytes, remotePath);

            Console.WriteLine("Архив загружен на FTP сервер.");

 
        }
    }
}