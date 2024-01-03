using MinifyImagesBot_v2.Enums;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;

namespace MinifyImagesBot_v2.Interfaces;

public interface ITelegramHelper
{
    public Task<Message?> SendBaseMessage(string? message, bool replyMessage = false);
    public Task<Message?> SendSystemMessage(
        string message, 
        SystemMessagesTypesEnum type = SystemMessagesTypesEnum.Default, 
        bool replyMessage = false
        );

    public Task<File?> DownloadFileAndSave(string filePath, string fileId);
    public Task SendFile(string filePath);
}