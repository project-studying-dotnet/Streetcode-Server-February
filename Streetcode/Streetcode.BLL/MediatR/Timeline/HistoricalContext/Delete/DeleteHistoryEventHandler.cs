using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext.Delete;

public class DeleteHistoryEventHandler
    : IRequestHandler<DeleteHistoryEventCommand, Result<Unit>>
{
    private readonly IHistoricalContextRepository _historicalContextRepository;
    private readonly ILoggerService _logger;

    public DeleteHistoryEventHandler(
        IHistoricalContextRepository historicalContextRepository,
        ILoggerService logger)
    {
        _historicalContextRepository = historicalContextRepository;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        DeleteHistoryEventCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
        {
            return Result.Fail(
                new Error($"Id must be more than 0. Id was {request.Id}"));
        }

        var historicalEvent = await _historicalContextRepository
            .GetFirstOrDefaultAsync(
                predicate: he => he.Id == request.Id);

        if (historicalEvent is null)
        {
            return Result.Fail(
                new Error($"Cannot find historical event with id: {request.Id}"));
        }

        _historicalContextRepository.Delete(historicalEvent);
        _logger.LogInformation(
            $"Historical event with id: {request.Id} was deleted");

        return Result.Ok(Unit.Value);
    }
}