namespace MinifyImagesBot_v2.Data;

internal static class ResponseSystemTextMessagesData
{
    internal static readonly string WrongCommand =
        "\n\ud83d\udeab Нет такой комманды! \n \n<b>Описание бота</b>: /info \n<b>Документация:</b> /guide";

    internal static readonly string ImageNotDocument =
        "\n\u26a0 Для лучшей оптимизации <b>отправьте изображение файлом</b> (без сжатия)";

    internal static readonly string ErrorFormat =
        " \n\ud83d\udeab Файл поврежден или имеет недопустимый формат \n \nДопустимые форматы: \n <b>jpg | jpeg | webp | png</b>";
    
    internal static readonly string WrongFormat =
        " \n\u26a0 Указан недопустимый формат \n \nДопустимые форматы: \n <b>jpg | jpeg | webp | png</b>";

    internal static readonly string Error =
        "\n\ud83d\udeab Что то пошло не так! \n \nПовторите попытку";

    internal static readonly string StartFormatting =
        "\n\u2705 Начинаю форматирование!";
    
    internal static readonly string IsWebp =
        "\n\u2705 Изображение уже в формате <b>WebP</b>! Используйте его :)";
    
    internal static readonly string Done =
        "\n\u2705 Успешно!";
}