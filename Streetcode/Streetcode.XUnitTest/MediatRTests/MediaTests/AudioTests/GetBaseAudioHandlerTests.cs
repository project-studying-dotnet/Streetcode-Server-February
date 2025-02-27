using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests;

public class GetBaseAudioHandlerTests
{
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IAudioRepository> _audioRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetBaseAudioHandler _handler;

    public GetBaseAudioHandlerTests()
    {
        _blobServiceMock = new Mock<IBlobService>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _audioRepositoryMock = new Mock<IAudioRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock.Setup(r => r.AudioRepository).Returns(_audioRepositoryMock.Object);

        _handler = new GetBaseAudioHandler(_blobServiceMock.Object, _repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMemoryStream_WhenAudioExists()
    {
        // Arrange
        var request = new GetBaseAudioQuery(1);
        var audio = new Audio { Id = 1, BlobName = "audio-file.mp3" };
        var memoryStream = new MemoryStream();

        _audioRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Audio, bool>>>(), null))
            .ReturnsAsync(audio);

        _blobServiceMock
            .Setup(blob => blob.FindFileInStorageAsMemoryStream(audio.BlobName))
            .Returns(memoryStream);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(memoryStream);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenAudioNotFound()
    {
        // Arrange
        var request = new GetBaseAudioQuery(99);

        _audioRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Audio, bool>>>(), null))
            .ReturnsAsync((Audio)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("Cannot find an audio with corresponding id"));
    }
}
