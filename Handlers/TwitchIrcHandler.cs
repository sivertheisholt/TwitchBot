using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TwitchBot.Handlers
{
    public class TwitchIrcHandler
    {
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        public TwitchIrcHandler(TcpClient client)
        {
            _reader = new StreamReader(client.GetStream());
            _writer = new StreamWriter(client.GetStream())  { NewLine = "\r\n", AutoFlush = true};
        }

        public async void Login(string password, string nick)
        {
            Console.WriteLine("Logging in");
            await _writer.WriteLineAsync($"PASS {password}");
            await _writer.WriteLineAsync($"NICK {nick}");
        }
        public async void JoinChat(string twitchName)
        {
            Console.WriteLine($"Joining chat: {twitchName}");
            await _writer.WriteLineAsync($"JOIN #{twitchName}");
        }
        public async void Pong()
        {
            Console.WriteLine("Sending PONG");
            await _writer.WriteLineAsync($"PONG");
        }
        public async Task<string> ReadMessage()
        {
            Console.WriteLine("Reading message");
            return await _reader.ReadLineAsync();
        }
    }
}