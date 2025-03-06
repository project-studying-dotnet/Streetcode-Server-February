using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update;

public class UpdateFactHandler : IRequestHandler<UpdateFactCommand, Result<FactDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;
    private readonly ILoggerService _logger;

    public UpdateFactHandler(
                         IRepositoryWrapper repositoryWrapper,
                         IMapper mapper,
                         IBlobService blobService,
                         ILoggerService logger)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _blobService = blobService;
        _logger = logger;
    }

    public async Task<Result<FactDTO>> Handle(UpdateFactCommand request, CancellationToken cancellationToken)
    {
        if (request.FactDTO.Id <= 0)
        {
            const string errorMsg = "Invalid fact ID for update";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var fact = _mapper.Map<DAL.Entities.Streetcode.TextContent.Fact>(request.FactDTO);
        if (fact is null)
        {
            const string errorMsg = "Cannot convert null to fact";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var existingFact = await _repositoryWrapper.FactRepository
            .GetFirstOrDefaultAsync(f => f.Id == request.FactDTO.Id);

        if (existingFact is null)
        {
            var errorMsg = $"Cannot find fact with id: {request.FactDTO.Id} to update";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        _mapper.Map(request.FactDTO, existingFact);
        var response = _mapper.Map<FactDTO>(existingFact);

        if (fact.Image is not null && !string.IsNullOrEmpty(response.Image?.BlobName))
        {
            response.Image.Base64 = _blobService.FindFileInStorageAsBase64(response.Image.BlobName);
        }
        else if (fact.ImageId != existingFact.ImageId && existingFact.ImageId.HasValue)
        {
            var img = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(x => x.Id == existingFact.ImageId);
            if (img != null)
            {
                _repositoryWrapper.ImageRepository.Delete(img);
            }
        }

        _repositoryWrapper.FactRepository.Update(existingFact);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (resultIsSuccess)
        {
            return Result.Ok(response);
        }
        else
        {
            const string errorMsg = "Failed to update fact";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}
