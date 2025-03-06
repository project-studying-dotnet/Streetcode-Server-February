using System.ComponentModel.DataAnnotations;
using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

public class FactUpdateCreateDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(600, MinimumLength = 1)]
    public string FactContent { get; set; } = null!;

    public int? ImageId { get; set; }
    public ImageDTO? Image { get; set; }

    public int StreetcodeId { get; set; }
}
