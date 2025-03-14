using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;

public class UpdateTimelineItemHandler
    : IRequestHandler<UpdateTimelineItemCommand, Result<Unit>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;
    private readonly ILoggerService _logger;

    public UpdateTimelineItemHandler(
        IRepositoryWrapper repositoryWrapper,
        IMapper mapper,
        IBlobService blobService,
        ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _blobService = blobService;
        _logger = logger;
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

        if (!string.IsNullOrWhiteSpace(request.TimelineItemDTO.Title))
        {
            existingTimelineItem.Title = request.TimelineItemDTO.Title;
        }

        if (!string.IsNullOrWhiteSpace(request.TimelineItemDTO.Description))
        {
            existingTimelineItem.Description = request
                                                 .TimelineItemDTO
                                                 .Description;
        }

        if (request.TimelineItemDTO.Date != null)
        {
            existingTimelineItem.Date = request.TimelineItemDTO.Date.Value;
        }

        int enumCount = Enum.GetNames(typeof(DateViewPattern)).Length - 1;
        if (request.TimelineItemDTO.DateViewPattern < 0
            || request.TimelineItemDTO.DateViewPattern > enumCount)
        {
            string errorMsg = $"Out of range DateViewPattern [0 - {enumCount}]";
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }
        else if (request.TimelineItemDTO.DateViewPattern.HasValue)
        {
            existingTimelineItem.DateViewPattern = (DateViewPattern)request.TimelineItemDTO.DateViewPattern.Value;
        }

        if (request.TimelineItemDTO.StreetcodeId != null)
        {
            var existingStreetcode = await _repositoryWrapper.StreetcodeRepository
                .GetFirstOrDefaultAsync(
                s => s.Id == request.TimelineItemDTO.StreetcodeId, null);
            if (existingStreetcode == null)
            {
                string errorMsg = $"Cannot find Streetcode by id = " +
                    $"{request.TimelineItemDTO.StreetcodeId}";
                _logger.LogError(request, errorMsg);

                return Result.Fail(new Error(errorMsg));
            }
            else if (request.TimelineItemDTO.StreetcodeId.HasValue)
            {
                existingTimelineItem.StreetcodeId = request.TimelineItemDTO.StreetcodeId.Value;
            }
        }

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
