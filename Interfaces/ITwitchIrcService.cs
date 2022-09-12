using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchBot.Handlers;

namespace TwitchBot.Interfaces
{
    public interface ITwitchIrcService
    {
        void SendTwitchMessage(string message, string twitchName);
    }
}