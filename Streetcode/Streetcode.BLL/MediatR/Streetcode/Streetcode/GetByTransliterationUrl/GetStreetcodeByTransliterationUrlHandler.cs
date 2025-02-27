using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByTransliterationUrl;

public class GetStreetcodeByTransliterationUrlHandler
    : IRequestHandler<GetStreetcodeByTransliterationUrlQuery, Result<StreetcodeDTO>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public GetStreetcodeByTransliterationUrlHandler(IRepositoryWrapper repository, IMapper mapper, ILoggerService logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<StreetcodeDTO>> Handle(GetStreetcodeByTransliterationUrlQuery request, CancellationToken cancellationToken)
    {
        var streetcode = await _repository.StreetcodeRepository
            .GetFirstOrDefaultAsync(
                predicate: st => st.TransliterationUrl == request.Url);

        if (streetcode == null)
        {
            string errorMsg = $"Cannot find streetcode by transliteration url: {request.Url}";
            _logger.LogError(request, errorMsg);

            return new Error(errorMsg);
        }

        var tagIndexed = await _repository.StreetcodeTagIndexRepository
                                        .GetAllAsync(
                                            t => t.StreetcodeId == streetcode.Id,
                                            include: q => q.Include(ti => ti.Tag));

        var streetcodeDto = _mapper.Map<StreetcodeDTO>(streetcode);
        streetcodeDto.Tags = _mapper.Map<List<StreetcodeTagDTO>>(tagIndexed);

        return Result.Ok(streetcodeDto);
    }
}
