using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchBot.DTOs
{
    public class TwitchSendMessageDto
    {
        public string TwitchName { get; set; }
        public string Message { get; set; }
    }
}