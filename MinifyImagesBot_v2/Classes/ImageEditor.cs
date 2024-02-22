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
        if (string.IsNullOrEmpty(imagePath) && string.IsNullOrEmpty(_userParams.FilePath))
        {
            info = null;
            return false;
        }

        var imageInfo = new MagickImageInfo(imagePath ?? _userParams.FilePath!);
        var fileInfo = new FileInfo(imagePath ?? _userParams.FilePath!);
        fileInfo.Refresh();
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

    private ImageEditingResultModel HandlePng(ImageInfoModel info)
    {
        if (!string.IsNullOrEmpty(_userParams.FilePath))
        {
            var path = "";
            if (_userParams.CompressLevel == CompressKeyboardEnum.MaxCompress)
            {
                
                using var magik = new MagickImage(_userParams.FilePath);

                magik.RemoveProfile("*");// удаление всех мета
                magik.Strip();
                magik.SetCompression(CompressionMethod.Zip);
                magik.SetBitDepth(8);
                magik.ClassType = ClassType.Pseudo;
                //magik.ColorType = ColorType.Optimize;
                magik.ColorSpace = ColorSpace.sRGB;
                path = FileHelper.CreateEditingFilePath(info.Extension ?? ".png");
                magik.Write(path);
            }

            TryGetImageInfo(out var infoAfter, imagePath: path);
            Console.WriteLine(infoAfter);
            return new ImageEditingResultModel
            {
                IsSuccess = true, 
                FilePath = path,
                FileInfoBefore = info,
                FileInfoAfter = infoAfter
            };
        }
        return new ImageEditingResultModel { IsSuccess = false };
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