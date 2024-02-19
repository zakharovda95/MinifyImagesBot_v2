using System.ComponentModel.DataAnnotations;

namespace MinifyImagesBot_v2.Enums;

internal enum KeyboardTypesEnum
{
    Format,
    Compression
}

internal enum FormatKeyboardEnum
{
    [Display(Name = "В WebP")]
    Webp,
    [Display(Name = "Без конвертации")]
    None,
}

internal enum CompressKeyboardEnum
{
    [Display(Name = "Максимальное сжатие")]
    MaxCompress,
    [Display(Name = "Щадящее сжатие")]
    MinCompress,
    [Display(Name = "Без сжатия")]
    None,
}