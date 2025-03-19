using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Image.Delete;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using MediatR;

namespace Streetcode.XUnitTest.MediatRTests.Media.ImageTests.Delete;

public class DeleteImageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly DeleteImageHandler _handler;

    public DeleteImageHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryWrapper>();
        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new DeleteImageHandler(
            _repositoryMock.Object,
            _blobServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenImageDeletedSuccessfully()
    {
        // Arrange
        var imageId = 1;
        var image = new Image
        {
            Id = imageId,
            BlobName = "image1.jpg",
            MimeType = "image/jpg"
        };

        _repositoryMock
            .Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Image, bool>>>(), null))
            .ReturnsAsync(image);

        _repositoryMock.Setup(r => r.ImageRepository.Delete(image));
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var command = new DeleteImageCommand(imageId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);

        _repositoryMock.Verify(r => r.ImageRepository.Delete(image), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _blobServiceMock.Verify(b => b.DeleteFileInStorage(image.BlobName), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenImageNotFound()
    {
        // Arrange
        var imageId = 1;

        _repositoryMock
            .Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Image, bool>>>(), null))
            .ReturnsAsync((Image)null); // Симулюємо відсутність зображення

        var command = new DeleteImageCommand(imageId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == $"Cannot find an image with Id: {imageId}");

        _loggerMock.Verify(l => l.LogError(command, $"Cannot find an image with Id: {imageId}"), Times.Once);
        _repositoryMock.Verify(r => r.ImageRepository.Delete(It.IsAny<Image>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        _blobServiceMock.Verify(b => b.DeleteFileInStorage(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        // Arrange
        var imageId = 1;
        var image = new Image { Id = imageId, BlobName = "image1.jpg" };

        _repositoryMock
            .Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Image, bool>>>(), null))
            .ReturnsAsync(image);

        _repositoryMock.Setup(r => r.ImageRepository.Delete(image));
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var command = new DeleteImageCommand(imageId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Failed to delete an image");

        _loggerMock.Verify(l => l.LogError(command, "Failed to delete an image"), Times.Once);
        _blobServiceMock.Verify(b => b.DeleteFileInStorage(It.IsAny<string>()), Times.Never);
    }
}
