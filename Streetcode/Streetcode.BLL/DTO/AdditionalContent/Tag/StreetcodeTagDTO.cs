namespace Streetcode.BLL.DTO.AdditionalContent.Tag;

public class StreetcodeTagDTO
{
    public int Id { get; set; }
    required public string Title { get; set; }
    public bool IsVisible { get; set; }
    public int Index { get; set; }
}
