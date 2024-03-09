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
        info = new ImageInfoModel();
        return true;
    }

    private ImageEditingResultModel HandlePng(ImageInfoModel info)
    {
        return new ImageEditingResultModel { IsSuccess = true };
    }

    private ImageEditingResultModel HandleJpgJpeg(ImageInfoModel info)
    {
        return new ImageEditingResultModel { IsSuccess = true };
    }

    private ImageEditingResultModel HandleHeic(ImageInfoModel info)
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
               return HandlePng(info);
            case AvailableFormatsEnum.Jpeg:
                return HandleJpgJpeg(info);
            case AvailableFormatsEnum.Jpg:
                return HandleJpgJpeg(info);
            case AvailableFormatsEnum.Heic:
                return HandleHeic(info);
            case AvailableFormatsEnum.Webp:
            default: return new ImageEditingResultModel { IsSuccess = false };
        }
    }
}