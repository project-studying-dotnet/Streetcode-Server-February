using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.News.GetByUrl;

public class GetNewsByUrlHandler
    : IRequestHandler<GetNewsByUrlQuery, Result<NewsDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;
    private readonly ILoggerService _logger;

    public GetNewsByUrlHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IBlobService blobService, ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _blobService = blobService;
        _logger = logger;
    }

    public async Task<Result<NewsDTO>> Handle(GetNewsByUrlQuery request, CancellationToken cancellationToken)
    {
        string url = request.Url;
        var newsDto = _mapper.Map<NewsDTO>(await _repositoryWrapper.NewsRepository.GetFirstOrDefaultAsync(
            predicate: sc => sc.URL == url,
            include: scl => scl
                .Include(sc => sc.Image)));
        if(newsDto is null)
        {
            string errorMsg = $"No news by entered Url - {url}";
            _logger.LogError(request, errorMsg);

            return Result.Fail(errorMsg);
        }

        if (newsDto.Image is not null)
        {
            newsDto.Image.Base64 = _blobService.FindFileInStorageAsBase64(newsDto.Image.BlobName);
        }

        return Result.Ok(newsDto);
    }
}
