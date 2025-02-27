using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.News.GetById;

public class GetNewsByIdHandler
    : IRequestHandler<GetNewsByIdQuery, Result<NewsDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;
    private readonly ILoggerService _logger;

    public GetNewsByIdHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, IBlobService blobService, ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _blobService = blobService;
        _logger = logger;
    }

    public async Task<Result<NewsDTO>> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
    {
        int id = request.Id;
        var newsDto = _mapper.Map<NewsDTO>(await _repositoryWrapper.NewsRepository.GetFirstOrDefaultAsync(
            predicate: sc => sc.Id == id,
            include: scl => scl
                .Include(sc => sc.Image)));
        if(newsDto is null)
        {
            string errorMsg = $"No news by entered Id - {id}";
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
