namespace MinifyImagesBot_v2.Models;

internal record struct ImageEditingResultModel
{
    internal bool IsSuccess { get; init; }
    internal string? FilePath { get; init; }
    internal ImageEditingFileInfoModel? FileInfoBefore { get; init; } 
    internal ImageEditingFileInfoModel? FileInfoAfter { get; init; } 
};