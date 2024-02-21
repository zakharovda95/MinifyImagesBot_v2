using MinifyImagesBot_v2.Classes.Extensions;
using MinifyImagesBot_v2.Classes.Helpers;
using MinifyImagesBot_v2.Data;
using MinifyImagesBot_v2.Enums;
using MinifyImagesBot_v2.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace MinifyImagesBot_v2.Classes;

internal static class TelegramBot
{
    private static readonly Dictionary<long, ImageEditingParamsModel> UserEditParams = new();

    private static async Task OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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

    private static Task OnError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        return Task.CompletedTask;
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
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongAlgoritm,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
                File.Delete(userParams.FilePath);
                UserEditParams.Remove((long)chatId);
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
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongAlgoritm,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
                File.Delete(userParams.FilePath);
                UserEditParams.Remove((long)chatId);
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
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongAlgoritm,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);
                File.Delete(userParams.FilePath);
                UserEditParams.Remove((long)chatId);
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

        // Если нет активного процесса - создаем запись в словаре
        if (!UserEditParams.ContainsKey((long)chatId))
        {
            var newUserParams = new ImageEditingParamsModel { ChatId = chatId };
            UserEditParams.Add((long)chatId, newUserParams);
        }

        // Если есть активный процесс
        var hasProcess = UserEditParams.TryGetValue((long)chatId, out var userParams);
        if (hasProcess && userParams is not null)
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


            var document = update.Message?.Document;
            // Если недопустимый формат - выдаем предупреждение
            if (document?.MimeType is null || !document.MimeType.StartsWith("image/"))
            {
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.WrongFormat,
                    type: SystemMessagesTypesEnum.Warning,
                    replyMessage: true);

                if (!string.IsNullOrEmpty(userParams.FilePath)) File.Delete(userParams.FilePath);
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
                if (!string.IsNullOrEmpty(userParams.FilePath)) File.Delete(userParams.FilePath);
                UserEditParams.Remove((long)chatId);
            }

            userParams.FilePath = filePath;

            // Выдаем клавиатуру для выбора формата
            var keyboardFormats = TelegramHelper.GetKeyboard(KeyboardTypesEnum.Format);
            if (keyboardFormats is null)
            {
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.Error,
                    type: SystemMessagesTypesEnum.Error,
                    replyMessage: true
                );
                File.Delete(userParams.FilePath);
                UserEditParams.Remove((long)chatId);
                return;
            }

            await TelegramHelper.SendKeyboardMessage(
                botClient: botClient,
                update: update,
                cancellationToken: cancellationToken,
                message:
                "*Форматирование\\:* \nВыберите один из предложенных вариантов \n\n \u2757_Для отмены отправьте любое сообщение в чат_",
                replyMessage: true,
                markup: keyboardFormats);
        }
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
                message: ResponseSystemTextMessagesData.UserIdError,
                type: SystemMessagesTypesEnum.Error,
                replyMessage: true
            );
            return;
        }

        if (!UserEditParams.TryGetValue((long)chatId, out var userParams))
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
            return;
        }

        if (userParams.FilePath is null)
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
            return;
        }

        // Не выбрано параметров
        if (
            userParams.ChatId == chatId &&
            !string.IsNullOrEmpty(userParams.FilePath) &&
            userParams.ResultFormat is null &&
            userParams.CompressLevel is null
        )
        {
            if (Enum.TryParse<FormatKeyboardEnum>(update.CallbackQuery?.Data, out var format))
            {
                userParams.ResultFormat = format;
                
                //отправляем клавиатуру степени сжатия
                var keyboardQuality = TelegramHelper.GetKeyboard(KeyboardTypesEnum.Compression);
                if (keyboardQuality is null)
                {
                    await TelegramHelper.SendSystemMessage(
                        botClient: botClient,
                        update: update,
                        cancellationToken: cancellationToken,
                        message: ResponseSystemTextMessagesData.Error,
                        type: SystemMessagesTypesEnum.Error,
                        replyMessage: true
                    );
                    File.Delete(userParams.FilePath);
                    UserEditParams.Remove((long)chatId);
                    return;
                }
                await TelegramHelper.SendKeyboardMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message:
                    $"Форматирование\\: *{format.GetDisplayName().ToLower()}* \n\n*Уровень сжатия\\:*  \nВыберите один из предложенных вариантов \n\n\u2757 _Для отмены отправьте любое сообщение в чат_",
                    replyMessage: true,
                    markup: keyboardQuality);
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
                File.Delete(userParams.FilePath);
                UserEditParams.Remove((long)chatId);
            }
        }
        else if ( // выбран формат не выбран уровень сжатия
                 userParams.ChatId == chatId &&
                 !string.IsNullOrEmpty(userParams.FilePath) &&
                 userParams.ResultFormat is not null &&
                 userParams.CompressLevel is null
                )
        {
            if (Enum.TryParse<CompressKeyboardEnum>(update.CallbackQuery?.Data, ignoreCase: true, out var compress))
            {
                userParams.CompressLevel = compress;
                
                // начинаем обработку файла
                await TelegramHelper.SendSystemMessage(
                    botClient: botClient,
                    update: update,
                    cancellationToken: cancellationToken,
                    message: ResponseSystemTextMessagesData.StartFormatting,
                    type: SystemMessagesTypesEnum.Default,
                    replyMessage: false
                );

                var editor = new ImageEditor(userParams);
                var editingResult = editor.Edit();
                if (editingResult.IsSuccess)
                {
                    await TelegramHelper.SendSystemMessage(
                        botClient: botClient,
                        update: update,
                        cancellationToken: cancellationToken,
                        message: ResponseSystemTextMessagesData.Done,
                        type: SystemMessagesTypesEnum.Error,
                        replyMessage: true
                    );
                    File.Delete(userParams.FilePath);
                    if (editingResult.FilePath is not null) 
                        File.Delete(editingResult.FilePath);
                    UserEditParams.Remove((long)chatId);
                }
                else
                {
                    await TelegramHelper.SendSystemMessage(
                        botClient: botClient,
                        update: update,
                        cancellationToken: cancellationToken,
                        message: ResponseSystemTextMessagesData.EditingError,
                        type: SystemMessagesTypesEnum.Error,
                        replyMessage: true
                    );
                    File.Delete(userParams.FilePath);
                    UserEditParams.Remove((long)chatId);
                }
                
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
                File.Delete(userParams.FilePath);
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
            File.Delete(userParams.FilePath);
            UserEditParams.Remove((long)chatId);
        }
    }
    
    private static async Task ShowReadyInfo(ITelegramBotClient telegramBotClient)
    {
        var info = await telegramBotClient.GetMeAsync();
        Console.WriteLine($"The Bot {info.Username} with ID {info.Id} is ready!");
    }

    public static async Task CreateTelegramClientAndRun(string? telegramKey)
    {
        if (telegramKey is null) return;

        var telegramBotClient = new TelegramBotClient(telegramKey);
        using var ctx = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions() { AllowedUpdates = Array.Empty<UpdateType>() };
        telegramBotClient.StartReceiving(
            updateHandler: OnUpdate,
            pollingErrorHandler: OnError,
            receiverOptions: receiverOptions,
            cancellationToken: ctx.Token
        );

        await ShowReadyInfo(telegramBotClient: telegramBotClient);
    }
}