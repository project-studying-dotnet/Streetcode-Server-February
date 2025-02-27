using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;

public class GetCategoryContentByStreetcodeIdHandler
    : IRequestHandler<GetCategoryContentByStreetcodeIdQuery, Result<StreetcodeCategoryContentDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public GetCategoryContentByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<StreetcodeCategoryContentDTO>> Handle(GetCategoryContentByStreetcodeIdQuery request, CancellationToken cancellationToken)
    {
        if((await _repositoryWrapper.StreetcodeRepository
            .GetFirstOrDefaultAsync(s => s.Id == request.StreetcodeId)) == null)
        {
            string errorMsg = $"No such streetcode with id = {request.StreetcodeId}";
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }

        var streetcodeContent = await _repositoryWrapper.StreetcodeCategoryContentRepository
            .GetFirstOrDefaultAsync(
                sc => sc.StreetcodeId == request.StreetcodeId && sc.SourceLinkCategoryId == request.CategoryId);

        if (streetcodeContent == null)
        {
            string errorMsg = "The streetcode content is null";
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(_mapper.Map<StreetcodeCategoryContentDTO>(streetcodeContent));
    }
}
