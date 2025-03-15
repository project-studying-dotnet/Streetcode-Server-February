using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.GetById;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.DAL.Repositories.Interfaces.Base;
using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.XUnitTest.MediatRTests.Media.Video.GetById;

public class GetVideoByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetVideoByIdHandler _handler;

    public GetVideoByIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new GetVideoByIdHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_VideoExists_ReturnsSuccessResult()
    {
        var query = new GetVideoByIdQuery(1);
        var video = new VideoEntity { Id = 1, Title = "Test Video" };
        var videoDto = new VideoDTO { Id = 1, Description = "Test Video" };

        _repositoryWrapperMock.Setup(r => r.VideoRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(), null))
            .ReturnsAsync(video);

        _mapperMock.Setup(m => m.Map<VideoDTO>(video))
            .Returns(videoDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_VideoExists_ReturnsMappedVideoDTO()
    {
        var query = new GetVideoByIdQuery(1);
        var video = new VideoEntity { Id = 1, Title = "Test Video" };
        var videoDto = new VideoDTO { Id = 1, Description = "Test Video" };

        _repositoryWrapperMock.Setup(r => r.VideoRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(), null))
            .ReturnsAsync(video);

        _mapperMock.Setup(m => m.Map<VideoDTO>(video))
            .Returns(videoDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(videoDto, result.Value);
    }

    [Fact]
    public async Task Handle_VideoDoesNotExist_ReturnsFailure()
    {
        var query = new GetVideoByIdQuery(999);

        _repositoryWrapperMock.Setup(r => r.VideoRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(), null))
            .ReturnsAsync((VideoEntity)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_VideoDoesNotExist_LogsError()
    {
        var query = new GetVideoByIdQuery(999);
        string expectedMessage =
            $"Cannot find a video with corresponding id: {query.Id}";

        _repositoryWrapperMock.Setup(r => r.VideoRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(), null))
            .ReturnsAsync((VideoEntity)null!);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogError(query, expectedMessage), Times.Once);
    }
}
