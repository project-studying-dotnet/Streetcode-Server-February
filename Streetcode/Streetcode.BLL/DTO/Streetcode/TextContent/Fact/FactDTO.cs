using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

public class FactDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int? ImageId { get; set; }
    public ImageDTO? Image { get; set; }
    public string? ImageDescription { get; set; }
    public string? FactContent { get; set; }
}
