using MinifyImagesBot_v2.Enums;
using MinifyImagesBot_v2.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using StreamFile = System.IO.File;
using File = Telegram.Bot.Types.File;

namespace MinifyImagesBot_v2.Classes;

internal class TelegramHelper : ITelegramHelper
{
    private readonly ITelegramBotClient _botClient;
    private readonly Update? _update;
    private readonly CancellationToken _cancellationToken;
    private readonly Exception? _exception;
    private readonly long? _chatId;

    internal TelegramHelper(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _botClient = botClient;
        _update = update;
        _cancellationToken = cancellationToken;
        _chatId = update.Message?.Chat.Id;
    }

    internal TelegramHelper(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _botClient = botClient;
        _exception = exception;
        _cancellationToken = cancellationToken;
    }

    public async Task<Message?> SendBaseMessage(string? message, bool replyMessage = false)
    {
        try
        {
            if (message is null || _chatId is null || _update is null) return null;
            var res = await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: message,
                cancellationToken: _cancellationToken,
                disableNotification: true,
                replyToMessageId: replyMessage is false ? null : _update.Message?.MessageId,
                parseMode: ParseMode.MarkdownV2
            );

            return res;
        }
        catch (Exception e)
        {
            await SendSystemMessage(
                message: e.Message,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );

            return null;
        }
    }

    public async Task<Message?> SendSystemMessage(
        string? message,
        SystemMessagesTypesEnum type = SystemMessagesTypesEnum.Default,
        bool replyMessage = false
    )
    {
        try
        {
            if (message is null || _chatId is null || _update is null) return null;

            var messagePrefix = type switch
            {
                SystemMessagesTypesEnum.Error => "Системная ошибка: ",
                SystemMessagesTypesEnum.Warning => "Системное предупреждение: ",
                _ => "Системное сообщение: "
            };

            var systemMessage = $"<b>{messagePrefix}</b><i>{message}</i>";

            var res = await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: systemMessage,
                cancellationToken: _cancellationToken,
                disableNotification: true,
                replyToMessageId: replyMessage is false ? null : _update.Message?.MessageId,
                parseMode: ParseMode.Html
            );

            return res;
        }
        catch (Exception e)
        {
            await SendSystemMessage(
                message: e.Message,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );

            return null;
        }
    }

    public async Task<File?> DownloadFileAndSave(string filePath, string fileId)
    {
        await using Stream fileStream = new FileStream(filePath, FileMode.Create);
        var res = await _botClient.GetInfoAndDownloadFileAsync(
            fileId: fileId,
            destination: fileStream,
            cancellationToken: _cancellationToken
        );
        
        fileStream.Close();
        return res;
    }

    public async Task<bool> SendFile(string filePath, string? caption = null, bool replyMessage = false)
    {
        try
        {
            await using Stream fileStream = StreamFile.OpenRead(filePath);
            var fileName = filePath.Split("/").Last();
            await _botClient.SendDocumentAsync(
                chatId: _chatId!,
                document: InputFile.FromStream(fileStream, fileName),
                replyToMessageId: !replyMessage ? null : _update?.Message?.MessageId,
                caption: caption,
                cancellationToken: _cancellationToken
            );

            fileStream.Close();
            return true;
        }
        catch (Exception e)
        {
            await SendSystemMessage(
                message: e.Message,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );

            return false;
        }
    }
}