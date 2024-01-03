namespace MinifyImagesBot_v2.Data;

internal static class ResponseSystemTextMessagesData
{
    internal static readonly string WrongCommand =
        "\nНет такой комманды! \n \n<b>Описание бота</b>: /info \n<b>Документация:</b> /guide";

    internal static readonly string ImageNotDocument =
        "\nДля лучшей оптимизации <b>отправьте изображение файлом</b> (без сжатия)";

    internal static readonly string WrongFormat =
        " \nУказан недопустимый формат \n \nДопустимые форматы: \n <b>jpg | jpeg | webp | png</b>";

    internal static readonly string Error =
        "\nЧто то пошло не так! \n \nПовторите попытку";

    internal static readonly string StartFormatting =
        "\nНачинаю форматирование!";
    
    internal static readonly string IsWebp =
        "\nИзображение уже в формате <b>WebP</b> Используйте его :) \n \nЧтобы отформатировать отправьте <b>как фото (со сжатием)</b>";
}