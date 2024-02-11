using MinifyImagesBot_v2.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace MinifyImagesBot_v2.Interfaces;

internal interface ITelegramHelper
{
    Task<Message?> SendBaseMessage(string? message, bool replyMessage = false, InlineKeyboardMarkup? markup = null);
    Task<Message?> SendSystemMessage(
        string message, 
        SystemMessagesTypesEnum type = SystemMessagesTypesEnum.Default, 
        bool replyMessage = false
        );

    Task<File?> DownloadFileAndSave(string filePath, string fileId);
    Task<bool> SendFile(string filePath, string? caption = null, bool replyMessage = false);
    Task<bool> SendPhoto(string filePath, string? caption = null, bool replyMessage = false);

    Task<ReplyKeyboardMarkup> CreateKeyboardMarkup(string[] buttons);
}