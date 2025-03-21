using Xunit;
using Moq;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;
using Streetcode.BLL.DTO.Timeline.TimelineItem;
using Streetcode.DAL.Repositories.Interfaces.Base;

using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem.GetById;

public class GetByIdTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLoggerService;
    private readonly GetTimelineItemByIdHandler _handler;

    public GetByIdTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLoggerService = new Mock<ILoggerService>();
        _handler = new GetTimelineItemByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLoggerService.Object);
    }

    [Fact]
    public async Task Handle_GivenValidId_ShouldReturnTimelineItem()
    {
        var query = new GetTimelineItemByIdQuery(1);

        var timelineItemById = new TimelineItemEntity { Id = 1, Title = "Event 1" };

        _mockRepositoryWrapper.Setup(repo => repo.TimelineRepository.GetFirstOrDefaultAsync(
                x => x.Id == query.Id,
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>?>()))
            .ReturnsAsync(timelineItemById);

        var timelineItemByIdDto = new TimelineItemDTO() { Id = 1, Title = "Event 1" };

        _mockMapper.Setup(mapper => mapper.Map<TimelineItemDTO>(timelineItemById))
            .Returns(timelineItemByIdDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(timelineItemByIdDto);
    }

    [Fact]
    public async Task Handle_GivenInvalidId_ShouldReturnNull()
    {
        var query = new GetTimelineItemByIdQuery(1);

        _mockRepositoryWrapper.Setup(repo => repo.TimelineRepository.GetFirstOrDefaultAsync(
                x => x.Id == query.Id,
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>?>()))
            .ReturnsAsync((TimelineItemEntity?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be($"Cannot find a timeline item with corresponding id: {query.Id}");

        _mockLoggerService.Verify(
            logger => logger.LogError(
                It.IsAny<GetTimelineItemByIdQuery>(),
                $"Cannot find a timeline item with corresponding id: {query.Id}"),
            Times.Once);
    }
}
