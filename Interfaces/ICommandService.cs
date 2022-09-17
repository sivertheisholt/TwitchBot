using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchBot.Entities;
using TwitchBot.Handlers;

namespace TwitchBot.Interfaces
{
    public interface ICommandService
    {
        void HandleCommand(TwitchChat chat, string username, string message, TwitchIrcHandler twitchIrcHandler);
    }
}