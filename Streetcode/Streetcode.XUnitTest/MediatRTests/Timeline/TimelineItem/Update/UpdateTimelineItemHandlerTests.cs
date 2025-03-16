using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.Timeline.TimelineItem;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using StreetcodeEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem.Update;

public class UpdateTimelineItemHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ILoggerService> _loggerServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateTimelineItemHandler _handler;

    public UpdateTimelineItemHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerServiceMock = new Mock<ILoggerService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateTimelineItemHandler(
            _repositoryWrapperMock.Object,
            _loggerServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenTimelineItemUpdatedSuccessfully()
    {
        // Arrange
        var timelineDto = new TimelineItemUpdateDTO
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            Date = DateTime.UtcNow,
            DateViewPattern = 3,
            StreetcodeId = 2
        };

        var existingTimelineItem = new TimelineItemEntity
        {
            Id = 1,
            StreetcodeId = 2
        };

        var existingStreetcode = new StreetcodeEntity { Id = 2 };

        _repositoryWrapperMock
            .Setup(r => r.TimelineRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                null))
            .ReturnsAsync(existingTimelineItem);

        _repositoryWrapperMock
            .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeEntity, bool>>>(),
                null))
            .ReturnsAsync(existingStreetcode);

        _mapperMock.Setup(m => m.Map(timelineDto, existingTimelineItem));

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var command = new UpdateTimelineItemCommand(timelineDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public async Task Handle_ShouldReturnFail_WhenDateViewPatternIsOutOfRange(int invalidDateViewPattern)
    {
        // Arrange
        var timelineDto = new TimelineItemUpdateDTO
        {
            Id = 1,
            StreetcodeId = 2,
            DateViewPattern = invalidDateViewPattern
        };

        var existingTimelineItem = new TimelineItemEntity
        {
            Id = 1,
            StreetcodeId = 2
        };

        _repositoryWrapperMock
            .Setup(r => r.TimelineRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                null))
            .ReturnsAsync(existingTimelineItem);

        var command = new UpdateTimelineItemCommand(timelineDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Out of range DateViewPattern [0 - 3]");

        _loggerServiceMock.Verify(l => l.LogError(command, "Out of range DateViewPattern [0 - 3]"), Times.Once);
    }
}
