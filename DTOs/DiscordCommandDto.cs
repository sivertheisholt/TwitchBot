using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchBot.DTOs
{
    public class DiscordCommandDto
    {
        public string CommandTrigger { get; set; }
        public string CommandTriggerParam { get; set; }
        public string CommandMessage { get; set; }
        public string Username { get; set; }
    }
}