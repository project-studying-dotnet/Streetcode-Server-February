using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;

public class DeleteTimelineItemHandler
    : IRequestHandler<DeleteTimelineItemCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteTimelineItemHandler(
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        DeleteTimelineItemCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            return Result.Fail(
                new Error(ValidatorMessages.IdMustBeGreaterThanZero + $": {request.Id}"));
        }

        var timelineItem = await _repositoryWrapper.TimelineRepository
            .GetFirstOrDefaultAsync(
                predicate: ti => ti.Id == request.Id);

        if (timelineItem is null)
        {
            return Result.Fail(
                new Error(
                    $"Cannot find timeline item with id: {request.Id}"));
        }

        _repositoryWrapper.TimelineRepository.Delete(timelineItem);
        await _repositoryWrapper.SaveChangesAsync();
        _logger.LogInformation(
            $"Timeline item with id: {request.Id} was deleted");

        return Result.Ok(Unit.Value);
    }
}