using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.GetAll;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.DAL.Repositories.Interfaces.Base;
using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.XUnitTest.MediatRTests.Media.Video.GetAll;

public class GetAllVideosHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllVideosHandler _handler;

    public GetAllVideosHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetAllVideosHandler(
            _mockRepo.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsFailResult_WhenVideosAreNull()
    {
        _mockRepo.Setup(r => r.VideoRepository.GetAllAsync(null, null))
                 .ReturnsAsync((IEnumerable<VideoEntity>)null!);

        var result = await _handler.Handle(
            new GetAllVideosQuery(), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_LogsError_WhenVideosAreNull()
    {
        _mockRepo.Setup(r => r.VideoRepository.GetAllAsync(null, null))
                 .ReturnsAsync((IEnumerable<VideoEntity>)null!);

        await _handler.Handle(new GetAllVideosQuery(), CancellationToken.None);

        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<GetAllVideosQuery>(), "Cannot find any videos"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsOkResult_WhenVideosExist()
    {
        var videos = new List<VideoEntity> { new() };
        _mockRepo.Setup(r => r.VideoRepository.GetAllAsync(null, null))
                 .ReturnsAsync(videos);
        _mockMapper.Setup(m => m.Map<IEnumerable<VideoDTO>>(videos))
                   .Returns(new List<VideoDTO> { new() });

        var result = await _handler.Handle(
            new GetAllVideosQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
