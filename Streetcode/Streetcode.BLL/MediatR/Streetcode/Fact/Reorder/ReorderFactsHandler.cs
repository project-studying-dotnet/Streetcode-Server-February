using System.Linq.Expressions;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;

public class ReorderFactsHandler : IRequestHandler<ReorderFactsCommand, Result<Unit>>
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
        if (request is null)
        {
            const string errorMsg = "Request is null";
            _logger.LogError(new { Message = errorMsg }, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (request.Facts is null)
        {
            const string errorMsg = "Facts collection is null";
            _logger.LogError(new { Message = errorMsg }, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (!request.Facts.Any())
        {
            const string errorMsg = "Facts list is empty";
            _logger.LogError(new { Message = errorMsg }, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (request.Facts.Any(f => f.Index < 0))
        {
            const string errorMsg = "Index cannot be negative";
            _logger.LogError(new { Message = errorMsg }, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (request.Facts.Any(f => f.Id <= 0))
        {
            const string errorMsg = "Id must be positive";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var distinctIds = request.Facts.Select(f => f.Id).Distinct();
        if (distinctIds.Count() != request.Facts.Count())
        {
            const string errorMsg = "Duplicate fact ids are not allowed";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var distinctIndexes = request.Facts.Select(f => f.Index).Distinct();
        if (distinctIndexes.Count() != request.Facts.Count())
        {
            const string errorMsg = "Duplicate indexes are not allowed";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        foreach (var factReorderDto in request.Facts)
        {
            var fact = await _repositoryWrapper.FactRepository
                .GetFirstOrDefaultAsync(f => f.Id == factReorderDto.Id);

            if (fact is null)
            {
                string errorMsg = $"Cannot find fact with id: {factReorderDto.Id}";
                _logger.LogError(new { Message = errorMsg }, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            fact.Index = factReorderDto.Index;
            _repositoryWrapper.FactRepository.Update(fact);
        }

        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (!resultIsSuccess)
        {
            const string errorMsg = "Failed to reorder facts";
            _logger.LogError(new { Message = errorMsg }, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(Unit.Value);
    }
}
