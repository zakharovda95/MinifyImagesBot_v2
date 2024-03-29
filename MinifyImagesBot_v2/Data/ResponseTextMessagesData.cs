namespace MinifyImagesBot_v2.Data;

internal static class ResponseTextMessagesData
{
    internal static readonly string Info = 
        "*\ud83d\udc7e Привет\\! Это небольшой бот\\-редактор изображений\\.* \n \n\ud83d\udca3 Минификация и конвертация в *WebP* \\| *PNG* \\| *JPG* \\| *JPEG* за одно действие\\. \n \n\ud83e\udd70 Что нового? \n\u25aa Фото\\-гайд /guide \n\u25aa \\-5\\% к сжатию\n\u25aa Информативные системные сообщения \n \n\ud83d\udcd4 Документация /guide";
    internal static readonly string Guide = 
        "\ud83e\udd0c Все очень просто: \n \n🔹 *Скиньте фото документом* \\(без сжатия\\) \n\n🔹 По умолчанию идет оптимальное сжатие и конвертация *в формат WebP* \n\n🔹 Если вам нужно сжатие без изменения формата \\- укажите в подписи к файлу *исходный формат файла* \n\n🔹 При необходимости можно переконвертировать в нужный формат, для этого в подписи к файлу укажите *нужный формат \\(расширение\\) файла* \n\n🔹 Поддерживаются следующие форматы файлов: \n *png* \\| *jpg* \\| *jpeg* \\| *webp*";
}