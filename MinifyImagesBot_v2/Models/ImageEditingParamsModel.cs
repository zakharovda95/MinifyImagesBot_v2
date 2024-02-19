using MinifyImagesBot_v2.Enums;

namespace MinifyImagesBot_v2.Models;

internal record struct ImageEditingParamsModel
{
    internal long? ChatId { get; init; }
    internal string? FilePath { get; init; }
    internal FormatKeyboardEnum? ResultFormat { get; set; }
    internal CompressKeyboardEnum? CompressLevel { get; set; }
}