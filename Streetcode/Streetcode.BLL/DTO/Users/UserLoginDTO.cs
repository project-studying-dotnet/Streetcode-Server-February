using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Users;

public class UserLoginDTO
{
    [Required]
    [MaxLength(20)]
    required public string Login { get; set; }
    [Required]
    [MaxLength(20)]
    required public string Password { get; set; }
}
