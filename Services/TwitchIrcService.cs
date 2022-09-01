using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchBot.Entities;
using TwitchBot.Handlers;
using TwitchBot.Interfaces;

namespace TwitchBot.Services
{
    public class TwitchIrcService : ITwitchIrcService
    {
        private readonly IConfiguration _config;
        private readonly List<TwitchChat> _chats = new List<TwitchChat>();
        private readonly string _twitchOAuth;
        static string ip = "irc.chat.twitch.tv";
        static int port = 6667;
        private readonly HttpClient _httpClient;

        public TwitchIrcService(IConfiguration config, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _config = config;

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Development")
            {
                _twitchOAuth = _config.GetSection("APP_SETTINGS")["TWITCH_IRC_OAUTH"];
                
                CreateChats(_config.GetSection("APP_SETTINGS")["TWITCH_CHATS"],
                    _config.GetSection("APP_SETTINGS")["DISCORD_BOT_API_BASEURL"],
                    _config.GetSection("APP_SETTINGS")["DISCORD_BOT_API_PORT"]);
            }
            else
            {
                _twitchOAuth = Environment.GetEnvironmentVariable("TWITCH_IRC_OAUTH");

                CreateChats(Environment.GetEnvironmentVariable("TWITCH_CHATS"),
                    Environment.GetEnvironmentVariable("DISCORD_BOT_API_BASEURL"),
                    Environment.GetEnvironmentVariable("DISCORD_BOT_API_PORT"));
            }

            _chats.ForEach(chat => {
                Thread trd = new Thread(new ThreadStart(() => StartConnection(chat)));
                trd.Start();
            });
        }
        
        private async void StartConnection(TwitchChat chat)
        {
            var tcpClient = new TcpClient(ip, port);
            var twitchHandler = new TwitchIrcHandler(tcpClient);
            twitchHandler.Login(_twitchOAuth, "Wondyrr");
            twitchHandler.JoinChat(chat.TwitchName);

            Console.WriteLine(chat.TwitchName);
            
            while (true)
            {
                var line = await twitchHandler.ReadMessage();
                
                HandleMessage(twitchHandler, chat, line);

            }
        }
        private void HandleMessage(TwitchIrcHandler handler, TwitchChat chat, string line)
        {
            Console.WriteLine(line);
            string[] split = line.Split(" ");
            if (split.Length < 1) return;

            Console.WriteLine(split);
            
            switch (split[1])
            {
                case "PRIVMSG":
                    int exclamationPointPosition = split[0].IndexOf("!");
                    string username = split[0].Substring(1, exclamationPointPosition - 1);
                    //Skip the first character, the first colon, then find the next colon
                    int secondColonPosition = line.IndexOf(':', 1); // the 1 here is what skips the first character
                    string message = line.Substring(secondColonPosition + 1); // Everything past the second colon
                    Console.WriteLine($"{username} said '{message}'");

                    SendHttpRequest(username, message, chat);
                    break;
                case "PING":
                    handler.Pong();
                    break;
                default:
                    Console.WriteLine("Something");
                    break;
            }
        }
        private async void SendHttpRequest(string username, string message, TwitchChat chat)
        {
            var dto = new {Username = username, Message = message};
            var jsonString = JsonSerializer.Serialize(dto);
            Console.WriteLine(jsonString);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            
            try{
                var result = await _httpClient.PostAsync($"{chat.BotBaseUrl}:{chat.BotBasePort}/TwitchChat/message", httpContent);
            } catch(HttpRequestException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Can't connect to API, skipping...");
            }
        }
        private void CreateChats(string twitchName, string baseUrl, string basePort)
        {
            if(twitchName.Contains(","))
            {
                var names = twitchName.Split(",");
                var baseUrls = baseUrl.Split(",");
                var basePorts = basePort.Split(",");

                for (int i = 0; i < names.Length; i++)
                {
                    _chats.Add(new TwitchChat(){TwitchName = names[i], BotBasePort = basePorts[i], BotBaseUrl = baseUrls[i]});
                }
            } else {
                _chats.Add(new TwitchChat(){TwitchName = twitchName, BotBaseUrl = baseUrl, BotBasePort = basePort});
            }
        }
    }
}