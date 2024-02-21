using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
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

    private ImageEditingResultModel HandlePng()
    {
        if (!string.IsNullOrEmpty(_userParams.FilePath))
        {
            using var magik = new MagickImage(_userParams.FilePath);
            if (_userParams.CompressLevel == CompressKeyboardEnum.MaxCompress)
            {
                magik.RemoveProfile("*"); // удаление всех мета
                magik.Settings.SetDefine(MagickFormat.Png, "compression-level", "9"); // сжатие
            }
            
            magik.Write(_userParams.FilePath);

            TryGetImageInfo(out var info);
            Console.Write(info);
            
            return new ImageEditingResultModel { IsSuccess = true };
        }
        return new ImageEditingResultModel { IsSuccess = false };
    }

    private ImageEditingResultModel HandleJpgJpeg()
    {
        return new ImageEditingResultModel { IsSuccess = true };
    }

    private ImageEditingResultModel HandleHeic()
    {
        return new ImageEditingResultModel { IsSuccess = true };
    }

    public ImageEditingResultModel Edit()
    {
        if (!TryGetImageInfo(out var info) || info.Format is null)
            return new ImageEditingResultModel { IsSuccess = false };

        var formatString = Enum.GetName(typeof(MagickFormat), info.Format);
        if (!Enum.TryParse<AvailableFormatsEnum>(formatString, ignoreCase: true, out var format))
            return new ImageEditingResultModel { IsSuccess = false };

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
            case AvailableFormatsEnum.Webp:
            default: return new ImageEditingResultModel { IsSuccess = false };
        }

        return new ImageEditingResultModel { IsSuccess = false };
    }
}