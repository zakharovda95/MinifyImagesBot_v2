using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using ImageMagick;
using ImageMagick.Formats;
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
            if (_userParams.CompressLevel == CompressKeyboardEnum.MaxCompress)
            {
                var settings = new MagickReadSettings
                {
                    Compression = CompressionMethod.ZipS,
                    ColorType = ColorType.PaletteAlpha,
                    Depth = 8
                };

                using var magik = new MagickImage(_userParams.FilePath, settings);
                
                magik.RemoveProfile("*"); // удаление всех мета
                magik.Strip();
                magik.FilterType = FilterType.Lanczos2Sharp;
                magik.Write(_userParams.FilePath);
            }
            
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
               return HandlePng();
            case AvailableFormatsEnum.Jpeg:
                return HandleJpgJpeg();
            case AvailableFormatsEnum.Jpg:
                return HandleJpgJpeg();
            case AvailableFormatsEnum.Heic:
                return HandleHeic();
            case AvailableFormatsEnum.Webp:
            default: return new ImageEditingResultModel { IsSuccess = false };
        }
    }
}