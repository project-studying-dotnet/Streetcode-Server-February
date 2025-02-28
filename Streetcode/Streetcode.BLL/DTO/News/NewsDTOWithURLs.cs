namespace Streetcode.BLL.DTO.News;

public class NewsDTOWithURLs
{
    public NewsDTO? News { get; set; }

    public string? PrevNewsUrl { get; set; }

    public string? NextNewsUrl { get; set; }

    public RandomNewsDTO? RandomNews { get; set; } = new RandomNewsDTO();
}
