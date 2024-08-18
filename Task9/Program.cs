using System.IO.Compression;

namespace Task9
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string zipUrl = "https://github.com/mbaibatyr/SEP_221_NET/archive/refs/heads/master.zip";
            string zipPath = "master.zip";
            string extractPath = "SEP_221_NET-master/SEP_221_NET-master";

            // 1. Скачать ZIP файл
            await DownloadFileAsync(zipUrl, zipPath);

            // 2. Извлечь содержимое ZIP файла
            ExtractZipFile(zipPath, extractPath);

            // 3. Прочитать содержимое файла .gitignore
            string gitignorePath = FindGitignoreFile(extractPath);
            if (!string.IsNullOrEmpty(gitignorePath))
            {
                await ReadGitignoreFileAsync(gitignorePath);
            }
            else
            {
                Console.WriteLine("Файл .gitignore не найден.");
            }

            // 4. Удалить ZIP файл и папку
            DeleteFile(zipPath);
            DeleteDirectory("SEP_221_NET-master");
        }

        static async Task DownloadFileAsync(string url, string path)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
            }
        }

        static void ExtractZipFile(string zipPath, string extractPath)
        {
            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }

        static string FindGitignoreFile(string rootPath)
        {
            // Поиск файла .gitignore в извлеченной папке и ее подпапках
            foreach (var file in Directory.GetFiles(rootPath, ".gitignore", SearchOption.AllDirectories))
            {
                return file;
            }
            return null;
        }

        static async Task ReadGitignoreFileAsync(string gitignorePath)
        {
            if (File.Exists(gitignorePath))
            {
                string content = await File.ReadAllTextAsync(gitignorePath);
                Console.WriteLine("Содержимое файла .gitignore:");
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine("Файл .gitignore не найден.");
            }
        }

        static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Console.WriteLine($"Файл {path} удален.");
            }
        }

        static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Console.WriteLine($"Папка {path} удалена.");
            }
        }
    }
}