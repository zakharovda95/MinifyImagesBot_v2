using MinifyImagesBot_v2.Classes.Helpers;
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
    private static Dictionary<long, ImageEditingParamsModel> _userEditParams = new();

    private async Task OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var text = update.Message?.Text;
        var photo = update.Message?.Photo;
        var document = update.Message?.Document;
        var sticker = update.Message?.Sticker;
        var query = update.CallbackQuery;

        if (text is not null && sticker is null)
            await HandleText(botClient: botClient, update: update, cancellationToken: cancellationToken);
        else if (photo is not null)
            await HandlePhoto(botClient: botClient, update: update, cancellationToken: cancellationToken);
        else if (sticker is not null)
            await HandleSticker(botClient: botClient, update: update, cancellationToken: cancellationToken);
        else if (document is not null)
            await HandleDocument(botClient: botClient, update: update, cancellationToken: cancellationToken);
        else if (query is not null)
            await HandleQuery(botClient: botClient, update: update, cancellationToken: cancellationToken);
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

    private static bool TryGetUserParams(long? chatId, out ImageEditingParamsModel? userParams)
    {
        if (chatId is null)
        {
            userParams = null;
            return false;
        }
        var isSuccess = _userEditParams.TryGetValue((long)chatId!, out var settings);
        if (!isSuccess)
        {
            userParams = null;
            return false;
        }

        userParams = settings;
        return true;
    }

    private static async void ShowReadyInfo(ITelegramBotClient telegramBotClient)
    {
        var info = await telegramBotClient.GetMeAsync();
        Console.WriteLine($"The Bot {info.Username} with ID {info.Id} is ready!");
    }

    private static async Task<Message?> HandleText(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken
    )
    {
        if (TryGetUserParams(update.Message?.Chat.Id, out var userParams))
        {
            File.Delete(userParams.Value.FilePath!);
            _userEditParams.Remove((long)update.Message?.Chat.Id!);
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.WrongAlgoritm,
                type: SystemMessagesTypesEnum.Warning,
                replyMessage: true);
        }
        
        var text = update.Message?.Text;
        switch (text)
        {
            case "/info":
                return await TelegramHelper.SendBaseMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseTextMessagesData.Info,
                    replyMessage: true
                );
            case "/start":
                return await TelegramHelper.SendBaseMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseTextMessagesData.Info,
                    replyMessage: true
                );
            case "/guide":
                var message =
                    await TelegramHelper.SendBaseMessage(
                        botClient: botClient,
                        update: update,
                        cancellationToken: cancellationToken,
                        message: ResponseTextMessagesData.Guide,
                        replyMessage: true
                    );
                var path = FileHelper.GetGuideFilePath(fileName: "guide-image.png");
                if (path is not null)
                {
                    await TelegramHelper.SendPhoto(
                        botClient: botClient,
                        update: update,
                        cancellationToken: cancellationToken,
                        filePath: path);
                }

                return message;
            case "/news":
                return await TelegramHelper.SendBaseMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseTextMessagesData.News,
                    replyMessage: true
                );
            case "/author":
                return await TelegramHelper.SendBaseMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseTextMessagesData.Author,
                    replyMessage: true
                );
            default:
                return await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongCommand,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
        }
    }

    private static async Task<Message?> HandlePhoto(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken
    )
    {
        if (TryGetUserParams(update.Message?.Chat.Id, out var userParams))
        {
            File.Delete(userParams.Value.FilePath!);
            _userEditParams.Remove((long)update.Message?.Chat.Id!);
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.WrongAlgoritm,
                type: SystemMessagesTypesEnum.Warning,
                replyMessage: true);
        }
        
        return await TelegramHelper.SendSystemMessage(
            botClient: botClient,
            update: update,
            cancellationToken: cancellationToken,
            message: ResponseSystemTextMessagesData.ImageNotDocument,
            type: SystemMessagesTypesEnum.Warning,
            replyMessage: true
        );
    }

    private static async Task HandleSticker(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken
    )
    {
        if (TryGetUserParams(update.Message?.Chat.Id, out var userParams))
        {
            File.Delete(userParams.Value.FilePath!);
            _userEditParams.Remove((long)update.Message?.Chat.Id!);
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.WrongAlgoritm,
                type: SystemMessagesTypesEnum.Warning,
                replyMessage: true);
        }
        await TelegramHelper.SendSystemMessage(
            botClient: botClient,
            update: update,
            cancellationToken: cancellationToken,
            message: ResponseSystemTextMessagesData.IsWebp,
            type: SystemMessagesTypesEnum.Default,
            replyMessage: true
        );
    }

    private static async Task HandleDocument(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken
    )
    {
        if (TryGetUserParams(update.Message?.Chat.Id, out var userParams))
        {
            File.Delete(userParams.Value.FilePath!);
            _userEditParams.Remove((long)update.Message?.Chat.Id!);
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.WrongAlgoritm,
                type: SystemMessagesTypesEnum.Warning,
                replyMessage: true);
        }
        var document = update.Message?.Document;
        if (document?.MimeType is null || !document.MimeType.StartsWith("image/")) return;

        var filePath = FileHelper.CreateFilePath(document: document);

        var res = await TelegramHelper.DownloadFileAndSave(
            botClient: botClient,
            update: update,
            cancellationToken: cancellationToken,
            filePath: filePath,
            fileId: document.FileId
        );

        if (res is null)
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.FileSaveError,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );

        var chatId = update.Message?.Chat.Id;
        if (chatId is not null)
        {
            var newUserParams = new ImageEditingParamsModel()
            {
                ChatId = chatId,
                FilePath = filePath,
            };

            _userEditParams.Add((long)chatId, newUserParams);
        }

        var keyboardFormats = TelegramHelper.GetKeyboard(KeyboardTypesEnum.Format);
        if (keyboardFormats is null) return;
        await TelegramHelper.SendKeyboardMessage(
            botClient: botClient,
            update: update,
            cancellationToken: cancellationToken,
            message: "*Конвертация\\:*",
            replyMessage: true,
            markup: keyboardFormats);

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

    private static async Task HandleQuery(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(update);
        var chatId = update.CallbackQuery?.From.Id;
        if (chatId is null)
        {
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.Error,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            return;
        }

        var isSuccess = _userEditParams.TryGetValue((long)chatId, out var settings);
        if (!isSuccess)
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.Error,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );

        if (
            settings.ChatId == chatId &&
            !string.IsNullOrEmpty(settings.FilePath) &&
            settings.ResultFormat is null &&
            settings.CompressLevel is null
        )
        {
            if (Enum.TryParse<FormatKeyboardEnum>(update.CallbackQuery?.Data, out var format))
            {
                settings.ResultFormat = format;
                var keyboardQuality = TelegramHelper.GetKeyboard(KeyboardTypesEnum.Compression);
                if (keyboardQuality is null) return;
                await TelegramHelper.SendKeyboardMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: "*Уровень сжатия\\:*",
                    replyMessage: true,
                    markup: keyboardQuality);
            }
            else
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.Error,
                    type: SystemMessagesTypesEnum.Error,
                    replyMessage: true
                );
        }
        else if (
            settings.ChatId == chatId &&
            !string.IsNullOrEmpty(settings.FilePath) &&
            settings.ResultFormat is not null &&
            settings.CompressLevel is null
        )
        {
            if (Enum.TryParse<CompressKeyboardEnum>(update.CallbackQuery?.Data, out var compress))
            {
                settings.CompressLevel = compress;
            }
            else
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.Error,
                    type: SystemMessagesTypesEnum.Error,
                    replyMessage: true
                );
        }
        else
        {
            //Форматирование
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.Error,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
        }
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