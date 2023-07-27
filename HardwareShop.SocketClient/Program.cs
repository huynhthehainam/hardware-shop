using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using HardwareShop.Application.Dtos;

namespace HardwareShop.SocketClient
{

    public class WindowBackgroundService : BackgroundService
    {
        public class LoginResponse
        {
            public string AccessToken { get; set; } = string.Empty;

        }
        private class HttpResponse<T> where T : class, new()
        {

            public T Data { get; set; } = new T();

        }
        private readonly TargetConfiguration targetConfiguration;
        private readonly IHttpClientFactory httpClientFactory;
        public WindowBackgroundService(IHttpClientFactory httpClientFactory, IOptions<TargetConfiguration> options)
        {
            targetConfiguration = options.Value;

            this.httpClientFactory = httpClientFactory;
        }
        public static JsonSerializerOptions CamelOptions { get; set; } = new() { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Login

            var httpProtocol = targetConfiguration.IsSecured ? "https" : "http";
            var wsProtocol = targetConfiguration.IsSecured ? "wss" : "ws";
            LoginResponse? auth = null;
            using (var client = httpClientFactory.CreateClient())
            {
                client.BaseAddress = new Uri(httpProtocol + "://" + targetConfiguration.Host + ":" + targetConfiguration.Port.ToString());
                var loginResponse = await client.PostAsJsonAsync(targetConfiguration.LoginPath, new
                {
                    Username = targetConfiguration.UserName,
                    Password = targetConfiguration.Password
                }, CamelOptions);
                var data = await loginResponse.Content.ReadFromJsonAsync<HttpResponse<LoginResponse>>(CamelOptions);
                if (data != null)
                {
                    auth = data.Data;
                }
            }

            if (auth == null) throw new Exception("Cannot get authentication");
            var wsPath = wsProtocol + "://" + targetConfiguration.Host + ":" + targetConfiguration.Port.ToString() + targetConfiguration.HubPath + "?access_token=" + auth.AccessToken;
            var connection = new HubConnectionBuilder().WithUrl(wsPath).Build();
            int? chatSessionId = null;

            await connection.StartAsync();

            connection.On<List<ChatContactDto>>("InitContacts", (chatSessions) =>
            {
                Console.WriteLine($"InitContacts {chatSessions.Count}");
                var chatSession = chatSessions.FirstOrDefault(e => e.Name == "admin");
                if (chatSession != null)
                {
                    if (chatSession.Id == 0)
                    {
                        connection.InvokeAsync("JoinChatSession", chatSession.AffectedUserIds).Wait();
                    }
                    else
                    {
                        chatSessionId = chatSession.Id;
                    }
                }
            });
            connection.On<CreatedChatSessionDto>("SuccessfullyCreatedChatSession", (chatSession) =>
            {
                Console.WriteLine($"SuccessfullyCreatedChatSession {chatSession.Id}");
                chatSessionId = chatSession.Id;
            });

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var now = DateTime.Now;
                    if (chatSessionId != null)
                    {

                        Console.WriteLine($"Send message at {now.ToString()}");
                        await connection.InvokeAsync("SendChatMessage", chatSessionId, $"Sample message at {now.ToString()}");
                    }
                    else
                    {
                        Console.WriteLine($"Connecting at {now.ToString()}");
                    }





                    await Task.Delay(TimeSpan.FromMilliseconds(targetConfiguration.Interval), stoppingToken);
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception)
            {
                Environment.Exit(1);
            }
            Console.WriteLine("End end");

        }
    }
    public class TargetConfiguration
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5000;
        public string HubPath { get; set; } = "/chatHub";
        public string LoginPath { get; set; } = "/api/auth/login";
        public bool IsSecured { get; set; } = false;
        public int Interval { get; set; } = 1000;
        public string UserName { get; set; } = "admin1";
        public string Password { get; set; } = "123";
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
            {
                services.Configure<TargetConfiguration>(config =>
                {

                });

                services.AddHttpClient();
                services.AddHostedService<WindowBackgroundService>();

            })
                .Build().Run();
        }
    }
}