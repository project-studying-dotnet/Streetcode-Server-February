using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update;

public class UpdateFactHandlerr : IRequestHandler<UpdateFactCommand, Result<FactDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IBlobService _blobService;
    private readonly ILoggerService _logger;

    public UpdateFactHandlerr(
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

        var existingFact = await _repositoryWrapper.FactRepository
            .GetFirstOrDefaultAsync(f => f.Id == request.FactDTO.Id);
        if (existingFact is null)
        {
            var errorMsg = $"Cannot find fact with id: {request.FactDTO.Id} to update";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        if (!string.IsNullOrEmpty(request.FactDTO.Title))
        {
            existingFact.Title = request.FactDTO.Title;
        }

        if (!string.IsNullOrEmpty(request.FactDTO.FactContent))
        {
            existingFact.FactContent = request.FactDTO.FactContent;
        }

        if (!string.IsNullOrEmpty(request.FactDTO.ImageDescription))
        {
            _logger.LogInformation($"ImageDescription: {request.FactDTO.ImageDescription}");
        }

        if (request.FactDTO.ImageId.HasValue && request.FactDTO.ImageId != existingFact.ImageId)
        {
            if (existingFact.ImageId.HasValue && _repositoryWrapper.ImageRepository != null)
            {
                var oldImage = await _repositoryWrapper.ImageRepository.GetFirstOrDefaultAsync(x => x.Id == existingFact.ImageId);
                if (oldImage != null)
                {
                    _repositoryWrapper.ImageRepository.Delete(oldImage);
                }
            }

            existingFact.ImageId = request.FactDTO.ImageId;
        }

        _repositoryWrapper.FactRepository.Update(existingFact);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
        if (resultIsSuccess)
        {
            var response = _mapper.Map<FactDTO>(existingFact);
            if (response.Image is not null && !string.IsNullOrEmpty(response.Image.BlobName))
            {
                response.Image.Base64 = _blobService.FindFileInStorageAsBase64(response.Image.BlobName);
            }

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
