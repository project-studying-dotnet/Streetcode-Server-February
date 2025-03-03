namespace Streetcode.BLL.DTO.Streetcode.Types;

public class PersonStreetcodeDTO : StreetcodeDTO
{
    required public string FirstName { get; set; }
    public string? Rank { get; set; }
    required public string LastName { get; set; }
}
