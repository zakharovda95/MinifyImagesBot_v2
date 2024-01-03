using Telegram.Bot.Types;
namespace MinifyImagesBot_v2.Classes;

internal static class FileHelper
{
    internal static string? CreateFilePath(Document document)
    {
        var fileName = document.FileName;
        var ext = Path.GetExtension(fileName);
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        var fileDir = Path.Combine(currentDir, "Assets", "Images");
        if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
        return Path.Combine(fileDir, $"{fileName}{ext}") ?? null;
    }
}