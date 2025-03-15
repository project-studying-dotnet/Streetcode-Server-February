namespace Streetcode.BLL.DTO.Timeline.TimelineItem;

public class TimelineItemCreateDTO
{
    required public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public int DateViewPattern { get; set; }
    public int StreetcodeId { get; set; }
}
