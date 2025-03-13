using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TagEntity = Streetcode.DAL.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag.GetAll;

public class GetAllTagsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllTagsHandler _handler;

    public GetAllTagsHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetAllTagsHandler(
            _mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsFailResult_WhenTagsAreNull()
    {
        _mockRepo.Setup(r => r.TagRepository.GetAllAsync(null, null))
            .ReturnsAsync((IEnumerable<TagEntity>)null!);

        var result = await _handler.Handle(
            new GetAllTagsQuery(), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_LogsError_WhenTagsAreNull()
    {
        _mockRepo.Setup(r => r.TagRepository.GetAllAsync(null, null))
            .ReturnsAsync((IEnumerable<TagEntity>)null!);

        await _handler.Handle(new GetAllTagsQuery(), CancellationToken.None);

        _mockLogger.Verify(
            l => l.LogError(It.IsAny<GetAllTagsQuery>(), It.Is<string>(
                s => s == "Cannot find any tags")),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsOkResult_WhenTagsExist()
    {
        var tags = new List<TagEntity>
        {
            new() { Title = "TestTag" },
            new() { Title = "TestTag" }
        };
        _mockRepo.Setup(r => r.TagRepository.GetAllAsync(null, null))
            .ReturnsAsync(tags);

        _mockMapper.Setup(m => m.Map<IEnumerable<TagDTO>>(tags))
            .Returns(new List<TagDTO>
            {
                new() { Title = "TestTag" },
                new() { Title = "TestTag" }
            });

        var result = await _handler.Handle(
            new GetAllTagsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
