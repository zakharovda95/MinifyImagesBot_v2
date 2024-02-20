using MinifyImagesBot_v2.Classes.Extensions;
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

    private static async Task HandleText(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken
    )
    {
        var chatId = update.Message?.Chat.Id;
        if (chatId is not null && _userEditParams.TryGetValue((long)chatId, out var userParams))
        {
            if (userParams.FilePath is not null)
            {
                Console.WriteLine(_userEditParams);
                File.Delete(userParams.FilePath);
                _userEditParams.Remove((long)chatId);
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongAlgoritm,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
                Console.WriteLine(_userEditParams);
                return;
            }
        }

        var text = update.Message?.Text;
        switch (text)
        {
            case "/info":
                await TelegramHelper.SendBaseMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseTextMessagesData.Info,
                    replyMessage: true
                );
                break;
            case "/start":
                await TelegramHelper.SendBaseMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseTextMessagesData.Info,
                    replyMessage: true
                );
                break;
            case "/guide":
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

                break;
            case "/news":
                await TelegramHelper.SendBaseMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseTextMessagesData.News,
                    replyMessage: true
                );
                break;
            case "/author":
                await TelegramHelper.SendBaseMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseTextMessagesData.Author,
                    replyMessage: true
                );
                break;
            default:
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongCommand,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
                break;
        }
    }

    private static async Task HandlePhoto(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken
    )
    {
        var chatId = update.Message?.Chat.Id;
        if (chatId is not null && _userEditParams.TryGetValue((long)chatId, out var userParams))
        {
            if (userParams.FilePath is not null)
            {
                File.Delete(userParams.FilePath);
                _userEditParams.Remove((long)chatId);
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongAlgoritm,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
                return;
            }
        }

        await TelegramHelper.SendSystemMessage(
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
        var chatId = update.Message?.Chat.Id;
        if (chatId is not null && _userEditParams.TryGetValue((long)chatId, out var userParams))
        {
            if (userParams.FilePath is not null)
            {
                File.Delete(userParams.FilePath);
                _userEditParams.Remove((long)chatId);
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongAlgoritm,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
                return;
            }
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
        var chatId = update.Message?.Chat.Id;
        if (chatId is not null && _userEditParams.TryGetValue((long)chatId, out var userParams))
        {
            if (userParams.FilePath is not null)
            {
                File.Delete(userParams.FilePath);
                _userEditParams.Remove((long)chatId);
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongAlgoritm,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
                return;
            }
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
            message:
            "*Форматирование\\:* \nВыберите один из предложенных вариантов \n\n \u2757_Для отмены отправьте любое сообщение в чат_",
            replyMessage: true,
            markup: keyboardFormats);
    }

    private static async Task HandleQuery(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken
    )
    {
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

        if (!_userEditParams.TryGetValue((long)chatId, out var settings))
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.Error,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
        Console.WriteLine(settings);

        // Не выбрано параметров
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
                var res = await TelegramHelper.SendKeyboardMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message:
                    $"Форматирование\\: *{format.GetDisplayName().ToLower()}* \n\n*Уровень сжатия\\:*  \nВыберите один из предложенных вариантов \n\n\u2757 _Для отмены отправьте любое сообщение в чат_",
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
        else if ( // выбран формат не выбран уровень сжатия
                 settings.ChatId == chatId &&
                 !string.IsNullOrEmpty(settings.FilePath) &&
                 settings.ResultFormat is not null &&
                 settings.CompressLevel is null
                )
        {
            if (Enum.TryParse<CompressKeyboardEnum>(update.CallbackQuery?.Data, out var compress))
            {
                settings.CompressLevel = compress;
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.StartFormatting,
                    type: SystemMessagesTypesEnum.Default,
                    replyMessage: false
                );
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
            //Ошибка
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