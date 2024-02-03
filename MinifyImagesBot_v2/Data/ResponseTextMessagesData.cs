namespace MinifyImagesBot_v2.Data;

internal static class ResponseTextMessagesData
{
    internal static readonly string Info = 
        "*\ud83d\udc7e Привет\\! Это небольшой бот\\-редактор изображений\\.* \n \n\ud83d\udca3 Минификация и конвертация в *WebP* \\| *PNG* \\| *JPG* \\| *JPEG* за несколько действий\\. \n \n\ud83e\udd70 Бот развивается, следите за новостями /news \n \n\ud83d\udcd4 Документация /guide";
    internal static readonly string Guide = 
        "\ud83e\udd0c Все очень просто: \n \n🔹 *Скиньте фото документом* *\\(без сжатия\\)* \n\n🔹 *Выберите нужно ли применять сжатие\\** \n\n🔹 *Выберите нужный формат\\* \\(или оставьте исходное расширение\\)* \n\n🔹 \\*Размер файла уменьшается за счет: \n\t \u25aa lossless\\-компрессии  \n\t \u25aa удаления метаданных \n\t \u25aa прогрессивной компрессии \n\t \u25aa конвертирования в *WebP* \\(опционально\\)  \n\n🔹 \\*Поддерживаются следующие форматы файлов: \n *png* \\| *jpg* \\| *jpeg* \\| *webp* \n\n🔹 В будущем будут добавляться новые форматы и новые функции \\- следите за новостями /news";

    internal static readonly string News = "*Image Minify Bot v\\. 2\\.2\\.0* \n\n \ud83d\udcc4 Что нового: \n \u25aa Улучшенный, более интуитивный интерфейс \n \u25aa Новая стратегия сжатия, без потери качетсва \n\n \ud83d\udce3 Анонсы: \n \u25aa Будет добавлен журнал ошибок \n \u25aa Будут детализированы сами ошибки, увеличится количество обработанных исключений \n \u25aa Будут добавлены настройки \n \u25aa Многое другое\\.\\.\\.";
    internal static readonly string Author = "\ud83e\udd77 Автор: \n Все замечания, комплименты, пожелания, проклятия посылать: \n @zakharovda95"; 
}