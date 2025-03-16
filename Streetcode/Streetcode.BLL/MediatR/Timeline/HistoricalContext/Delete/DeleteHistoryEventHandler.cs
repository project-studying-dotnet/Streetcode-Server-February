using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext.Delete;

public class DeleteHistoryEventHandler
    : IRequestHandler<DeleteHistoryEventCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteHistoryEventHandler(
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
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

        var historicalEvent = await _repositoryWrapper.HistoricalContextRepository
            .GetFirstOrDefaultAsync(
                predicate: he => he.Id == request.Id);

        if (historicalEvent is null)
        {
            return Result.Fail(
                new Error(
                    $"Cannot find historical event with id: {request.Id}"));
        }

        _repositoryWrapper.HistoricalContextRepository.Delete(historicalEvent);
        await _repositoryWrapper.SaveChangesAsync();
        _logger.LogInformation(
            $"Historical event with id: {request.Id} was deleted");

        return Result.Ok(Unit.Value);
    }
}