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
    private static readonly Dictionary<long, ImageEditingParamsModel> UserEditParams = new();

    private async Task OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var text = update.Message?.Text;
        var photo = update.Message?.Photo;
        var document = update.Message?.Document;
        var sticker = update.Message?.Sticker;
        var query = update.CallbackQuery;

        /*if (update.Message?.MediaGroupId is not null)
        {
            if (!_isMultipleImagesError)
            {
                _isMultipleImagesError = true;
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongMultiple,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
                return;
            }
            _isProcessed = false;
            _isMultipleImagesError = false;
            return;
        }*/

        /*if (!_isProcessed)
        {
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.WrongMultiple,
                type: SystemMessagesTypesEnum.Warning,
                replyMessage: true);
            return;
        }

        _isMultipleImagesError = false;*/

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
        return Task.CompletedTask;
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
        if (chatId is not null && UserEditParams.TryGetValue((long)chatId, out var userParams))
        {
            if (userParams.FilePath is not null)
            {
                File.Delete(userParams.FilePath);
                UserEditParams.Remove((long)chatId);
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
        if (chatId is not null && UserEditParams.TryGetValue((long)chatId, out var userParams))
        {
            if (userParams.FilePath is not null)
            {
                File.Delete(userParams.FilePath);
                UserEditParams.Remove((long)chatId);
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
        if (chatId is not null && UserEditParams.TryGetValue((long)chatId, out var userParams))
        {
            if (userParams.FilePath is not null)
            {
                File.Delete(userParams.FilePath);
                UserEditParams.Remove((long)chatId);
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

        if (chatId is null)
        {
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.UserIdError,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            return;
        }

        var hasProcess = UserEditParams.TryGetValue((long)chatId, out var checkUserParams);

        // Если нет активного процесса - создаем запись в словаре
        if (!hasProcess && checkUserParams is null)
        {
            var newUserParams = new ImageEditingParamsModel { ChatId = chatId };
            UserEditParams.Add((long)chatId, newUserParams);
        }

        // Если есть активный процесс
        if (UserEditParams.TryGetValue((long)chatId, out var userParams))
        {
            // Если есть группа файлов - выдаем ошибку, чистим запись
            if (update.Message?.MediaGroupId is not null)
            {
                if (!userParams.IsMultiple)
                {
                    userParams.IsMultiple = true;
                    await TelegramHelper.SendSystemMessage(
                        botClient: botClient,
                        update: update,
                        cancellationToken: cancellationToken,
                        message: ResponseSystemTextMessagesData.WrongMultiple,
                        type: SystemMessagesTypesEnum.Warning,
                        replyMessage: true);
                    return;
                }
                UserEditParams.Remove((long)chatId);
                return;
            }

            // Если есть файлы в работе - значит процесс активный - выдаем предупреждение
            if (!string.IsNullOrEmpty(userParams.FilePath))
            {
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.InProcessError,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
                return;
            }
        }

        var document = update.Message?.Document;
        if (document?.MimeType is null || !document.MimeType.StartsWith("image/"))
        {
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.WrongFormat,
                type: SystemMessagesTypesEnum.Warning,
                replyMessage: true);
            
            if (!string.IsNullOrEmpty(userParams?.FilePath)) File.Delete(userParams.FilePath);
            UserEditParams.Remove((long)chatId);
            return;
        }

        var filePath = FileHelper.CreateFilePath(document: document);

        var res = await TelegramHelper.DownloadFileAndSave(
            botClient: botClient,
            update: update,
            cancellationToken: cancellationToken,
            filePath: filePath,
            fileId: document.FileId
        );

        if (res is null)
        {
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.FileSaveError,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            if (!string.IsNullOrEmpty(userParams?.FilePath)) File.Delete(userParams.FilePath);
            UserEditParams.Remove((long)chatId);
        }

        if (userParams is not null) userParams.FilePath = filePath;

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
        /*if (!_isProcessed)
        {
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.UserIdError,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            return;
        }*/

        var chatId = update.CallbackQuery?.From.Id;
        if (chatId is null)
        {
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.UserIdError,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            return;
        }

        if (!UserEditParams.TryGetValue((long)chatId, out var settings))
        {
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.FilePathError,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            UserEditParams.Remove((long)chatId);
            Console.WriteLine("нет объекта");
            return;
        }

        if (settings.FilePath is null)
        {
            await TelegramHelper.SendSystemMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message: ResponseSystemTextMessagesData.FilePathError,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            UserEditParams.Remove((long)chatId);
            Console.WriteLine("нет пути");
            return;
        }

        // Не выбрано параметров
        if (
            settings.ChatId == chatId &&
            !string.IsNullOrEmpty(settings.FilePath) &&
            settings.ResultFormat is null &&
            settings.CompressLevel is null
        )
        {
            Console.WriteLine("Блок1");
            if (Enum.TryParse<FormatKeyboardEnum>(update.CallbackQuery?.Data, out var format))
            {
                settings.ResultFormat = format;
                Console.WriteLine(UserEditParams);
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
                Console.WriteLine("Трайпарс1");
            }
            else
            {
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.Error,
                    type: SystemMessagesTypesEnum.Error,
                    replyMessage: true
                );
                File.Delete(settings.FilePath);
                UserEditParams.Remove((long)chatId);
            }
        }
        else if ( // выбран формат не выбран уровень сжатия
                 settings.ChatId == chatId &&
                 !string.IsNullOrEmpty(settings.FilePath) &&
                 settings.ResultFormat is not null &&
                 settings.CompressLevel is null
                )
        {
            Console.WriteLine("Блок2");
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
                Console.WriteLine("Трайпарс2");
            }
            else
            {
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.Error,
                    type: SystemMessagesTypesEnum.Error,
                    replyMessage: true
                );
                File.Delete(settings.FilePath);
                UserEditParams.Remove((long)chatId);
            }
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
            File.Delete(settings.FilePath);
            UserEditParams.Remove((long)chatId);
            Console.WriteLine("другая ошибка");
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