using TwitchBot.Helpers;
using TwitchBot.Interfaces;
using TwitchBot.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ITwitchIrcService, TwitchIrcService>();
builder.Services.AddSingleton<IDiscordBotApiService, DiscordBotApiService>();
builder.Services.AddSingleton<ICommandService, CommandService>();

var app = builder.Build();

var twitchService = app.Services.GetRequiredService<ITwitchIrcService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
