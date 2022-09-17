using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitchBot.DTOs
{
    public class UserDto
    {
        public string Username { get; set; }
        public int Messagecount { get; set; }
        public int Xp { get; set; }
        public int Level { get; set; }
    }
}