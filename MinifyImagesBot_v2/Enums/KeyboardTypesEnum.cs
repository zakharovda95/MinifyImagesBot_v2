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
    [Display(Name = "Без форматирования")]
    None,
}

internal enum CompressKeyboardEnum
{
    [Display(Name = "C дополнительным сжатием")]
    WithCompress,
    [Display(Name = "Без сжатия")]
    None,
}