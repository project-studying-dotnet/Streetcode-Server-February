using System.Reflection;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Services.Logging;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Services.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Services.BlobStorageService;
using Microsoft.FeatureManagement;
using Streetcode.BLL.Interfaces.Payment;
using Streetcode.BLL.Services.Payment;
using Streetcode.BLL.Interfaces.Instagram;
using Streetcode.BLL.Services.Instagram;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.Services.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Org.BouncyCastle.Bcpg.Sig;
using StackExchange.Redis;
using Streetcode.BLL.Behaviors;
using Streetcode.BLL.Services.CacheService;
using Streetcode.DAL.Repositories.Interfaces.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Analytics;
using Streetcode.DAL.Repositories.Interfaces.Media;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.News;
using Streetcode.DAL.Repositories.Interfaces.Partners;
using Streetcode.DAL.Repositories.Interfaces.Source;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Team;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Transactions;
using Streetcode.DAL.Repositories.Interfaces.Users;
using Streetcode.DAL.Repositories.Realizations.AdditionalContent;
using Streetcode.DAL.Repositories.Realizations.Analytics;
using Streetcode.DAL.Repositories.Realizations.Media;
using Streetcode.DAL.Repositories.Realizations.Media.Images;
using Streetcode.DAL.Repositories.Realizations.News;
using Streetcode.DAL.Repositories.Realizations.Partners;
using Streetcode.DAL.Repositories.Realizations.Source;
using Streetcode.DAL.Repositories.Realizations.Streetcode;
using Streetcode.DAL.Repositories.Realizations.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Realizations.Team;
using Streetcode.DAL.Repositories.Realizations.Timeline;
using Streetcode.DAL.Repositories.Realizations.Toponyms;
using Streetcode.DAL.Repositories.Realizations.Transactions;
using Streetcode.DAL.Repositories.Realizations.Users;
using Streetcode.WebApi.ExceptionHandlers;
using IRedisCacheService = Streetcode.BLL.Interfaces.CacheService.IRedisCacheService;

namespace Streetcode.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    private static Assembly[] _currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();

    public static void AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<IFactRepository, FactRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ITeamPositionRepository, TeamPositionRepository>();
        services.AddScoped<IAudioRepository, AudioRepository>();
        services.AddScoped<IStreetcodeCoordinateRepository, StreetcodeCoordinateRepository>();
        services.AddScoped<IVideoRepository, VideoRepository>();
        services.AddScoped<IArtRepository, ArtRepository>();
        services.AddScoped<IStreetcodeArtRepository, StreetcodeArtRepository>();
        services.AddScoped<IPartnersRepository, PartnersRepository>();
        services.AddScoped<ISourceCategoryRepository, SourceCategoryRepository>();
        services.AddScoped<IStreetcodeCategoryContentRepository, StreetcodeCategoryContentRepository>();
        services.AddScoped<IRelatedFigureRepository, RelatedFigureRepository>();
        services.AddScoped<IStreetcodeRepository, StreetcodeRepository>();
        services.AddScoped<ISubtitleRepository, SubtitleRepository>();
        services.AddScoped<IStatisticRecordRepository, StatisticRecordsRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ITermRepository, TermRepository>();
        services.AddScoped<ITextRepository, TextRepository>();
        services.AddScoped<ITimelineRepository, TimelineRepository>();
        services.AddScoped<IToponymRepository, ToponymRepository>();
        services.AddScoped<ITransactLinksRepository, TransactLinksRepository>();
        services.AddScoped<IHistoricalContextRepository, HistoricalContextRepository>();
        services.AddScoped<IPartnerSourceLinkRepository, PartnerSourceLinkRepository>();
        services.AddScoped<IRelatedTermRepository, RelatedTermRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStreetcodeTagIndexRepository, StreetcodeTagIndexRepository>();
        services.AddScoped<IPartnerStreetcodeRepository, PartnerStreetcodeRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<ITeamLinkRepository, TeamLinkRepository>();
        services.AddScoped<IImageDetailsRepository, ImageDetailsRepository>();
        services.AddScoped<IHistoricalContextTimelineRepository, HistoricalContextTimelineRepository>();
        services.AddScoped<IStreetcodeToponymRepository, StreetcodeToponymRepository>();
        services.AddScoped<IStreetcodeImageRepository, StreetcodeImageRepository>();
    }

    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddRepositoryServices();
        services.AddFeatureManagement();
        services.AddAutoMapper(_currentAssemblies);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(_currentAssemblies));
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped<ILoggerService, LoggerService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IInstagramService, InstagramService>();
        services.AddScoped<ITextService, AddTermsToTextService>();
    }

    public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
        services.AddSingleton(emailConfig);

        services.AddDbContext<StreetcodeDbContext>(options =>
        {
            options.UseSqlServer(connectionString, opt =>
            {
                opt.MigrationsAssembly(typeof(StreetcodeDbContext).Assembly.GetName().Name);
                opt.MigrationsHistoryTable("__EFMigrationsHistory", schema: "entity_framework");
            })
            .ConfigureWarnings(warnings =>
                warnings.Log(RelationalEventId.PendingModelChangesWarning));
        });

        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(connectionString);
        });

        services.AddHangfireServer();

        var corsConfig = configuration.GetSection("CORS").Get<CorsConfiguration>();
        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        services.AddHsts(opt =>
        {
            opt.Preload = true;
            opt.IncludeSubDomains = true;
            opt.MaxAge = TimeSpan.FromDays(30);
        });

        services.AddLogging();
        services.AddControllers();
    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi", Version = "v1" });
            opt.CustomSchemaIds(x => x.FullName);
        });
    }

    public static void AddValidationPipeline(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        var bllAssembly = _currentAssemblies.FirstOrDefault(a => a.GetName().Name == "Streetcode.BLL");
        services.AddValidatorsFromAssembly(bllAssembly, includeInternalTypes: true);
    }

    public static void AddExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

    public static void AddRedisCache(this IServiceCollection services, ConfigurationManager configuration)
    {
        var redisCacheSection = configuration.GetSection("Caching");

        services.Configure<RedisEnviromentVariables>(redisCacheSection);

        var redisCache = redisCacheSection.Get<RedisEnviromentVariables>();

        if (!redisCache.Enabled)
        {
            return;
        }

        var redisConfiguration = configuration.GetConnectionString("Redis");
        var redis = ConnectionMultiplexer.Connect(redisConfiguration);
        services.AddSingleton<IConnectionMultiplexer>(redis);

        services.AddSingleton<IRedisCacheService, RedisCacheService>();
    }

    public class CorsConfiguration
    {
        public List<string> AllowedOrigins { get; set; }
        public List<string> AllowedHeaders { get; set; }
        public List<string> AllowedMethods { get; set; }
        public int PreflightMaxAge { get; set; }
    }
}
