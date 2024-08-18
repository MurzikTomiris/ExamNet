using System;
using System.IO;
using System.Net;
using System.Text;

namespace Task4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Создаем HttpListener
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            Console.WriteLine("HTTP сервер запущен. Ожидание запросов на http://localhost:8080/");

            while (true)
            {
                // Ожидание входящего запроса
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/calculate")
                {
                    string responseString = ProcessRequest(request);
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                else
                {
                    // Возвращаем главную страницу с формой
                    string responseString = GetMainPage();
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }

        static string GetMainPage()
        {
            return @"
                <html>
                <body>
                    <h1>Surface Area Calculator</h1>
                    <form method='post' action='/calculate'>
                        Length: <input type='text' name='length' /><br/>
                        Width: <input type='text' name='width' /><br/>
                        Height: <input type='text' name='height' /><br/>
                        <input type='submit' value='Calculate' />
                    </form>
                </body>
                </html>";
        }

        static string ProcessRequest(HttpListenerRequest request)
        {
            // Читаем входные данные
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                string formData = reader.ReadToEnd();
                var parameters = ParseFormData(formData);

                // Валидация входных данных
                if (!double.TryParse(parameters["length"], out double length) || length <= 0 ||
                    !double.TryParse(parameters["width"], out double width) || width <= 0 ||
                    !double.TryParse(parameters["height"], out double height) || height <= 0)
                {
                    return @"
                        <html><body>
                        <h1>Error: Invalid input</h1>
                        <p>Please enter valid positive numbers for length, width, and height.</p>
                        </body></html>";
                }

                // Вычисление площади поверхности
                double surfaceArea = 2 * (length * width + width * height + height * length);

                return $@"
                    <html><body>
                    <h1>Surface Area Calculation</h1>
                    <p>The surface area of the rectangular parallelepiped is: {surfaceArea} square units.</p>
                    </body></html>";
            }
        }

        static System.Collections.Specialized.NameValueCollection ParseFormData(string formData)
        {
            var result = new System.Collections.Specialized.NameValueCollection();

            string[] pairs = formData.Split('&');
            foreach (string pair in pairs)
            {
                string[] parts = pair.Split('=');
                if (parts.Length == 2)
                {
                    result.Add(WebUtility.UrlDecode(parts[0]), WebUtility.UrlDecode(parts[1]));
                }
            }

            return result;
        }
    }
}