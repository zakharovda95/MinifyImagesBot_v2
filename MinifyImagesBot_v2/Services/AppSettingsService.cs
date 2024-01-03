using Microsoft.Extensions.Configuration;
using MinifyImagesBot_v2.Interfaces;

namespace MinifyImagesBot_v2.Services;

internal sealed class AppSettingsService : IAppSettings
{
    private readonly IConfiguration _configuration;

    public AppSettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string? GetTelegramKey()
    {
        return _configuration.GetConnectionString("TELEGRAM_KEY");
    }
}