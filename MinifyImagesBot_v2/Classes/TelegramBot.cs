using MinifyImagesBot_v2.Data;
using MinifyImagesBot_v2.Enums;
using MinifyImagesBot_v2.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MinifyImagesBot_v2.Classes;

internal sealed class TelegramBot
{
    private async Task OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var text = update.Message?.Text;
        var photo = update.Message?.Photo;
        var document = update.Message?.Document;
        var telegramHelper =
            new TelegramHelper(botClient: botClient, update: update, cancellationToken: cancellationToken);

        if (text is not null) await HandleText(text, telegramHelper: telegramHelper);
        if (photo is not null) await HandlePhoto(telegramHelper: telegramHelper);
        if (document is not null) await HandleDocument(document: document, telegramHelper: telegramHelper);
    }

    private async Task OnError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static async void ShowReadyInfo(ITelegramBotClient telegramBotClient)
    {
        var info = await telegramBotClient.GetMeAsync();
        Console.WriteLine($"The Bot {info.Username} with ID {info.Id} is ready!");
    }

    private static async Task<Message?> HandleText(string text, ITelegramHelper telegramHelper)
    {
        return text switch
        {
            "/info" => await telegramHelper.SendBaseMessage(message: ResponseTextMessagesData.Info, replyMessage: true),
            "/guide" => await telegramHelper.SendBaseMessage(message: ResponseTextMessagesData.Guide,
                replyMessage: true),
            _ => await telegramHelper.SendSystemMessage(message: ResponseSystemTextMessagesData.WrongCommand,
                type: SystemMessagesTypesEnum.Warning, replyMessage: true)
        };
    }

    private static async Task<Message?> HandlePhoto(ITelegramHelper telegramHelper)
    {
        return await telegramHelper.SendSystemMessage(
            message: ResponseSystemTextMessagesData.ImageNotDocument,
            type: SystemMessagesTypesEnum.Warning,
            replyMessage: true
        );
    }

    private static async Task HandleDocument(Document document, ITelegramHelper telegramHelper)
    {
        if (document.MimeType is null || !document.MimeType.StartsWith("image/")) return;
        
        var filePath = FileHelper.CreateFilePath(document: document);
        if (filePath is null) return;
        
        var res = await telegramHelper.DownloadFileAndSave(filePath: filePath, fileId: document.FileId);

        if (res is null)
        {
            await telegramHelper.SendSystemMessage(
                message: ResponseSystemTextMessagesData.Error,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            return;
        }
        
        Console.WriteLine(res);
    }

    public void CreateTelegramClientAndRun(string? telegramKey)
    {
        if (telegramKey is null) return;

        var telegramBotClient = new TelegramBotClient(telegramKey);
        using CancellationTokenSource ctx = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions() { AllowedUpdates = Array.Empty<UpdateType>() };

        telegramBotClient.StartReceiving(
            updateHandler: OnUpdate,
            pollingErrorHandler: OnError,
            receiverOptions: receiverOptions,
            cancellationToken: ctx.Token
        );

        ShowReadyInfo(telegramBotClient: telegramBotClient);
    }
}