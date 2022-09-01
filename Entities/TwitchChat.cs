using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchBot.Entities
{
    public class TwitchChat
    {
        public string TwitchName { get; set; }
        public string BotBaseUrl { get; set; }
        public string BotBasePort { get; set; }
    }
}