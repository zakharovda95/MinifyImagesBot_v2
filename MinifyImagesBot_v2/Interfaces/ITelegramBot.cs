namespace MinifyImagesBot_v2.Interfaces;

public interface ITelegramBot
{
    public void CreateTelegramClientAndRun(string? telegramKey);
}