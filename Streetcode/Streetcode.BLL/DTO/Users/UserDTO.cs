using System.ComponentModel.DataAnnotations;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Users;

public class UserDTO
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    required public string Name { get; set; }
    [Required]
    [MaxLength(50)]
    required public string Surname { get; set; }
    [Required]
    [EmailAddress]
    required public string Email { get; set; }
    [Required]
    [MaxLength(20)]
    required public string Login { get; set; }
    [Required]
    [MaxLength(20)]
    required public string Password { get; set; }
    [Required]
    public UserRole Role { get; set; }
}
