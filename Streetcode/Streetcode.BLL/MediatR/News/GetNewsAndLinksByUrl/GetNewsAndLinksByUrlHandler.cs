using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.News.GetNewsAndLinksByUrl;

public class GetNewsAndLinksByUrlHandler
    : IRequestHandler<GetNewsAndLinksByUrlQuery, Result<NewsDTOWithURLs>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;
    private readonly ILoggerService _logger;

    public GetNewsAndLinksByUrlHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IBlobService blobService, ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _blobService = blobService;
        _logger = logger;
    }

    public async Task<Result<NewsDTOWithURLs>> Handle(GetNewsAndLinksByUrlQuery request, CancellationToken cancellationToken)
    {
        string url = request.Url;
        var newsDto = _mapper.Map<NewsDTO>(await _repositoryWrapper.NewsRepository.GetFirstOrDefaultAsync(
            predicate: sc => sc.URL == url,
            include: scl => scl
                .Include(sc => sc.Image)));

        if (newsDto is null)
        {
            string errorMsg = $"No news by entered Url - {url}";
            _logger.LogError(request, errorMsg);

            return Result.Fail(errorMsg);
        }

        if (newsDto.Image is not null)
        {
            newsDto.Image.Base64 = _blobService.FindFileInStorageAsBase64(newsDto.Image.BlobName);
        }

        var news = (await _repositoryWrapper.NewsRepository.GetAllAsync()).ToList();
        var newsIndex = news.FindIndex(x => x.Id == newsDto.Id);
        string prevNewsLink = null;
        string nextNewsLink = null;

        if(newsIndex != 0)
        {
            prevNewsLink = news[newsIndex - 1].URL;
        }

        if(newsIndex != news.Count - 1)
        {
            nextNewsLink = news[newsIndex + 1].URL;
        }

        var randomNewsTitleAndLink = new RandomNewsDTO();

        var arrCount = news.Count;
        if (arrCount > 3)
        {
            if (newsIndex + 1 == arrCount - 1 || newsIndex == arrCount - 1)
            {
                randomNewsTitleAndLink.RandomNewsUrl = news[newsIndex - 2].URL;
                randomNewsTitleAndLink.Title = news[newsIndex - 2].Title;
            }
            else
            {
                randomNewsTitleAndLink.RandomNewsUrl = news[arrCount - 1].URL;
                randomNewsTitleAndLink.Title = news[arrCount - 1].Title;
            }
        }
        else
        {
            randomNewsTitleAndLink.RandomNewsUrl = news[newsIndex].URL;
            randomNewsTitleAndLink.Title = news[newsIndex].Title;
        }

        var newsDtoWithUrls = new NewsDTOWithURLs();
        newsDtoWithUrls.RandomNews = randomNewsTitleAndLink;
        newsDtoWithUrls.News = newsDto;
        newsDtoWithUrls.NextNewsUrl = nextNewsLink;
        newsDtoWithUrls.PrevNewsUrl = prevNewsLink;

        if (newsDtoWithUrls.News is null)
        {
            string errorMsg = $"No news by entered Url - {url}";
            _logger.LogError(request, errorMsg);

            return Result.Fail(errorMsg);
        }

        return Result.Ok(newsDtoWithUrls);
    }
}
