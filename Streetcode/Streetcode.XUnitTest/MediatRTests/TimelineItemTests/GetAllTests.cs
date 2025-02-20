using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetAll;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.TimelineItemTests;

public class GetAllTimelineItemsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllTimelineItemsHandler _handler;

    public GetAllTimelineItemsHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();

        _handler = new GetAllTimelineItemsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTimelineItems_WhenTheyExist()
    {
        var query = new GetAllTimelineItemsQuery();

        var timelineItems = new List<TimelineItem>
        {
            new() { Id = 1, Title = "Event 1" },
            new() { Id = 2, Title = "Event 2" }
        };

        _mockRepositoryWrapper.Setup(repo => repo.TimelineRepository.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItem, bool>>?>(),
                It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>?>()
            ))
            .ReturnsAsync(timelineItems);

        var timelineItemDtos = new List<TimelineItemDTO>
        {
            new() { Id = 1, Title = "Event 1" },
            new() { Id = 2, Title = "Event 2" }
        };

        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TimelineItemDTO>>(timelineItems))
            .Returns(timelineItemDtos);
        
        var result = await _handler.Handle(query, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(timelineItemDtos);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoTimelineItemsExist()
    {
        var query = new GetAllTimelineItemsQuery();

        _mockRepositoryWrapper.Setup(repo => repo.TimelineRepository.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItem, bool>>?>(),
                It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>?>()
            ))
            .ReturnsAsync((IEnumerable<TimelineItem>?)null);
        
        var result = await _handler.Handle(query, CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be("Cannot find any timelineItem");

        _mockLogger.Verify(
            logger => logger.LogError(It.IsAny<GetAllTimelineItemsQuery>(), "Cannot find any timelineItem"),
            Times.Once);
    }
}