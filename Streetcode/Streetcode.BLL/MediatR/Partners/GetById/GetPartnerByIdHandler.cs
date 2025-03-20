using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.CacheService;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Partners.GetById;

public class GetPartnerByIdHandler
    : IRequestHandler<GetPartnerByIdQuery, Result<PartnerDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IRedisCacheService _cacheService;

    public GetPartnerByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger, IRedisCacheService cacheService)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Result<PartnerDTO>> Handle(GetPartnerByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"Partner:{request.Id}";

        var cachedPartner = await _cacheService.GetCacheValueAsync<PartnerDTO>(cacheKey);

        if (cachedPartner is not null)
        {
            return Result.Ok(cachedPartner);
        }

        var partner = await _repositoryWrapper
            .PartnersRepository
            .GetSingleOrDefaultAsync(
                predicate: p => p.Id == request.Id,
                include: p => p
                    .Include(pl => pl.PartnerSourceLinks));

        if (partner is null)
        {
            string errorMsg = $"Cannot find any partner with corresponding id: {request.Id}";
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }

        var partnerDto = _mapper.Map<PartnerDTO>(partner);

        await _cacheService.SetCacheValueAsync(cacheKey, partnerDto);

        return Result.Ok(partnerDto);
    }
}
