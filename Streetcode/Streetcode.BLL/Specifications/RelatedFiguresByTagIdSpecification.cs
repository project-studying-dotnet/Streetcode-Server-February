using Ardalis.Specification;
using StreetcodeEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;

namespace Streetcode.BLL.Specifications;

public class StreetcodeByTagIdSpecification : Specification<StreetcodeEntity>
{
    public StreetcodeByTagIdSpecification(int tagId)
    {
        Query.Where(sc => sc.Status == DAL.Enums.StreetcodeStatus.Published &&
                          sc.Tags.Any(t => t.Id == tagId))
             .Include(sc => sc.Images)
             .Include(sc => sc.Tags);
    }
}
