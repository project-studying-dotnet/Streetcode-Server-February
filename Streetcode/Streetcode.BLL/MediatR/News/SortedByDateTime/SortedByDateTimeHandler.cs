﻿using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.News.SortedByDateTime;

public class SortedByDateTimeHandler
    : IRequestHandler<SortedByDateTimeQuery, Result<List<NewsDTO>>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    private readonly ILoggerService _logger;

    public SortedByDateTimeHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _blobService = blobService;
        _logger = logger;
    }

    public async Task<Result<List<NewsDTO>>> Handle(SortedByDateTimeQuery request, CancellationToken cancellationToken)
    {
        var news = await _repositoryWrapper.NewsRepository.GetAllAsync(
            include: cat => cat.Include(img => img.Image));
        if (news == null)
        {
            const string errorMsg = "There are no news in the database";
            _logger.LogError(request, errorMsg);

            return Result.Fail(errorMsg);
        }

        var newsDtos = _mapper.Map<IEnumerable<NewsDTO>>(news).OrderByDescending(x => x.CreationDate).ToList();

        foreach (var dto in newsDtos)
        {
            if (dto.Image is not null)
            {
                dto.Image.Base64 = _blobService.FindFileInStorageAsBase64(dto.Image.BlobName);
            }
        }

        return Result.Ok(newsDtos);
    }
}
