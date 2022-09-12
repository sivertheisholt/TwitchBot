using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TwitchBot.Handlers
{
    public class TwitchConnection
    {
        private readonly TcpClient _client;
        private readonly TwitchIrcHandler _ircHandler;
        public readonly string TwitchName;

        public TwitchConnection(string ip, int port, string twitchName)
        {
            TwitchName = twitchName;
            _client = new TcpClient(ip, port);
            _ircHandler = new TwitchIrcHandler(_client);
        }

        public TcpClient GetClient()
        {
            return _client;
        }
        public TwitchIrcHandler GetTwitchIrcHandler()
        {
            return _ircHandler;
        }
        
    }
}