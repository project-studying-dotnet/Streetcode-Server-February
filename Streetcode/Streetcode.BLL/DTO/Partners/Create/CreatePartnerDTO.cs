using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.DTO.Partners.Create;

public class CreatePartnerDTO
{
    public int Id { get; set; }
    public bool IsKeyPartner { get; set; }
    public bool IsVisibleEverywhere { get; set; }
    required public string Title { get; set; }
    public string? Description { get; set; }
    public string? TargetUrl { get; set; }
    public int LogoId { get; set; }
    public string? UrlTitle { get; set; }
    public List<CreatePartnerSourceLinkDTO>? PartnerSourceLinks { get; set; }
    public List<StreetcodeShortDTO> Streetcodes { get; set; } = new List<StreetcodeShortDTO>();
}
