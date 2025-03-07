using Binance.Spot;

using Telegram.Bot;
using TradingBot;
using TradingTelegramService.Services;
using TradingTelegramService.Worker;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<SpotTradingNotificationWorker>();
builder.Services.AddSingleton<Market>();
builder.Services.AddSingleton<BotServices>();
builder.Services.AddSingleton<SpotingService>();
builder.Services.AddScoped<Helpers>();
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

builder.Services.AddSingleton<Helpers>();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
