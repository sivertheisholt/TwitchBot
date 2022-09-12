using Microsoft.AspNetCore.Mvc;
using TwitchBot.DTOs;
using TwitchBot.Interfaces;

namespace TwitchBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TwitchController : Controller
    {
        private readonly ILogger<TwitchController> _logger;
        private readonly ITwitchIrcService _twitchIrcService;

        public TwitchController(ILogger<TwitchController> logger, ITwitchIrcService twitchIrcService)
        {
            _twitchIrcService = twitchIrcService;
            _logger = logger;
        }

        [HttpPost]
        [Route("levelup")]
        public async Task<IActionResult> SendLevelUp(TwitchLevelUpMessageDto messageDto)
        {
            _twitchIrcService.SendTwitchMessage(messageDto.Message, messageDto.TwitchName);
            return Ok();
        }
    }
}