namespace MinifyImagesBot_v2.Interfaces;

internal interface ITelegramBot
{
    void CreateTelegramClientAndRun(string? telegramKey);
}