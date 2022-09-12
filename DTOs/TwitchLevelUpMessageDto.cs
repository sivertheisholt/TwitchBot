using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchBot.DTOs
{
    public class TwitchLevelUpMessageDto
    {
        public string TwitchName { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
    }
}