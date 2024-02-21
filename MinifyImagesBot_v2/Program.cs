using ImageMagick;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinifyImagesBot_v2.Classes;
using MinifyImagesBot_v2.Interfaces;
using MinifyImagesBot_v2.Services;

var hostBuilder = Host.CreateApplicationBuilder(args);
var env = hostBuilder.Environment.EnvironmentName;
var aer = hostBuilder.Environment;

var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
var configuration = hostBuilder.Configuration;
var services = hostBuilder.Services;

configuration.SetBasePath(baseDirectory).AddJsonFile($"appsettings.{env}.json");

services.AddSingleton<IAppSettings>(_ => new AppSettingsService(configuration: configuration));

var host = hostBuilder.Build();

MagickNET.Initialize();

var settingsService = host.Services.GetRequiredService<IAppSettings>();

var tgKey = settingsService.GetTelegramKey();
await TelegramBot.CreateTelegramClientAndRun(telegramKey: tgKey);

Console.ReadKey();

await host.RunAsync();

/*while (true)
{
    Thread.Sleep(100000);
}*/