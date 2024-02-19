using ImageMagick;
using MinifyImagesBot_v2.Classes.Helpers;
using MinifyImagesBot_v2.Enums;
using MinifyImagesBot_v2.Interfaces;
using MinifyImagesBot_v2.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

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
        using var image = Image.Load(_filePath);
        
        var info = new FileInfo(_filePath);
        var ext = image.Metadata.DecodedImageFormat?.Name;

        int CalculateQuality(byte currQuality, byte qualityDemotion, byte minQuality)
        {
            if (currQuality > 95) qualityDemotion += 15;
            else if (currQuality > 90) qualityDemotion += 10;
            else if (currQuality > 80) qualityDemotion += 5;

            return (currQuality - qualityDemotion) < minQuality ? minQuality : currQuality - qualityDemotion;
        }
        
        var isJpg = string.Equals(ext, ImageFormatsEnum.Jpg.ToString(), StringComparison.OrdinalIgnoreCase);
        var isJpeg = string.Equals(ext, ImageFormatsEnum.Jpeg.ToString(), StringComparison.OrdinalIgnoreCase);
        var isPng = string.Equals(ext, ImageFormatsEnum.Png.ToString(), StringComparison.OrdinalIgnoreCase);
        var isHeic = string.Equals(ext, ImageFormatsEnum.Heic.ToString(), StringComparison.OrdinalIgnoreCase);
        var isWebp = string.Equals(ext, ImageFormatsEnum.Webp.ToString(), StringComparison.OrdinalIgnoreCase);

        /** удаление метаданных **/
        _magickImage.Strip();
        _magickImage.Write(_filePath);
        info.Refresh();
        
        if (isHeic || isWebp)
        {
           
        }
        else if (isJpeg || isJpg)
        {
            if (isJpg) _magickImage.Format = MagickFormat.Jpeg;
            
            _magickImage.Settings.Interlace = Interlace.Jpeg;

            Console.WriteLine(_magickImage.Quality);
            _magickImage.Quality = CalculateQuality(
                currQuality: (byte)_magickImage.Quality,
                qualityDemotion: 20,
                minQuality: 60);
            Console.WriteLine(_magickImage.Quality);
            
            _magickImage.Depth = 8;
            _magickImage.Write(_filePath);
            info?.Refresh();
        }
        else if (isPng)
        {

            if (image.Metadata.ExifProfile is not null) 
                image.Metadata.ExifProfile = null;
            
            if (image.Metadata.CicpProfile is not null) 
                image.Metadata.CicpProfile = null;
            
            var encoder = new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.Level9, 
                BitDepth = PngBitDepth.Bit8,
                FilterMethod = PngFilterMethod.Adaptive,
            };
            
            image.Save(_filePath, encoder);
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