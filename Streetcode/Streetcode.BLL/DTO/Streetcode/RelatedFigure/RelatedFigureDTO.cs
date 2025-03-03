using Streetcode.BLL.DTO.AdditionalContent.Tag;

namespace Streetcode.BLL.DTO.Streetcode.RelatedFigure;

public class RelatedFigureDTO
{
    public int Id { get; set; }
    required public string Title { get; set; }
    required public string Url { get; set; }
    public string? Alias { get; set; }
    public int ImageId { get; set; }
    required public IEnumerable<TagDTO> Tags { get; set; }
}
