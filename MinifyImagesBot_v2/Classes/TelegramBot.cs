using MinifyImagesBot_v2.Data;
using MinifyImagesBot_v2.Enums;
using MinifyImagesBot_v2.Interfaces;
using MinifyImagesBot_v2.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace MinifyImagesBot_v2.Classes;

internal sealed class TelegramBot : ITelegramBot
{
    private async Task OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var telegramHelper =
            new TelegramHelper(botClient: botClient, update: update, cancellationToken: cancellationToken);
        var text = update.Message?.Text;
        var photo = update.Message?.Photo;
        var document = update.Message?.Document;
        var sticker = update.Message?.Sticker;
        var caption = update.Message?.Caption;

        if (text is not null && sticker is null) await HandleText(text: text, telegramHelper: telegramHelper);
        else if (photo is not null) await HandlePhoto(telegramHelper: telegramHelper);
        else if (document is not null)
            await HandleDocument(document: document, caption: caption, telegramHelper: telegramHelper);
        else if (sticker is not null) await HandleSticker(sticker: sticker, telegramHelper: telegramHelper);
    }

    private Task OnError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    private static async void ShowReadyInfo(ITelegramBotClient telegramBotClient)
    {
        var info = await telegramBotClient.GetMeAsync();
        Console.WriteLine($"The Bot {info.Username} with ID {info.Id} is ready!");
    }

    private static async Task<Message?> HandleText(string text, ITelegramHelper telegramHelper)
    {
        switch (text)
        {
            case "/info":
               return await telegramHelper.SendBaseMessage(message: ResponseTextMessagesData.Info, replyMessage: true);
            case "/start":
                return await telegramHelper.SendBaseMessage(message: ResponseTextMessagesData.Info, replyMessage: true);
            case "/guide":
                var message = await telegramHelper.SendBaseMessage(message: ResponseTextMessagesData.Guide, replyMessage: true);
                var path = FileHelper.GetGuideFilePath(fileName: "guide-image.png");
                if (path is not null) await telegramHelper.SendPhoto(filePath: path);
                return message;
            case "/news":
                return await telegramHelper.SendBaseMessage(message: ResponseTextMessagesData.News, replyMessage: true);
            case "/author":
                return await telegramHelper.SendBaseMessage(message: ResponseTextMessagesData.Author, replyMessage: true);
            default:
                return await telegramHelper.SendSystemMessage(message: ResponseSystemTextMessagesData.WrongCommand,
                    type: SystemMessagesTypesEnum.Warning, replyMessage: true);
        }
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

        var keysFormats = new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(ImageFormatsEnum.Webp.ToString(),
                    ImageFormatsEnum.Webp.ToString()),
                InlineKeyboardButton.WithCallbackData(ImageFormatsEnum.Heic.ToString(),
                    ImageFormatsEnum.Heic.ToString()),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Без конвертации", "none"),
            }
        };

        var keyboardFormats = new InlineKeyboardMarkup(keysFormats);

        var messageFormats = await telegramHelper.SendBaseMessage(message: "Выберите формат для конвертации", replyMessage: true, markup: keyboardFormats);
        
        var keysQuality = new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Максимальное сжатие", "maxCompress"),
                InlineKeyboardButton.WithCallbackData("Щадящее сжатие", "minCompress"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Без сжатия", "noneCompress"),
            }
        };

        var keyboardQuality = new InlineKeyboardMarkup(keysQuality);

        var messageQuality = await telegramHelper.SendBaseMessage(
            message: "*Выберите Уровень сжатия* \n\n * \\- Максимальный* \\- конвертация \\+ максимальное сжатие \\(может быть незначительное снижение качества изображений при большом увеличении\\) \n * \\- Минимальный* \\- при изменении формата применяется щадящая компрессия, без изменения формата \\- применяется максимальная компрессия \n * \\- Без сжатия* \\- компрессия не применяется", 
            replyMessage: true, 
            markup: keyboardQuality);

        /*if (res is null)
        {
            await telegramHelper.SendSystemMessage(
                message: ResponseSystemTextMessagesData.Error,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            return;
        }*/

        /*await telegramHelper.SendSystemMessage(
            message: ResponseSystemTextMessagesData.StartFormatting,
            type: SystemMessagesTypesEnum.Default,
            replyMessage: true
        );*/

        //var result = EditImage(filePath: filePath, caption: caption);

        /*if (!result.IsSuccess)
        {
            await telegramHelper.SendSystemMessage(
                message: ResponseSystemTextMessagesData.ErrorFormat,
                type: SystemMessagesTypesEnum.EditingResult,
                replyMessage: true
            );
            File.Delete(filePath);
            return;
        }*/

        /*if (result.FilePath is not null)
        {
            var sendingCaption =
                $"Размер: {result.FileInfoBefore?.FileLength} --> {result.FileInfoAfter?.FileLength}; \n Формат: {result.FileInfoBefore?.FileExt} --> {result.FileInfoAfter?.FileExt}";

            await telegramHelper.SendSystemMessage(
                message: ResponseSystemTextMessagesData.Done,
                type: SystemMessagesTypesEnum.EditingResult,
                replyMessage: true
            );
            await telegramHelper.SendFile(filePath: result.FilePath, caption: sendingCaption, replyMessage: true);
            File.Delete(result.FilePath);
            File.Delete(filePath);
            return;
        }*/

        //File.Delete(filePath);
    }

    private static async Task HandleSticker(Sticker sticker, ITelegramHelper telegramHelper)
    {
        await telegramHelper.SendSystemMessage(
            message: ResponseSystemTextMessagesData.IsWebp,
            type: SystemMessagesTypesEnum.Default,
            replyMessage: true
        );
    }

    private static ImageEditingResultModel EditImage(string filePath, string? caption = null)
    {
        try
        {
            var imageEditor = new ImageEditor(filePath: filePath);
            /*ImageFormatsEnum? format;
            if (caption is null) format = ImageFormatsEnum.Webp;
            else
            {
                if (Enum.TryParse(caption, ignoreCase: true, out ImageFormatsEnum formatCaption)) format = formatCaption;
                else format = null;
            }*/

            /*if (format is null)
            {
                return new ImageEditingResultModel()
                {
                    IsSuccess = false,
                    Message = ResponseSystemTextMessagesData.WrongFormat,
                    FilePath = null,
                    FileInfoBefore = null,
                    FileInfoAfter = null,
                };
            }*/
            var infoBefore = imageEditor.GetFileInfo(filePath: filePath);
            imageEditor.MinifyImage();
            //imageEditor.ConvertImage(format);
            var path = imageEditor.Save();
            var infoAfter = imageEditor.GetFileInfo(filePath: path);

            return new ImageEditingResultModel()
            {
                IsSuccess = true,
                Message = null,
                FilePath = path,
                FileInfoBefore = infoBefore,
                FileInfoAfter = infoAfter,
            };
        }
        catch (Exception e)
        {
            return new ImageEditingResultModel()
            {
                IsSuccess = false,
                Message = ResponseSystemTextMessagesData.ErrorFormat,
                FilePath = null,
                FileInfoBefore = null,
                FileInfoAfter = null,
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