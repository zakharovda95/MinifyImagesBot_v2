namespace MinifyImagesBot_v2.Models;

internal record class ImageEditingResultModel
{
    internal bool IsSuccess { get; init; }
    
    internal string? Message { get; init; }
    internal string? FilePath { get; init; }
    internal ImageInfoModel? FileInfoBefore { get; init; } 
    internal ImageInfoModel? FileInfoAfter { get; init; }
};