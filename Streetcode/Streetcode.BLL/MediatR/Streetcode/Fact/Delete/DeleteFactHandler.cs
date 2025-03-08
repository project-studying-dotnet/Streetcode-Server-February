using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Delete;

public class DeleteFactHandler
    : IRequestHandler<DeleteFactCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public DeleteFactHandler(
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        DeleteFactCommand request,
        CancellationToken cancellationToken)
    {
        var fact = await _repositoryWrapper.FactRepository
            .GetFirstOrDefaultAsync(f => f.Id == request.Id, null);

        if (fact is null)
        {
            var notFoundMsg = $"Cannot find fact with id: {request.Id}";
            _logger.LogError(request, notFoundMsg);
            return Result.Fail(new Error(notFoundMsg));
        }

        _repositoryWrapper.FactRepository.Delete(fact);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (resultIsSuccess)
        {
            return Result.Ok(Unit.Value);
        }

        var deleteFailedMsg = "Failed to delete fact";
        _logger.LogError(request, deleteFailedMsg);
        return Result.Fail(new Error(deleteFailedMsg));
    }
}