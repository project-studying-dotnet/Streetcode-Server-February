using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.DTO.AdditionalContent.Tag;

public class TagDTO
{
    public int Id { get; set; }
    required public string Title { get; set; }
    public IEnumerable<StreetcodeDTO>? Streetcodes { get; set; }
}
