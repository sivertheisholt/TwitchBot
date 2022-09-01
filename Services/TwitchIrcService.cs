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
            await twitchHandler.Login(_twitchOAuth, "wondyrr");
            await twitchHandler.JoinChat(chat.TwitchName);

            var timeout = 1000;
            var reconnectionCount = 0;
            
            while (true)
            {
                var line = await twitchHandler.ReadMessage();
                
                try {
                    HandleMessage(twitchHandler, chat, line);
                } catch(Exception e)
                {
                    Console.WriteLine("Connection dropped, reconnecting in " + timeout);
                    tcpClient.Close();
                    Thread.Sleep(timeout);
                    timeout = timeout * 2;
                    reconnectionCount++;
                    tcpClient = new TcpClient(ip, port);
                    twitchHandler = new TwitchIrcHandler(tcpClient);
                    await twitchHandler.Login(_twitchOAuth, "wondyrr");
                    await twitchHandler.JoinChat(chat.TwitchName);
                }
            }
        }
        private async void HandleMessage(TwitchIrcHandler handler, TwitchChat chat, string line)
        {
            string[] split = line.Split(" ");
            
            if (split.Length < 1) return;
            
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
                    await handler.Pong(split[1]);
                    break;
                default:
                    Console.WriteLine(split[1]);
                    Console.WriteLine("This command is not supported yet");
                    break;
            }
        }
        private async void SendHttpRequest(string username, string message, TwitchChat chat)
        {
            // Sending message to discord bot
            var dto = new {Username = username, Message = message};
            var jsonString = JsonSerializer.Serialize(dto);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            
            try{
                var result = await _httpClient.PostAsync($"{chat.BotBaseUrl}:{chat.BotBasePort}/TwitchChat/message", httpContent);
            } catch(HttpRequestException e)
            {
                Console.WriteLine("Can't connect to API, skipping..." + e);
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