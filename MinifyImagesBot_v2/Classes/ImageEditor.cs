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
        
        int CalculateQuality(byte currQuality, byte qualityDemotion, byte minQuality)
        {
            if (currQuality > 95) qualityDemotion += 15;
            else if (currQuality > 90) qualityDemotion += 10;
            else if (currQuality > 80) qualityDemotion += 5;
            
            return (currQuality - qualityDemotion) < minQuality ? minQuality : currQuality - qualityDemotion;
        }
        
        /** удаление метаданных **/
        _magickImage.Strip();
        _magickImage.Write(_filePath);
        info.Refresh();

        if (string.Equals(ext, ImageFormatsEnum.Heic.ToString(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(ext, ImageFormatsEnum.Webp.ToString(), StringComparison.OrdinalIgnoreCase)) return;

        if (string.Equals(ext, ImageFormatsEnum.Jpg.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            _magickImage.Format = MagickFormat.Jpeg;
            _magickImage.Settings.Interlace = Interlace.Jpeg;
            
            var optimizer = new ImageOptimizer();
            optimizer.LosslessCompress(info);
            
            Console.WriteLine(_magickImage.Quality);
            _magickImage.Quality = CalculateQuality(
                currQuality: (byte)_magickImage.Quality, 
                qualityDemotion: 20,
                minQuality: 60);
            Console.WriteLine(_magickImage.Quality);
            _magickImage.Write(_filePath);
            info.Refresh();
        }

        if (string.Equals(ext, ImageFormatsEnum.Png.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            _magickImage.Settings.Interlace = Interlace.Png;
            _magickImage.Write(_filePath);
            info.Refresh();
        }
    }

    public ImageEditingFileInfoModel GetFileInfo(string? filePath)
    {
        var info = new FileInfo(filePath ?? _filePath);
        info.Refresh();
        return new ImageEditingFileInfoModel()
        {
            FileLength = $"{(info.Length / 1000).ToString()} kB",
            FileExt = info.Extension.ToLower().Replace(".", ""),
        };
    }

    public string Save()
    {
        var filePath = FileHelper.CreateEditingFilePath(_magickImage);
        _magickImage.Write(filePath);
        return filePath;
    }
}