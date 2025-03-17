using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;

public class CreateTimelineItemHandler : IRequestHandler<CreateTimelineItemCommand, Result<Unit>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateTimelineItemHandler(
        IMapper mapper,
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        CreateTimelineItemCommand request,
        CancellationToken cancellationToken)
    {
        var newTimelineItem = _mapper.Map<TimelineItemEntity>(request.TimelineItemDTO);

        if (newTimelineItem is null)
        {
            const string errorMsg = "Cannot convert null to timeline item";
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        var getStreetcodeById = await _repositoryWrapper.StreetcodeRepository
            .GetFirstOrDefaultAsync(
                predicate: s => s.Id == request.TimelineItemDTO.StreetcodeId);

        if (getStreetcodeById == null)
        {
            const string errorMsg = "No existing streetcode with the id";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        newTimelineItem.Id = 0;
        _repositoryWrapper.TimelineRepository.Create(newTimelineItem);

        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (resultIsSuccess)
        {
            return Result.Ok(Unit.Value);
        }
        else
        {
            const string errorMsg = "Failed to create a timeline item";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}
