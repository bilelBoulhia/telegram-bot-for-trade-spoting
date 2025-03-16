using Binance.Spot;
using CryptoExchange.Net.Authentication;
using Telegram.Bot;
using TradingBot;
using TradingBot.Helpers;
using TradingTelegramService.services;
using TradingTelegramService.Services;
using TradingTelegramService.Worker;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddHostedService<SpotTradingNotificationWorker>();
builder.Services.AddSingleton<Market>();
builder.Services.AddSingleton<SignalService>();
builder.Services.AddSingleton<BotRepo>();
builder.Services.AddSingleton<SpotingRepo>();
builder.Services.AddBinance(builder.Configuration.GetSection("Binance"));
builder.Services.AddBinance(options => {
 
    options.ApiCredentials = new ApiCredentials("APIKEY", "APISECRET");
});

builder.Services.AddSingleton<TelegramBotClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var botToken = configuration["Settings:token"];
    if (string.IsNullOrEmpty(botToken))
    {
        throw new ArgumentNullException("token is missing");
    }
    return new TelegramBotClient(botToken);
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});


app.UseAuthorization();

app.MapControllers();

app.Run();
