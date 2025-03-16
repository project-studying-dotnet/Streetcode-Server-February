using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;

public class UpdateTimelineItemHandler
    : IRequestHandler<UpdateTimelineItemCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IMapper _mapper;

    public UpdateTimelineItemHandler(
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<Unit>> Handle(
        UpdateTimelineItemCommand request,
        CancellationToken cancellationToken)
    {
        if (request.TimelineItemDTO.Id <= 0)
        {
            const string errorMsg = "TimelineItem Id must be more than 0";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
        else if (request.TimelineItemDTO.StreetcodeId <= 0)
        {
            const string errorMsg = "StreetcodeId must be more than 0";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var existingTimelineItem = await _repositoryWrapper.TimelineRepository
            .GetFirstOrDefaultAsync(
            t => t.Id == request.TimelineItemDTO.Id, null);
        if (existingTimelineItem == null)
        {
            const string errorMsg = "Cannot find TimelineItem";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        int enumCount = Enum.GetNames(typeof(DateViewPattern)).Length - 1;
        if (request.TimelineItemDTO.DateViewPattern.HasValue
            && (request.TimelineItemDTO.DateViewPattern < 0
            || request.TimelineItemDTO.DateViewPattern > enumCount))
        {
            string errorMsg = $"Out of range DateViewPattern [0 - {enumCount}]";
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }

        if (request.TimelineItemDTO.StreetcodeId.HasValue)
        {
            var existingStreetcode =
                await _repositoryWrapper.StreetcodeRepository
                .GetFirstOrDefaultAsync(
                s => s.Id == request.TimelineItemDTO.StreetcodeId, null);

            if (existingStreetcode == null)
            {
                string errorMsg = $"Cannot find Streetcode by id = " +
                    $"{request.TimelineItemDTO.StreetcodeId}";
                _logger.LogError(request, errorMsg);

                return Result.Fail(new Error(errorMsg));
            }
        }

        _mapper.Map(request.TimelineItemDTO, existingTimelineItem);
        _repositoryWrapper.TimelineRepository.Update(existingTimelineItem);

        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (resultIsSuccess)
        {
            return Result.Ok(Unit.Value);
        }
        else
        {
            const string errorMsg = "Failed to update TimelineItem";
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }
    }
}
