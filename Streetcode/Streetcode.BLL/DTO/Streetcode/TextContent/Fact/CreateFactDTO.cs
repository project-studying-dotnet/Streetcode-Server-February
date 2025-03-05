using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

public class CreateFactDTO
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(600)]
    public string FactContent { get; set; } = null!;

    public int? ImageId { get; set; }

    public int StreetcodeId { get; set; }
}
