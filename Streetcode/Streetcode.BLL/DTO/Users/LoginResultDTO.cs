namespace Streetcode.BLL.DTO.Users;

public class LoginResultDTO
{
    required public UserDTO User { get; set; }
    required public string Token { get; set; }
    public DateTime ExpireAt { get; set; }
}
