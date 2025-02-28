using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.News;

public class NewsDTO
{
    public int Id { get; set; }
    required public string Title { get; set; }
    required public string Text { get; set; }
    public int? ImageId { get; set; }
    required public string URL { get; set; }
    public ImageDTO? Image { get; set; }
    public DateTime CreationDate { get; set; }
}
