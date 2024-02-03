using ImageMagick;
using MinifyImagesBot_v2.Enums;
using MinifyImagesBot_v2.Interfaces;
using MinifyImagesBot_v2.Models;

namespace MinifyImagesBot_v2.Classes;

internal class ImageEditor : IImageEditor
{
    private readonly string _filePath;
    private readonly MagickImage _magickImage;

    internal ImageEditor(string filePath)
    {
        _filePath = filePath;
        _magickImage = new MagickImage(filePath);
    }

    public void ConvertImage(ImageFormatsEnum? ext)
    {
        _magickImage.Format = ext switch
        {
            ImageFormatsEnum.Jpeg => MagickFormat.Jpeg,
            ImageFormatsEnum.Jpg => MagickFormat.Jpg,
            ImageFormatsEnum.Png => MagickFormat.Png,
            ImageFormatsEnum.Webp => MagickFormat.WebP,
            ImageFormatsEnum.Heic => MagickFormat.Heic,
            _ => _magickImage.Format
        };
    }

    public void MinifyImage()
    {
        /** сжатие без потерь **/
        var info = new FileInfo(_filePath);
        var ext = info.Extension.Replace(".", "");
        var optimizer = new ImageOptimizer();
        
        /** удаление метаданных **/
        _magickImage.Strip();

        if (string.Equals(ext, ImageFormatsEnum.Heic.ToString(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(ext, ImageFormatsEnum.Webp.ToString(), StringComparison.OrdinalIgnoreCase)) return;

        /** Прогрессивная компрессия PNG **/
        if (string.Equals(ext, ImageFormatsEnum.Png.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            optimizer.LosslessCompress(info);
            _magickImage.Settings.Interlace = Interlace.Png;
        }

        /** Прогрессивная компрессия JPEG **/
        if (string.Equals(ext, ImageFormatsEnum.Jpeg.ToString(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(ext, ImageFormatsEnum.Jpg.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            optimizer.LosslessCompress(info);
            _magickImage.Settings.Interlace = Interlace.Jpeg;
            _magickImage.Quality = 90;
        }
    }

    public ImageEditingFileInfoModel GetFileInfo(string? filePath)
    {
        var info = new FileInfo(filePath ?? _filePath);
        info.Refresh();
        return new ImageEditingFileInfoModel()
        {
            FileLength = $"{(info.Length / 1000).ToString()} kB",
            FileExt = info.Extension,
        };
    }

    public string Save()
    {
        var filePath = FileHelper.CreateEditingFilePath(_magickImage);
        _magickImage.Write(filePath);
        return filePath;
    }
}