using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchBot.DTOs;
using TwitchBot.Entities;

namespace TwitchBot.Interfaces
{
    public interface IDiscordBotApiService
    {
        Task<UserDto> GetUser(TwitchChat chat, string username);
        void SendNewMessage(string username, string message, TwitchChat chat);
    }
}