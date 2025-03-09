using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Update;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.Facts.Update;

public class UpdateFactHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBlobService> _mockBlobService;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly UpdateFactHandler _handler;

    public UpdateFactHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockBlobService = new Mock<IBlobService>();
        _mockLogger = new Mock<ILoggerService>();

        _handler = new UpdateFactHandler(
            _mockRepo.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenTitleIsUpdated_ReturnsSuccessResult()
    {
        // Arrange
        var factId = 1;
        var factDto = new FactUpdateCreateDTO
        {
            Id = factId,
            Title = "Updated Title",
            FactContent = "Updated content",
            StreetcodeId = 1
        };
        var existingFact = new Fact { Id = factId };
        var mappedFact = new Fact { Id = factId };
        var responseDto = new FactDTO { Id = factId };

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                null))
            .ReturnsAsync(existingFact);
        _mockMapper.Setup(m => m.Map<Fact>(factDto))
            .Returns(mappedFact);
        _mockMapper.Setup(m => m.Map<FactDTO>(existingFact))
            .Returns(responseDto);
        _mockRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(
            new UpdateFactCommand(factDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Title", existingFact.Title);
    }

    [Fact]
    public async Task Handle_WhenFactContentIsUpdated_ReturnsSuccessResult()
    {
        // Arrange
        var factId = 1;
        var factDto = new FactUpdateCreateDTO
        {
            Id = factId,
            Title = "Updated Title",
            FactContent = "Updated content",
            StreetcodeId = 1
        };
        var existingFact = new Fact { Id = factId };
        var mappedFact = new Fact { Id = factId };
        var responseDto = new FactDTO { Id = factId };

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                null))
            .ReturnsAsync(existingFact);
        _mockMapper.Setup(m => m.Map<Fact>(factDto))
            .Returns(mappedFact);
        _mockMapper.Setup(m => m.Map<FactDTO>(existingFact))
            .Returns(responseDto);
        _mockRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(
            new UpdateFactCommand(factDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated content", existingFact.FactContent);
    }

    [Fact]
    public async Task Handle_WhenFactExists_ReturnsSuccessResult()
    {
        // Arrange
        var factId = 1;
        var factDto = new FactUpdateCreateDTO
        {
            Id = factId,
            Title = "Updated Title",
            FactContent = "Updated content",
            StreetcodeId = 1
        };
        var existingFact = new Fact { Id = factId };
        var mappedFact = new Fact { Id = factId };
        var responseDto = new FactDTO { Id = factId };

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                null))
            .ReturnsAsync(existingFact);
        _mockMapper.Setup(m => m.Map<Fact>(factDto))
            .Returns(mappedFact);
        _mockMapper.Setup(m => m.Map<FactDTO>(existingFact))
            .Returns(responseDto);
        _mockRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(
            new UpdateFactCommand(factDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepo.Verify(r => r.FactRepository.Update(existingFact), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenFactNotFound_ReturnsFailure()
    {
        // Arrange
        var factId = 1;
        var factDto = new FactUpdateCreateDTO { Id = factId };
        var mappedFact = new Fact { Id = factId };

        _mockMapper.Setup(m => m.Map<Fact>(factDto))
            .Returns(mappedFact);
        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                null))
            .ReturnsAsync((Fact?)null);

        // Act
        var result = await _handler.Handle(
            new UpdateFactCommand(factDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<UpdateFactCommand>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenImageDescriptionIsUpdatedWithoutImage_ReturnsSuccessResult()
    {
        // Arrange
        var factId = 1;
        var factDto = new FactUpdateCreateDTO
        {
            Id = factId,
            ImageDescription = "Updated image description",
            StreetcodeId = 1
        };
        var existingFact = new Fact { Id = factId };
        var mappedFact = new Fact { Id = factId };
        var responseDto = new FactDTO { Id = factId };

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                null))
            .ReturnsAsync(existingFact);
        _mockMapper.Setup(m => m.Map<Fact>(factDto))
            .Returns(mappedFact);
        _mockMapper.Setup(m => m.Map<FactDTO>(existingFact))
            .Returns(responseDto);
        _mockRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(
            new UpdateFactCommand(factDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated image description", factDto.ImageDescription);
    }

    [Fact]
    public async Task Handle_WithImageUpdate_HandlesImageCorrectly()
    {
        // Arrange
        var factId = 1;
        var oldImageId = 2;
        var newImageId = 3;

        var factDto = new FactUpdateCreateDTO
        {
            Id = factId,
            ImageId = newImageId,
            Image = new ImageDTO
            {
                Id = newImageId,
                BlobName = "new-image.jpg"
            }
        };
        var existingFact = new Fact { Id = factId, ImageId = oldImageId };
        var mappedFact = new Fact
        {
            Id = factId,
            ImageId = newImageId,
            Image = new DAL.Entities.Media.Images.Image { Id = newImageId }
        };
        var responseDto = new FactDTO
        {
            Id = factId,
            Image = new ImageDTO
            {
                Id = newImageId,
                BlobName = "new-image.jpg"
            }
        };

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                null))
            .ReturnsAsync(existingFact);
        _mockMapper.Setup(m => m.Map<Fact>(factDto))
            .Returns(mappedFact);
        _mockMapper.Setup(m => m.Map<FactDTO>(existingFact))
            .Returns(responseDto);
        _mockBlobService.Setup(
                b => b.FindFileInStorageAsBase64(It.IsAny<string>()))
            .Returns("base64-image-data");
        _mockRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(
            new UpdateFactCommand(factDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockBlobService.Verify(
            b => b.FindFileInStorageAsBase64(It.IsAny<string>()),
            Times.Once);
    }
}
