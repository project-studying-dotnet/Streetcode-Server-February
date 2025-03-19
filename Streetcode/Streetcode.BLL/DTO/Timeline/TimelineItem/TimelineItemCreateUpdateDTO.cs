using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Timeline.TimelineItem;

public abstract class TimelineItemCreateUpdateDTO
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public DateViewPattern DateViewPattern { get; set; }
    public int StreetcodeId { get; set; }
}
