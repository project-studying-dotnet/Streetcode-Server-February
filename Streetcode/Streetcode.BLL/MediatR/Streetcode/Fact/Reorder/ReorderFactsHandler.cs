using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;

public class ReorderFactsHandler
    : IRequestHandler<ReorderFactsCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public ReorderFactsHandler(
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        ReorderFactsCommand request,
        CancellationToken cancellationToken)
    {
        foreach (var factDto in request.Facts)
        {
            var fact = await _repositoryWrapper.FactRepository
                .GetFirstOrDefaultAsync(f => f.Id == factDto.Id);

            if (fact is null)
            {
                string errorMsg = $"Cannot find fact with id: {factDto.Id}";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            fact.Index = factDto.Index;
            _repositoryWrapper.FactRepository.Update(fact);
        }

        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (resultIsSuccess)
        {
            return Result.Ok(Unit.Value);
        }
        else
        {
            const string errorMsg = "Failed to reorder facts";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}