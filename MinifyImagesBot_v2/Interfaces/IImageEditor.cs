using MinifyImagesBot_v2.Models;

namespace MinifyImagesBot_v2.Interfaces;

internal interface IImageEditor
{
    public ImageEditingResultModel Edit();
}