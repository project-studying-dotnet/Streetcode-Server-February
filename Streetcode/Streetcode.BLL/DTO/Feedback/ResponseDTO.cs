namespace Streetcode.BLL.DTO.Feedback;

public class ResponseDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    required public string Email { get; set; }
    public string? Description { get; set; }
}
