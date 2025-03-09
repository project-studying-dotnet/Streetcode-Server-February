using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using AutoMapper;
using DALFact = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;

public class ReorderFactsHandler
    : IRequestHandler<ReorderFactsCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;
    private readonly IMapper _mapper;

    public ReorderFactsHandler(
        IRepositoryWrapper repositoryWrapper,
        ILoggerService logger,
        IMapper mapper)
    {
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Result<Unit>> Handle(
        ReorderFactsCommand request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            const string errorMsg = "Request is null";
            _logger.LogError(null, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (request.Facts is null)
        {
            const string errorMsg = "Facts collection is null";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (!request.Facts.Any())
        {
            const string errorMsg = "Facts list is empty";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (request.Facts.Any(f => f.Index < 0))
        {
            const string errorMsg = "Index cannot be negative";
            _logger.LogError(request, errorMsg);
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

            var mappedFact = _mapper.Map<DALFact>(factDto);
            fact.Index = mappedFact.Index;
            _repositoryWrapper.FactRepository.Update(fact);
        }

        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (!resultIsSuccess)
        {
            const string errorMsg = "Failed to reorder facts";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        return Result.Ok(Unit.Value);
    }
}
