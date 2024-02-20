using MinifyImagesBot_v2.Enums;

namespace MinifyImagesBot_v2.Models;

internal record class ImageEditingParamsModel
{
    internal long? ChatId { get; init; }
    internal string? FilePath { get; set; }
    internal FormatKeyboardEnum? ResultFormat { get; set; }
    internal CompressKeyboardEnum? CompressLevel { get; set; }
    internal bool IsMultiple { get; set; } = false;
}