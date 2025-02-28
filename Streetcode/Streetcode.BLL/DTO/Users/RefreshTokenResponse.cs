namespace Streetcode.BLL.DTO.Users;

public class RefreshTokenResponse
{
    required public string Token { get; set; }
    public DateTime ExpireAt { get; set; }
}
