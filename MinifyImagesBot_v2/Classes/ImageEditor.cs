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
            _ => _magickImage.Format
        };
    }

    public void MinifyImage()
    {
        var info = new FileInfo(_filePath);
        _magickImage.Quality = 65;
        var optimizer = new ImageOptimizer();
        optimizer.LosslessCompress(info);
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