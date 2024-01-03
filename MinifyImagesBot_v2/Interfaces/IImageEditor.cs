using MinifyImagesBot_v2.Enums;
using MinifyImagesBot_v2.Models;

namespace MinifyImagesBot_v2.Interfaces;

internal interface IImageEditor
{
    void ConvertImage(ImageFormatsEnum ext );
    void MinifyImage();

    ImageEditingFileInfoModel GetFileInfo();

    string Save();
}