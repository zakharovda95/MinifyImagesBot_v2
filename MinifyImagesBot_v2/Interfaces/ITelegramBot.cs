namespace MinifyImagesBot_v2.Interfaces;

internal interface ITelegramBot
{
    Task CreateTelegramClientAndRun(string? telegramKey);
}