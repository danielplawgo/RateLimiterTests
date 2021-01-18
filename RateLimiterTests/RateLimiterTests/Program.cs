using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using ComposableAsync;
using Flurl.Http;
using Flurl.Http.Configuration;
using RateLimiter;

namespace RateLimiterTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await ConsoleExample();
            //await HttpClientExample();
            //await FlurlGlobalExample();
            //await FlurlClientExample();
        }

        static async Task ConsoleExample()
        {
            var timeConstraint = TimeLimiter.GetFromMaxCountByInterval(5, TimeSpan.FromSeconds(1));

            for (int i = 0; i < 20; i++)
            {
                await timeConstraint;
                Console.WriteLine($"{DateTime.Now:MM/dd/yyy HH:mm:ss.fff}");
            }
        }

        static async Task HttpClientExample()
        {
            var handler = TimeLimiter
                .GetFromMaxCountByInterval(5, TimeSpan.FromSeconds(10))
                .AsDelegatingHandler();
            
            var client = new HttpClient(handler);

            for (int i = 0; i < 20; i++)
            {
                var content = await client.GetStringAsync("https://visualstudio.plawgo.pl");
                Console.WriteLine($"Page downloaded: {content.Length}");
            }
        }

        static async Task FlurlGlobalExample()
        {
            FlurlHttp.Configure(settings => {
                settings.HttpClientFactory = new HttpClientFactory();
            });

            var url = "https://visualstudio.plawgo.pl";

            for (int i = 0; i < 20; i++)
            {
                var content = await url.GetStringAsync();
                Console.WriteLine($"Page downloaded: {content.Length}");
            }
        }

        static async Task FlurlClientExample()
        {
            var url = "https://visualstudio.plawgo.pl";

            var cli = new FlurlClient(url).Configure(settings => {
                settings.HttpClientFactory = new HttpClientFactory();
            });

            for (int i = 0; i < 20; i++)
            {
                var content = await cli.Request().GetStringAsync();
                Console.WriteLine($"Page downloaded: {content.Length}");
            }
        }
    }

    public class HttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            var handler = TimeLimiter
                .GetFromMaxCountByInterval(5, TimeSpan.FromSeconds(10))
                .AsDelegatingHandler();

            handler.InnerHandler = base.CreateMessageHandler();

            return handler;
        }
    }
}
