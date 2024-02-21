namespace MinifyImagesBot_v2.Data;

internal static class ResponseSystemTextMessagesData
{
    internal static readonly string WrongCommand =
        "\n\ud83d\udeab Нет такой комманды! \n \n<b>Описание бота</b>: /info \n<b>Документация:</b> /guide \n<b>Новости:</b> /news \n<b>Об авторе:</b> /author";

    internal static readonly string ImageNotDocument =
        "\n\u2757Неподдерживаемый тип отправки! \n\n\u25aaДля лучшей оптимизации <b>отправьте изображение файлом</b> (без сжатия)";

    internal static readonly string ErrorFormat =
        " \n\ud83d\udeab Файл поврежден или имеет недопустимый формат \n \nДопустимые форматы: \n <b>jpg/jpeg | png | heic</b>\n\n\u25aa<i>Повторите попытку!</i>";
    
    internal static readonly string WrongFormat =
        " \n\u2757 Указан недопустимый формат \n \nДопустимые форматы: \n <b>jpg/jpeg | png  | heic</b> \n\n\u25aa<i>Повторите попытку!</i>";

    internal static readonly string Error =
        "\n\ud83d\udeab Что то пошло не так! \n \n\u25aa</i>Повторите попытку!</i>";
    
    internal static readonly string FilePathError =
        "\n\ud83d\udeab Изображение для форматирования не найдено! \n \n\u25aa<i>Повторите попытку!</i>";
    
    internal static readonly string FileSaveError =
        "\n\ud83d\udeab Ошибка при сохранении файла! \n \n\u25aa<i>Повторите попытку!</i>";
    
    internal static readonly string UserIdError =
        "\n\ud83d\udeab Не определен id чата! \n \n\u25aa<i>Повторите попытку!</i>";
    
    internal static readonly string ProcessError =
        "\n\ud83d\udeab Процесс не активен! \n \n\u25aa<i>Повторите попытку!</i>";
    
    internal static readonly string EditingError =
        "\n\ud83d\udeab Ошибка в процессе редактирования! \n \n\u25aa<i>Повторите попытку!</i>";
    
    internal static readonly string InProcessError =
        "\n\ud83d\udeab Внимание, уже есть активный процесс! \n \n\u25aaЗавершите редактирование предыдущего изображения, либо отмените процесс \n \n\u2757<i>Чтобы отменить процесс отправьте любое сообщение</i>";
    
    internal static readonly string WrongAlgoritm =
        "\n\ud83d\udeab <b>Форматирование отменено!</b> \n\n\u2757<i>Чтобы повторить, загрузите изображение и следуйте подсказкам</i>";
    
    internal static readonly string WrongMultiple =
        "\n\ud83d\udeab <b>Форматирование отменено!</b> \n\n🔹Функция множественного редактирования  изображений в данный момент отключена. \n\n\u2757<i>Чтобы повторить, загрузите документом не более одного изображения за раз</i>";

    internal static readonly string StartFormatting =
        "\n\u2705 Начинаю форматирование!";
    
    internal static readonly string IsWebp =
        "\n\u2705 Изображение уже в формате <b>WebP</b>! Используйте его :)";
    
    internal static readonly string Done =
        "\n\u2705 Успешно!";
}