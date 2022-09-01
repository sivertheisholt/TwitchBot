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

        public async Task Login(string password, string nick)
        {
            Console.WriteLine("Logging in");
            await _writer.WriteLineAsync($"PASS {password}");
            await _writer.WriteLineAsync($"NICK {nick}");
        }
        public async Task JoinChat(string twitchName)
        {
            Console.WriteLine($"Joining chat: {twitchName}");
            await _writer.WriteLineAsync($"JOIN #{twitchName}");
        }
        public async Task Pong(string message)
        {
            Console.WriteLine("Sending PONG with message: " + message);
            await _writer.WriteLineAsync($"PONG {message}");
        }
        public async Task<string> ReadMessage()
        {
            Console.WriteLine("Reading message");
            return await _reader.ReadLineAsync();
        }
    }
}