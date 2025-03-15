using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.GetByStreetcodeId;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.XUnitTest.MediatRTests.Media.Video.GetByStreetcodeId;

public class GetVideoByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetVideoByStreetcodeIdHandler _handler;

    public GetVideoByStreetcodeIdHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetVideoByStreetcodeIdHandler(
            _mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenStreetcodeNotFound()
    {
        var request = new GetVideoByStreetcodeIdQuery(1);
        _mockRepo.Setup(r => r.VideoRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(), null))
            .ReturnsAsync((VideoEntity)null!);
        _mockRepo.Setup(r => r.StreetcodeRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync((StreetcodeContent)null!);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenVideoExists()
    {
        var request = new GetVideoByStreetcodeIdQuery(1);
        var video = new VideoEntity { StreetcodeId = 1 };
        var videoDTO = new VideoDTO();

        _mockRepo.Setup(r => r.VideoRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(), null))
            .ReturnsAsync(video);
        _mockMapper.Setup(m => m.Map<VideoDTO>(It.IsAny<VideoEntity>()))
            .Returns(videoDTO);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
