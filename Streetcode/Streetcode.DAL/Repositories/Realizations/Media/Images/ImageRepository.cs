using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Streetcode.DAL.Repositories.Realizations.Base;

namespace Streetcode.DAL.Repositories.Realizations.Media.Images;

public class ImageRepository
    : RepositoryBase<Image>, IImageRepository
{
    public ImageRepository(StreetcodeDbContext dbContext)
        : base(dbContext)
    {
    }
}
