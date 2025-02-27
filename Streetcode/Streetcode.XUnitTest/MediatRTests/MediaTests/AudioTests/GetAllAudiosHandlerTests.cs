using Moq;
using AutoMapper;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetAll;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
﻿using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests;

public class GetAllAudiosHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetAllAudiosHandler _handler;

    public GetAllAudiosHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new GetAllAudiosHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _blobServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenNoAudiosFound()
    {
        // Arrange
        var query = new GetAllAudiosQuery();
        _repositoryWrapperMock
            .Setup(repo => repo.AudioRepository.GetAllAsync(null, null))
            .ReturnsAsync(() => null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        _loggerMock.Verify(logger => logger.LogError(It.IsAny<object>(), "Cannot find any audios"));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenAudiosExist()
    {
        // Arrange
        var query = new GetAllAudiosQuery();
        var audios = new List<Audio>
        {
            new Audio { Id = 1, BlobName = "audio1.mp3" },
            new Audio { Id = 2, BlobName = "audio2.mp3" }
        };
        var audioDtos = new List<AudioDTO>
        {
            new AudioDTO { BlobName = "audio1.mp3" },
            new AudioDTO { BlobName = "audio2.mp3" }
        };

        _repositoryWrapperMock
            .Setup(repo => repo.AudioRepository.GetAllAsync(null, null))
            .ReturnsAsync(audios);

        _mapperMock
            .Setup(mapper => mapper.Map<IEnumerable<AudioDTO>>(audios))
            .Returns(audioDtos);

        _blobServiceMock
            .Setup(blob => blob.FindFileInStorageAsBase64(It.IsAny<string>()))
            .Returns("base64encodeddata");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
        Assert.All(result.Value, audio => Assert.Equal("base64encodeddata", audio.Base64));

        _blobServiceMock.Verify(blob => blob.FindFileInStorageAsBase64(It.IsAny<string>()), Times.Exactly(2));
    }
}