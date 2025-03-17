using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.Timeline.TimelineItem;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using StreetcodeEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem.Create;

public class CreateTimelineItemHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ILoggerService> _loggerServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTimelineItemHandler _handler;

    public CreateTimelineItemHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _loggerServiceMock = new Mock<ILoggerService>();
        _mapperMock = new Mock<IMapper>();

        _handler = new CreateTimelineItemHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object,
            _loggerServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenTimelineItemCreatedSuccessfully()
    {
        // Arrange
        var timelineItemDto = new TimelineItemCreateDTO
        {
            Title = "Test Timeline Item",
            Description = "Description of test timeline item",
            Date = DateTime.Now,
            DateViewPattern = (DateViewPattern)1,
            StreetcodeId = 1
        };

        var command = new CreateTimelineItemCommand(timelineItemDto);

        var timelineItem = new TimelineItemEntity
        {
            Title = timelineItemDto.Title,
            Description = timelineItemDto.Description,
            Date = timelineItemDto.Date,
            DateViewPattern = (DAL.Enums.DateViewPattern)timelineItemDto.DateViewPattern
        };

        var streetcode = new StreetcodeEntity { Id = 1 };

        _mapperMock
            .Setup(m => m.Map<TimelineItemEntity>(timelineItemDto))
            .Returns(timelineItem);

        _repositoryWrapperMock
            .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeEntity, bool>>>(),
                null))
            .ReturnsAsync(streetcode);

        _repositoryWrapperMock
            .Setup(r => r.TimelineRepository.Create(It.IsAny<TimelineItemEntity>()));

        _repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        timelineItem.Id.Should().Be(0);
        _repositoryWrapperMock.Verify(
            r => r.TimelineRepository.Create(It.IsAny<TimelineItemEntity>()),
            Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMappedTimelineItemIsNull()
    {
        // Arrange
        var timelineItemDto = new TimelineItemCreateDTO
        {
            Title = "Test Timeline Item",
            Description = "Description of test timeline item",
            Date = DateTime.Now,
            DateViewPattern = (DateViewPattern)1,
            StreetcodeId = 1
        };

        var command = new CreateTimelineItemCommand(timelineItemDto);

        _mapperMock
            .Setup(m => m.Map<TimelineItemEntity>(timelineItemDto))
            .Returns((TimelineItemEntity)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            e => e.Message == "Cannot convert null to timeline item");
        _loggerServiceMock.Verify(
            l => l.LogError(command, "Cannot convert null to timeline item"),
            Times.Once);
        _repositoryWrapperMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenStreetcodeDoesNotExist()
    {
        // Arrange
        var timelineItemDto = new TimelineItemCreateDTO
        {
            Title = "Test Timeline Item",
            Description = "Description of test timeline item",
            Date = DateTime.Now,
            DateViewPattern = (DateViewPattern)1,
            StreetcodeId = 999
        };

        var command = new CreateTimelineItemCommand(timelineItemDto);

        var timelineItem = new TimelineItemEntity
        {
            Title = timelineItemDto.Title,
            Description = timelineItemDto.Description,
            Date = timelineItemDto.Date,
            DateViewPattern = (DAL.Enums.DateViewPattern)timelineItemDto.DateViewPattern
        };

        _mapperMock
            .Setup(m => m.Map<TimelineItemEntity>(timelineItemDto))
            .Returns(timelineItem);

        _repositoryWrapperMock
            .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeEntity, bool>>>(),
                null))
            .ReturnsAsync((StreetcodeEntity)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            e => e.Message == "No existing streetcode with the id");
        _loggerServiceMock.Verify(
            l => l.LogError(command, "No existing streetcode with the id"),
            Times.Once);
        _repositoryWrapperMock.Verify(
            r => r.TimelineRepository.Create(It.IsAny<TimelineItemEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        // Arrange
        var timelineItemDto = new TimelineItemCreateDTO
        {
            Title = "Test Timeline Item",
            Description = "Description of test timeline item",
            Date = DateTime.Now,
            DateViewPattern = (DateViewPattern)1,
            StreetcodeId = 1
        };

        var command = new CreateTimelineItemCommand(timelineItemDto);

        var timelineItem = new TimelineItemEntity
        {
            Title = timelineItemDto.Title,
            Description = timelineItemDto.Description,
            Date = timelineItemDto.Date,
            DateViewPattern = (DAL.Enums.DateViewPattern)timelineItemDto.DateViewPattern
        };

        var streetcode = new StreetcodeEntity { Id = 1 };

        _mapperMock
            .Setup(m => m.Map<TimelineItemEntity>(timelineItemDto))
            .Returns(timelineItem);

        _repositoryWrapperMock
            .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeEntity, bool>>>(),
                null))
            .ReturnsAsync(streetcode);

        _repositoryWrapperMock
            .Setup(r => r.TimelineRepository.Create(It.IsAny<TimelineItemEntity>()));

        _repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            e => e.Message == "Failed to create a timeline item");
        _loggerServiceMock.Verify(
            l => l.LogError(command, "Failed to create a timeline item"),
            Times.Once);
        _repositoryWrapperMock.Verify(
            r => r.TimelineRepository.Create(It.IsAny<TimelineItemEntity>()),
            Times.Once);
    }
}
