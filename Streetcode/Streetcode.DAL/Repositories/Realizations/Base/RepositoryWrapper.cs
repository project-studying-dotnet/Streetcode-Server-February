using System.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Interfaces;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Analytics;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Newss;
using Streetcode.DAL.Repositories.Interfaces.Partners;
using Streetcode.DAL.Repositories.Interfaces.Source;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Team;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Transactions;
using Streetcode.DAL.Repositories.Interfaces.Users;

namespace Streetcode.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly IServiceProvider _serviceProvider;
    private readonly StreetcodeDbContext _streetcodeDbContext;

    public RepositoryWrapper(IServiceProvider serviceProvider, StreetcodeDbContext streetcodeDbContext)
    {
        _serviceProvider = serviceProvider;
        _streetcodeDbContext = streetcodeDbContext;
    }

    public INewsRepository NewsRepository => GetRepository<INewsRepository>();
    public IFactRepository FactRepository => GetRepository<IFactRepository>();
    public IImageRepository ImageRepository => GetRepository<IImageRepository>();
    public ITeamRepository TeamRepository => GetRepository<ITeamRepository>();
    public ITeamPositionRepository TeamPositionRepository => GetRepository<ITeamPositionRepository>();
    public IAudioRepository AudioRepository => GetRepository<IAudioRepository>();
    public IStreetcodeCoordinateRepository StreetcodeCoordinateRepository => GetRepository<IStreetcodeCoordinateRepository>();
    public IVideoRepository VideoRepository => GetRepository<IVideoRepository>();
    public IArtRepository ArtRepository => GetRepository<IArtRepository>();
    public IStreetcodeArtRepository StreetcodeArtRepository => GetRepository<IStreetcodeArtRepository>();
    public IPartnersRepository PartnersRepository => GetRepository<IPartnersRepository>();
    public ISourceCategoryRepository SourceCategoryRepository => GetRepository<ISourceCategoryRepository>();
    public IStreetcodeCategoryContentRepository StreetcodeCategoryContentRepository => GetRepository<IStreetcodeCategoryContentRepository>();
    public IRelatedFigureRepository RelatedFigureRepository => GetRepository<IRelatedFigureRepository>();
    public IStreetcodeRepository StreetcodeRepository => GetRepository<IStreetcodeRepository>();
    public ISubtitleRepository SubtitleRepository => GetRepository<ISubtitleRepository>();
    public IStatisticRecordRepository StatisticRecordRepository => GetRepository<IStatisticRecordRepository>();
    public ITagRepository TagRepository => GetRepository<ITagRepository>();
    public ITermRepository TermRepository => GetRepository<ITermRepository>();
    public ITextRepository TextRepository => GetRepository<ITextRepository>();
    public ITimelineRepository TimelineRepository => GetRepository<ITimelineRepository>();
    public IToponymRepository ToponymRepository => GetRepository<IToponymRepository>();
    public ITransactLinksRepository TransactLinksRepository => GetRepository<ITransactLinksRepository>();
    public IHistoricalContextRepository HistoricalContextRepository => GetRepository<IHistoricalContextRepository>();
    public IPartnerSourceLinkRepository PartnerSourceLinkRepository => GetRepository<IPartnerSourceLinkRepository>();
    public IRelatedTermRepository RelatedTermRepository => GetRepository<IRelatedTermRepository>();
    public IUserRepository UserRepository => GetRepository<IUserRepository>();
    public IStreetcodeTagIndexRepository StreetcodeTagIndexRepository => GetRepository<IStreetcodeTagIndexRepository>();
    public IPartnerStreetcodeRepository PartnerStreetcodeRepository => GetRepository<IPartnerStreetcodeRepository>();
    public IPositionRepository PositionRepository => GetRepository<IPositionRepository>();
    public ITeamLinkRepository TeamLinkRepository => GetRepository<ITeamLinkRepository>();
    public IImageDetailsRepository ImageDetailsRepository => GetRepository<IImageDetailsRepository>();
    public IHistoricalContextTimelineRepository HistoricalContextTimelineRepository => GetRepository<IHistoricalContextTimelineRepository>();
    public IStreetcodeToponymRepository StreetcodeToponymRepository => GetRepository<IStreetcodeToponymRepository>();
    public IStreetcodeImageRepository StreetcodeImageRepository => GetRepository<IStreetcodeImageRepository>();

    public int SaveChanges()
    {
        return _streetcodeDbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _streetcodeDbContext.SaveChangesAsync();
    }

    public TransactionScope BeginTransaction()
    {
        return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }

    private T GetRepository<T>()
        where T : class
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}
