using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create;

public class CreateFactHandler
    : IRequestHandler<CreateFactCommand, Result<Unit>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateFactHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(CreateFactCommand request, CancellationToken cancellationToken)
    {
        var newFact = _mapper.Map<DAL.Entities.Streetcode.TextContent.Fact>(request.newFact);
        if (newFact is null)
        {
            const string errorMsg = "Cannot convert null to fact";
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }

        var getStreetcodeById = await _repositoryWrapper.StreetcodeRepository
            .GetFirstOrDefaultAsync(
            predicate: s => s.Id == request.newFact.StreetcodeId);

        if (getStreetcodeById == null)
        {
            const string errorMsg = "No existing streetcode with the id";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        newFact.Id = 0;

        _repositoryWrapper.FactRepository.Create(newFact);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (resultIsSuccess)
        {
            return Result.Ok(Unit.Value);
        }
        else
        {
            const string errorMsg = "Failed to create a fact";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}
