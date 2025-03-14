using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Image.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Media.Image;

public class CreateImageHandlerTests
{
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<IRepositoryWrapper> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreateImageHandler _handler;

    public CreateImageHandlerTests()
    {
        _blobServiceMock = new Mock<IBlobService>();
        _repositoryMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new CreateImageHandler(
            _blobServiceMock.Object,
            _repositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenImageCreatedSuccessfully()
    {
        // Arrange
        var imageDto = new ImageFileBaseCreateDTO
        {
            BaseFormat = "someBaseFormat",
            Title = "imageTitle",
            Extension = "jpg"
        };

        var imageEntity = new DAL.Entities.Media.Images.Image();
        var createdImageDto = new ImageDTO
        {
            BlobName = "someHash.jpg",
            Base64 = "base64String"
        };

        _blobServiceMock
            .Setup(b => b.SaveFileInStorage(
                imageDto.BaseFormat,
                imageDto.Title,
                imageDto.Extension))
            .Returns("someHash");

        _mapperMock
            .Setup(m => m.Map<DAL.Entities.Media.Images.Image>(imageDto))
            .Returns(imageEntity);

        _repositoryMock
            .Setup(r => r.ImageRepository.CreateAsync(imageEntity))
            .Returns(Task.FromResult(imageEntity));

        _repositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock
            .Setup(m => m.Map<ImageDTO>(imageEntity))
            .Returns(createdImageDto);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsBase64(createdImageDto.BlobName))
            .Returns("base64String");

        var command = new CreateImageCommand(imageDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(createdImageDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFailsOrBase64IsEmpty()
    {
        // Arrange
        var imageDto = new ImageFileBaseCreateDTO
        {
            BaseFormat = "someBaseFormat",
            Title = "imageTitle",
            Extension = "jpg"
        };

        var imageEntity = new DAL.Entities.Media.Images.Image();
        var createdImageDto = new ImageDTO
        {
            BlobName = "someHash.jpg",
            Base64 = ""
        };

        _blobServiceMock
            .Setup(b => b.SaveFileInStorage(
                imageDto.BaseFormat,
                imageDto.Title,
                imageDto.Extension))
            .Returns("someHash");

        _mapperMock
            .Setup(m => m.Map<DAL.Entities.Media.Images.Image>(imageDto))
            .Returns(imageEntity);

        _repositoryMock
            .Setup(r => r.ImageRepository.CreateAsync(imageEntity))
            .ReturnsAsync(imageEntity);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        _mapperMock
            .Setup(m => m.Map<ImageDTO>(imageEntity))
            .Returns(createdImageDto);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsBase64(createdImageDto.BlobName))
            .Returns(string.Empty);

        var command = new CreateImageCommand(imageDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        if (result.Errors.Any(e => e.Message == "Failed to create an image"))
        {
            _loggerMock.Verify(
                l => l.LogError(command, "Failed to create an image"),
                Times.Once);
        }
        else if (result.Errors.Any(e => e.Message == "Failed to retrieve Base64"))
        {
            _loggerMock.Verify(
                l => l.LogError(command, "Failed to retrieve Base64"),
                Times.Once);
        }
    }
}
