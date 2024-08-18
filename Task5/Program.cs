using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;
using System.IO.Compression;
using System.Net.Mail;

namespace Task5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Task5
            //Mail.SendArchive();

            //Task6
            //Mail.MoveMail();

            //Task10
            Mail.ReadMail();

        }
    }

    public class Mail
    {
        public static void SendArchive()
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

            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress("murzik.@mail.ru");
                mail.To.Add("murzik.@mail.ru");
                mail.Subject = "subject sample";
                mail.Body = "<h1>body sample</h1>";
                Attachment attachment = new Attachment(archivePath);

                mail.Attachments.Add(attachment);
                using (System.Net.Mail.SmtpClient server = new System.Net.Mail.SmtpClient("smtp.mail.ru", 587))
                {
                    server.UseDefaultCredentials = false;
                    server.Credentials = new System.Net.NetworkCredential("murzik.@mail.ru", "code");
                    server.EnableSsl = true;
                    server.DeliveryMethod = SmtpDeliveryMethod.Network;
                    server.Send(mail);
                    server.Dispose();
                    Console.WriteLine("Send");
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        public static void MoveMail()
        {
            using (var client = new ImapClient())
            {
                client.Connect("imap.mail.ru", 993, true);
                client.Authenticate("murzik.@mail.ru", "code");

                // Открываем папку "Входящие"
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                // Ищем письмо по теме "subject sample"
                var query = SearchQuery.SubjectContains("subject sample");
                var uids = inbox.Search(query);

                if (uids.Count > 0)
                {
                    foreach (var uid in uids)
                    {
                        // Помечаем письмо как прочитанное
                        inbox.AddFlags(uid, MessageFlags.Seen, true);

                        // Папка для перемещения
                        var targetFolder = client.GetFolder("INBOX/Test");


                        // Перемещаем письмо в папку "Test"
                        inbox.MoveTo(uid, targetFolder);

                        Console.WriteLine("Письмо перемещено в папку 'Test'.");
                    }
                }
                else
                {
                    Console.WriteLine("Письмо с темой 'subject sample' не найдено.");
                }

                client.Disconnect(true);
            }
        }

        public static void ReadMail()
        {
            using (var client = new ImapClient())
            {
                // Подключение к IMAP-серверу Mail.ru
                client.Connect("imap.mail.ru", 993, true);
                client.Authenticate("murzik.@mail.ru", "code");

                // Открываем папку "Входящие"
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                // Получаем уникальные идентификаторы (UID) самых ранних 100 писем
                var uids = inbox.Search(SearchQuery.All).OrderBy(uid => uid).Take(100);

                if (uids.Any())
                {
                    // Выводим темы этих писем на экран
                    foreach (var uid in uids)
                    {
                        var message = inbox.GetMessage(uid);
                        Console.WriteLine($"Subject: {message.Subject}");
                    }
                }
                else
                {
                    Console.WriteLine("Писем не найдено.");
                }

                // Отключаемся от сервера
                client.Disconnect(true);
            }
        }
    }
}