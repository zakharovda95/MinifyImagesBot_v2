namespace MinifyImagesBot_v2.Data;

internal static class ResponseSystemTextMessagesData
{
    internal static readonly string WrongCommand =
        "\nНет такой комманды \n \nОписание бота /info \nИнструкция /guide";

    internal static readonly string ImageNotDocument =
        "\nДля лучшей оптимизации отправьте изображение файлом (без сжатия)";

    internal static readonly string WrongFormat =
        " \nУказан недопустимый формат \n \nДопустимые форматы: \n jpg jpeg webp png";

    internal static readonly string WrongOptionalParam =
        " \nНет такой опции \n \nНа данный момент из опциональных настроек бот поддерживает только конвертирование в другой формат \nЕсли нужны новые функции напишите @zakharovda95";

    internal static readonly string Error =
        "\nЧто то пошло не так\\! \n \nПовторите попытку";

    internal static readonly string StartFormatting =
        "\nНачинаю форматирование";
}