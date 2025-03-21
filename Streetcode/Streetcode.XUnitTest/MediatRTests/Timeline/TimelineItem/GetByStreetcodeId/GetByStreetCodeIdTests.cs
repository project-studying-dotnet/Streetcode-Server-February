using System.Linq.Expressions;
using Xunit;
using Moq;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;
using Streetcode.BLL.DTO.Timeline.TimelineItem;
using Streetcode.DAL.Repositories.Interfaces.Base;

using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem.GetByStreetcodeId;

public class GetByStreetCodeIdTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly GetTimelineItemsByStreetcodeIdHandler _handler;

    public GetByStreetCodeIdTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggerService = new Mock<ILoggerService>();
        _handler = new GetTimelineItemsByStreetcodeIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLoggerService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTimelineItems_WhenTheyExist()
    {
        var request = new GetTimelineItemsByStreetcodeIdQuery(1);

        var timelineItems = new List<TimelineItemEntity>
        {
            new() { Id = 1, StreetcodeId = request.StreetcodeId, Title = "Event 1" },
            new() { Id = 2, StreetcodeId = request.StreetcodeId, Title = "Event 2" }
        };

        var timelineItemsDtos = new List<TimelineItemDTO>
        {
            new() { Id = 1, Title = "Event 1" },
            new() { Id = 2, Title = "Event 2" }
        };

        _mockRepositoryWrapper.Setup(repo => repo.TimelineRepository.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>()))
            .ReturnsAsync(timelineItems);

        _mockMapper.Setup(m => m.Map<IEnumerable<TimelineItemDTO>>(timelineItems))
            .Returns(timelineItemsDtos);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(timelineItemsDtos);
    }

    [Fact]
    public async Task Handle_GivenInvalidStreetCodeId_ShouldReturnNull()
    {
        var request = new GetTimelineItemsByStreetcodeIdQuery(1);

        _mockRepositoryWrapper.Setup(repo => repo.TimelineRepository.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>?>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>?>()))
            .ReturnsAsync((IEnumerable<TimelineItemEntity>?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be($"Cannot find any timeline item by the streetcode id: {request.StreetcodeId}");

        _mockLoggerService.Verify(
            logger => logger.LogError(
                It.IsAny<GetTimelineItemsByStreetcodeIdQuery>(),
                $"Cannot find any timeline item by the streetcode id: {request.StreetcodeId}"),
            Times.Once);
    }
}
