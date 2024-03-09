using MinifyImagesBot_v2.Classes.Extensions;
using MinifyImagesBot_v2.Enums;
using MinifyImagesBot_v2.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using StreamFile = System.IO.File;
using File = Telegram.Bot.Types.File;

namespace MinifyImagesBot_v2.Classes.Helpers;

internal static class TelegramHelper
{
    internal static async Task<Message?> SendBaseMessage(
        ITelegramBotClient botClient, 
        Update update, 
        CancellationToken cancellationToken, 
        string? message, 
        bool replyMessage = false
        )
    {
        try
        {
            var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.From.Id;
            if (message is null || chatId is null) return null;
            var res = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                cancellationToken: cancellationToken,
                disableNotification: true,
                replyToMessageId: replyMessage is false ? null : update.Message?.MessageId,
                parseMode: ParseMode.MarkdownV2
            );
            return res;
        }
        catch (Exception e)
        {
            await SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: e.Message,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );

            return null;
        }
    }
    
    internal static async Task<Message?> SendKeyboardMessage(
        ITelegramBotClient botClient, 
        Update update, 
        CancellationToken cancellationToken, 
        string? message, 
        InlineKeyboardMarkup markup, 
        bool replyMessage = false
        )
    {
        try
        {
            var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.From.Id;
            if (message is null || chatId is null) return null;
            var res = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message,
                cancellationToken: cancellationToken,
                disableNotification: true,
                replyToMessageId: replyMessage is false ? null : update.Message?.MessageId,
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: markup
            );
            
            return res;
        }
        catch (Exception e)
        {
            await SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: e.Message,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );

            return null;
        }
    }

    internal static async Task<Message?> SendSystemMessage(
        ITelegramBotClient botClient, 
        Update update, 
        CancellationToken cancellationToken, 
        string? message,
        SystemMessagesTypesEnum type = SystemMessagesTypesEnum.Default,
        bool replyMessage = false
    )
    {
        try
        {
            var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.From.Id;
            if (message is null || chatId is null) return null;

            var messagePrefix = type switch
            {
                SystemMessagesTypesEnum.Error => "Системная ошибка: ",
                SystemMessagesTypesEnum.Warning => "Системное предупреждение: ",
                SystemMessagesTypesEnum.EditingResult => "Результат форматирования: ",
                _ => "Системное сообщение: "
            };

            var systemMessage = $"<b>{messagePrefix}</b><i>{message}</i>";

            var res = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: systemMessage,
                cancellationToken: cancellationToken,
                disableNotification: true,
                replyToMessageId: replyMessage is false ? null : update.Message?.MessageId,
                parseMode: ParseMode.Html
            );

            return res;
        }
        catch (Exception e)
        {
            await SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: e.Message,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );

            return null;
        }
    }

    internal static async Task<File?> DownloadFileAndSave(
        ITelegramBotClient botClient, 
        Update update, 
        CancellationToken cancellationToken, 
        string filePath, 
        string fileId
        )
    {
        await using Stream fileStream = new FileStream(filePath, FileMode.Create);
        var res = await botClient.GetInfoAndDownloadFileAsync(
            fileId: fileId,
            destination: fileStream,
            cancellationToken: cancellationToken
        );
        
        fileStream.Close();
        return res;
    }

    internal static async Task<bool?> SendFile(
        ITelegramBotClient botClient, 
        Update update, 
        CancellationToken cancellationToken, 
        string filePath, 
        string? caption = null, 
        bool replyMessage = false
        )
    {
        try
        {
            var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.From.Id;
            if (chatId is null) return null;
            await using Stream fileStream = StreamFile.OpenRead(filePath);
            var fileName = filePath.Split("/").Last();
            await botClient.SendDocumentAsync(
                chatId: chatId!,
                document: InputFile.FromStream(fileStream, fileName),
                replyToMessageId: !replyMessage ? null : update?.Message?.MessageId,
                caption: caption,
                cancellationToken: cancellationToken
            );

            fileStream.Close();
            return true;
        }
        catch (Exception e)
        {
            await SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: e.Message,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );

            return false;
        }
    }
    
    internal static async Task<bool?> SendPhoto(
        ITelegramBotClient botClient, 
        Update update, 
        CancellationToken cancellationToken, 
        string filePath, 
        string? caption = null, 
        bool replyMessage = false
        )
    {
        try
        {
            var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.From.Id;
            if (chatId is null) return null;
            await using Stream fileStream = StreamFile.OpenRead(filePath);
            var fileName = filePath.Split("/").Last();
            await botClient.SendPhotoAsync(
                chatId: chatId!,
                photo: InputFile.FromStream(fileStream, fileName),
                replyToMessageId: !replyMessage ? null : update?.Message?.MessageId,
                caption: caption,
                cancellationToken: cancellationToken
            );

            fileStream.Close();
            return true;
        }
        catch (Exception e)
        {
            await SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: e.Message,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );

            return false;
        }
    }

    internal static InlineKeyboardMarkup? GetKeyboard(KeyboardTypesEnum type)
    {
        switch (type)
        {
            case KeyboardTypesEnum.Format:
                var keysFormats = new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(FormatKeyboardEnum.Webp.GetDisplayName(), FormatKeyboardEnum.Webp.ToString()),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(FormatKeyboardEnum.None.GetDisplayName(), FormatKeyboardEnum.None.ToString()),
                    }
                };
                return new InlineKeyboardMarkup(keysFormats);
            case KeyboardTypesEnum.Compression:
                var keysQuality = new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(CompressKeyboardEnum.WithCompress.GetDisplayName(), CompressKeyboardEnum.WithCompress.ToString()),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(CompressKeyboardEnum.None.GetDisplayName(), CompressKeyboardEnum.None.ToString()),
                    }
                };
                return new InlineKeyboardMarkup(keysQuality);
            default: return null;
        }
    }
}