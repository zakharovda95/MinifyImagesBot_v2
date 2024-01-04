using System.Text.RegularExpressions;
using MinifyImagesBot_v2.Data;
using MinifyImagesBot_v2.Enums;
using MinifyImagesBot_v2.Interfaces;
using MinifyImagesBot_v2.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace MinifyImagesBot_v2.Classes;

internal sealed class TelegramBot : ITelegramBot
{
    private async Task OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var telegramHelper =
            new TelegramHelper(botClient: botClient, update: update, cancellationToken: cancellationToken);
        try
        {
            var text = update.Message?.Text;
            var photo = update.Message?.Photo;
            var document = update.Message?.Document;
            var sticker = update.Message?.Sticker;
            var caption = update.Message?.Caption;

            if (text is not null && sticker is null) await HandleText(text, telegramHelper: telegramHelper);
            else if (photo is not null) await HandlePhoto(telegramHelper: telegramHelper);
            else if (document is not null)
                await HandleDocument(document: document, caption: caption, telegramHelper: telegramHelper);
            else if (sticker is not null)
            {
                await telegramHelper.SendSystemMessage(
                    message: ResponseSystemTextMessagesData.IsWebp,
                    type: SystemMessagesTypesEnum.Default,
                    replyMessage: true
                );
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task OnError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var telegramHelper =
            new TelegramHelper(botClient: botClient, exception: exception, cancellationToken: cancellationToken);
        try
        {
            await telegramHelper.SendSystemMessage(
                message: ResponseSystemTextMessagesData.Error,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
        }
        catch (Exception e)
        {
            await telegramHelper.SendSystemMessage(
                message: ResponseSystemTextMessagesData.Error,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            Console.WriteLine(e);
            throw;
        }
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
            "/start" => await telegramHelper.SendBaseMessage(message: ResponseTextMessagesData.Info, replyMessage: true),
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

    private static async Task HandleDocument(Document document, string? caption, ITelegramHelper telegramHelper)
    {
        if (document.MimeType is null || !document.MimeType.StartsWith("image/")) return;

        var filePath = FileHelper.CreateFilePath(document: document);
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

        await telegramHelper.SendSystemMessage(
            message: ResponseSystemTextMessagesData.StartFormatting,
            type: SystemMessagesTypesEnum.Default,
            replyMessage: true
        );

        var result = EditImage(filePath: filePath, caption: caption, telegramHelper: telegramHelper);

        if (!result.IsSuccess)
        {
            await telegramHelper.SendSystemMessage(
                message: ResponseSystemTextMessagesData.WrongFormat,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            File.Delete(filePath);
            return;
        }
        
        var sendingCaption =
            $"Размер: {result.FileInfoBefore?.FileLength} --> {result.FileInfoAfter?.FileLength}; \n Формат: {result.FileInfoBefore?.FileExt} --> {result.FileInfoAfter?.FileExt}";
        if (result.FilePath is not null)
        {
            await telegramHelper.SendFile(filePath: result.FilePath, caption: sendingCaption, replyMessage: true);
            File.Delete(result.FilePath);
            File.Delete(filePath);
            return;
        }
        
        File.Delete(filePath);
    }

    private static ImageEditingResultModel EditImage(string filePath, ITelegramHelper telegramHelper,
        string? caption = null)
    {
        var imageEditor = new ImageEditor(filePath: filePath);
        ImageFormatsEnum? format;
        if (caption is null) format = ImageFormatsEnum.Webp;
        else
        {
            if (Enum.TryParse(caption, ignoreCase: true, out ImageFormatsEnum formatCaption))
            {
                format = formatCaption;
            }
            else
            {
                format = null;
            }
        }

        if (format is null)
        {
            return new ImageEditingResultModel()
            {
                IsSuccess = false,
                FilePath = null,
                FileInfoBefore = null,
                FileInfoAfter = null,
            };
        }
        else
        {
            var infoBefore = imageEditor.GetFileInfo(filePath: filePath);
            imageEditor.MinifyImage();
            imageEditor.ConvertImage(format);
            var path = imageEditor.Save();
            var infoAfter = imageEditor.GetFileInfo(filePath: path);

            return new ImageEditingResultModel()
            {
                IsSuccess = true,
                FilePath = path,
                FileInfoBefore = infoBefore,
                FileInfoAfter = infoAfter,
            };
        }
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