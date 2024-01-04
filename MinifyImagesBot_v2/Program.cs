using ImageMagick;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinifyImagesBot_v2.Classes;
using MinifyImagesBot_v2.Interfaces;
using MinifyImagesBot_v2.Services;

var configurationBuilder = new ConfigurationBuilder();
configurationBuilder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json");
var configuration = configurationBuilder.Build();


var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<IAppSettings>(_=> new AppSettingsService(configuration: configuration));

var serviceProvider = serviceCollection.BuildServiceProvider();

var settings = serviceProvider.GetRequiredService<IAppSettings>();

MagickNET.Initialize();

var telegramKey = settings.GetTelegramKey();
var telegramBot = new TelegramBot();
telegramBot.CreateTelegramClientAndRun(telegramKey: telegramKey);

while (true)
{
    Thread.Sleep(10000);
}