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
        var existingTimelineItem = await _repositoryWrapper.TimelineRepository
            .GetFirstOrDefaultAsync(
            t => t.Id == request.TimelineItemDTO.Id, null);
        if (existingTimelineItem == null)
        {
            const string errorMsg = "Cannot find TimelineItem";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

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

        _mapper.Map(request.TimelineItemDTO, existingTimelineItem);
        _repositoryWrapper.TimelineRepository.Update(existingTimelineItem!);

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
