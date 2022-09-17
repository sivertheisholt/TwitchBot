using System.Text;
using System.Text.Json;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TwitchBot.DTOs;
using TwitchBot.Entities;
using TwitchBot.Interfaces;

namespace TwitchBot.Services
{
    public class DiscordBotApiService : IDiscordBotApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public DiscordBotApiService(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }
       
        public async void SendNewMessage(string username, string message, TwitchChat chat)
        {
            // Sending message to discord bot
            var dto = new {Username = username, Message = message};
            var jsonString = System.Text.Json.JsonSerializer.Serialize(dto);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            
            try{
                var result = await _httpClient.PostAsync($"{chat.BotBaseUrl}:{chat.BotBasePort}/TwitchChat/message", httpContent);
            } catch(HttpRequestException e)
            {
                Console.WriteLine("Can't connect to API, skipping..." + e);
            }
        }

        public async Task<UserDto> GetUser(TwitchChat chat, string username)
        {
            try{
                var result = await _httpClient.GetAsync($"{chat.BotBaseUrl}:{chat.BotBasePort}/users/{username}");

                if(result.IsSuccessStatusCode)
                {
                    // Read Response Content (this will usually be JSON content)
                    var content = await result.Content.ReadAsStringAsync();
                    JObject contentObject = JObject.Parse(content);
                    var resultString = JsonConvert.SerializeObject(contentObject);
                    var userResult = JsonConvert.DeserializeObject<UserDto>(resultString);
                    return userResult;
                }
                return null;
            } catch(HttpRequestException e)
            {
                Console.WriteLine("Can't connect to API or invalid response, skipping..." + e);
                return null;
            }
        }

    }
}