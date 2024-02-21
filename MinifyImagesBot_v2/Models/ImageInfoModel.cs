using ImageMagick;

namespace MinifyImagesBot_v2.Models;

internal record class ImageInfoModel
{
    internal MagickFormat? Format { get; init; }
    internal Interlace? Interlace { get; init; }
    internal Density? Density { get; init; }
    internal CompressionMethod? Compression { get; init; }
    internal int? Quality { get; init; }
    internal string? Length { get; init; }
    internal string? Extension { get; init; }
};