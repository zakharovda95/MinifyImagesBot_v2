using System.Reflection;
using ImageMagick;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace MinifyImagesBot_v2.Classes;

internal static class FileHelper
{
    private static readonly string CurrentDir = AppDomain.CurrentDomain.BaseDirectory;
    internal static string CreateFilePath(Document document)
    {
        var fileName = document.FileName;
        var ext = Path.GetExtension(fileName);
        var fileDir = Path.Combine(CurrentDir, "Assets", "Images");
        if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
        return Path.Combine(fileDir, $"{Guid.NewGuid()}{ext}");
    }
    
    internal static string CreateEditingFilePath(MagickImage image)
    {
        var ext = "." + image.Format;
        var fileDir = Path.Combine(CurrentDir, "Assets", "Images", "Formatted");
        if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
        return Path.Combine(fileDir, $"{Guid.NewGuid()}{ext}");
    }

    internal static string? GetGuideFilePath(string fileName)
    {
        var pathToFile = Path.Combine(CurrentDir, "Assets", fileName);
        return !File.Exists(pathToFile) ? null : pathToFile;
    }
}