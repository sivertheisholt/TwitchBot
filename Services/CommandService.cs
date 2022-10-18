using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchBot.DTOs;
using TwitchBot.Entities;
using TwitchBot.Handlers;
using TwitchBot.Interfaces;

namespace TwitchBot.Services
{
    public class CommandService : ICommandService
    {
        private readonly IDiscordBotApiService _botApiService;
        public CommandService(IDiscordBotApiService botApiService)
        {
            _botApiService = botApiService;
        }

        public async void HandleCommand(TwitchChat chat, string username, string message, TwitchIrcHandler twitchIrcHandler)
        {
            var split = message.Split(" ");
            var command = split[0];
            switch(command.ToLower())
            {
                case "!me":
                    var user = await _botApiService.GetUser(chat, username);
                    if(user == null)
                    {
                        await twitchIrcHandler.SendMessage("Sorry, but you dont exist. Contact Wondyr for support.", chat.TwitchName);
                    } else {
                        await twitchIrcHandler.SendMessage($"@{username} you are level {user.Level}, have sent {user.Messagecount} message and have {user.Xp} XP.", chat.TwitchName);
                    }
                    break;
                case "!translate":
                    var messageSend = "";
                    for (int i = 2; i < split.Length; i++)
                    {
                        messageSend += " " + split[i];
                    }
                    var dto = new DiscordCommandDto {CommandTrigger = command.Remove(0, 1), CommandTriggerParam = split[1], Username = username, CommandMessage = messageSend};
                    _botApiService.SendCommand(chat, dto);
                    break;
                case "!insult":
                    var insultDto = new DiscordCommandDto {CommandTrigger = command.Remove(0, 1), CommandTriggerParam = "", Username = username, CommandMessage = ""};
                    _botApiService.SendCommand(chat, insultDto);
                    break;
                default:
                    Console.WriteLine("Command does not exist");
                    break;
            }
        }
    }
}