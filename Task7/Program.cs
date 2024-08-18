using HtmlAgilityPack;

namespace Task7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "https://beeline.kz/ru";

            var phones = ParsePhonesFromWebsite(url);

            Console.WriteLine("Найденные телефоны:");
            foreach (var phone in phones)
            {
                Console.WriteLine(phone);
            }
        }

        static string[] ParsePhonesFromWebsite(string url)
        {
            // Используем HttpClient для скачивания HTML-страницы
            using (HttpClient client = new HttpClient())
            {
                var html = client.GetStringAsync(url).Result;

                // Загружаем HTML в HtmlDocument
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);

                // Ищем все элементы с тегом <a>, которые могут содержать номера телефонов
                var phoneNodes = document.DocumentNode.SelectNodes("//a[contains(@href, 'tel:')]");

                if (phoneNodes != null)
                {
                    // Извлекаем содержимое href атрибута, где находятся номера телефонов
                    return phoneNodes.Select(node => node.GetAttributeValue("href", "")
                                                .Replace("tel:", "")
                                                .Trim())
                                     .Distinct()
                                     .ToArray();
                }

                return Array.Empty<string>();
            }
        }
    }
}