using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update;

public class UpdateFactHandler
    : IRequestHandler<UpdateFactCommand, Result<FactDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public UpdateFactHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<FactDTO>> Handle(UpdateFactCommand request, CancellationToken cancellationToken)
    {
        var existingFact = await _repositoryWrapper.FactRepository.GetFirstOrDefaultAsync(
            f => f.Id == request.FactDTO.Id);

        if (existingFact is null)
        {
            string errorMsg = $"Cannot find fact with id: {request.FactDTO.Id} to update";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        _mapper.Map(request.FactDTO, existingFact);

        _repositoryWrapper.FactRepository.Update(existingFact);
        await _repositoryWrapper.SaveChangesAsync();

        return Result.Ok(_mapper.Map<FactDTO>(existingFact));
    }
}
