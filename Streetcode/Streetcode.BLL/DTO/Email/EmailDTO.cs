using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Email;

public class EmailDTO
{

    [MaxLength(80)]
    required public string From { get; set; }

    [StringLength(500, MinimumLength = 1)]
    required public string Content { get; set; }
}
