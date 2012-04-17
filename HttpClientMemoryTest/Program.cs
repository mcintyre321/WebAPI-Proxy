using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Web.Http.SelfHost;

namespace HttpClientMemoryTest
{
    public class Program
    {
        private static bool exit;

        public static void Main(string[] args)
        {

            var timer = new Timer(Report, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
            Console.SetCursorPosition(0, 10);
            Console.WriteLine(@"Press enter to send c:\halfgigfile.zip to http://localhost:3000/home/upload or type exit");

            while (true)
            {

                if (Console.ReadLine() == "exit") break;
                var httpCLient = new HttpClient();
                var fileStream = File.OpenRead(@"C:\halfgigfile.zip");
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:3000/home/upload")
                {
                    Content = new StreamContent(fileStream)
                };
                httpCLient.SendAsync(request).ContinueWith(s => { Console.WriteLine("sent"); fileStream.Close(); });
                Console.WriteLine("sending");

            }

        }

        private static void Report(object state)
        {
            var pos = new { Console.CursorLeft, Console.CursorTop };
            Console.SetCursorPosition(0, 0);
            Console.Write(string.Format("{0:n0}", Process.GetCurrentProcess().PagedMemorySize64 / 1024) + "kb            ");
            Console.SetCursorPosition(pos.CursorLeft, pos.CursorTop);
        }
    }


}
