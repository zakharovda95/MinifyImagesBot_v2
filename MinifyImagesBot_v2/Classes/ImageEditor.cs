using System.Diagnostics.CodeAnalysis;
using ImageMagick;
using MinifyImagesBot_v2.Classes.Helpers;
using MinifyImagesBot_v2.Enums;
using MinifyImagesBot_v2.Interfaces;
using MinifyImagesBot_v2.Models;

namespace MinifyImagesBot_v2.Classes;

internal class ImageEditor : IImageEditor
{
    private readonly ImageEditingParamsModel _userParams;

    internal ImageEditor(ImageEditingParamsModel userParams)
    {
        _userParams = userParams;
    }

    private bool TryGetImageInfo([MaybeNullWhen(false)] out ImageInfoModel info, string? imagePath = null)
    {
        if (!string.IsNullOrEmpty(imagePath) && !string.IsNullOrEmpty(_userParams.FilePath))
        {
            info = null;
            return false;
        }

        var imageInfo = new MagickImageInfo(imagePath ?? _userParams.FilePath!);
        var fileInfo = new FileInfo(imagePath ?? _userParams.FilePath!);
        info = new ImageInfoModel
        {
            Format = imageInfo.Format,
            Density = imageInfo.Density,
            Quality = imageInfo.Quality,
            Compression = imageInfo.Compression,
            Interlace = imageInfo.Interlace,
            Length = fileInfo.Length / 1000 + "kB",
            Extension = fileInfo.Extension,
        };
        return true;
    }
    
    private ImageEditingResultModel HandlePng() {}
    private ImageEditingResultModel HandleJpgJpeg() {}
    
    private ImageEditingResultModel HandleHeic() {}

    public ImageEditingResultModel Edit()
    {
        if (!TryGetImageInfo(out var info)) return new ImageEditingResultModel() { };
        if (!Enum.TryParse<AvailableFormatsEnum>(info.Format.ToString(), ignoreCase: true, out var format))
            return new ImageEditingResultModel() { };
        switch (format)
        {
            case AvailableFormatsEnum.Png:
                HandlePng();
                break;
            case AvailableFormatsEnum.Jpeg:
                HandleJpgJpeg();
                break;
            case AvailableFormatsEnum.Jpg:
                HandleJpgJpeg();
                break;
            case AvailableFormatsEnum.Heic:
                HandleHeic();
                break;
            default:
                return new ImageEditingResultModel() { };
        }

        return new ImageEditingResultModel() { };
    }
    
    
}