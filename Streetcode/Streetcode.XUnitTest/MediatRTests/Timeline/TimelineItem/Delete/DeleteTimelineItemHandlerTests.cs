using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentResults;
using MediatR;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem.Delete;

public class DeleteTimelineItemHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<ITimelineRepository> _mockTimelineRepository;
    private readonly DeleteTimelineItemHandler _handler;

    public DeleteTimelineItemHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockTimelineRepository = new Mock<ITimelineRepository>();
        _mockLogger = new Mock<ILoggerService>();

        _mockRepositoryWrapper
            .Setup(repo => repo.TimelineRepository)
            .Returns(_mockTimelineRepository.Object);

        _handler = new DeleteTimelineItemHandler(
            _mockRepositoryWrapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTimelineItemExists()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(1);
        var timelineItem = new DAL.Entities.Timeline.TimelineItem { Id = 1 };

        _mockTimelineRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.TimelineItem, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync(timelineItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallDelete_WhenTimelineItemExists()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(1);
        var timelineItem = new DAL.Entities.Timeline.TimelineItem { Id = 1 };

        _mockTimelineRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.TimelineItem, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync(timelineItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTimelineRepository.Verify(
            repo => repo.Delete(timelineItem),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsync_WhenTimelineItemExists()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(1);
        var timelineItem = new DAL.Entities.Timeline.TimelineItem { Id = 1 };

        _mockTimelineRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.TimelineItem, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync(timelineItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo => repo.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogInformation_WhenTimelineItemExists()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(1);
        var timelineItem = new DAL.Entities.Timeline.TimelineItem { Id = 1 };

        _mockTimelineRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.TimelineItem, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync(timelineItem);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            logger => logger.LogInformation(
                It.Is<string>(s => s.Contains('1') && s.Contains("deleted"))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTimelineItemDoesNotExist()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(1);

        _mockTimelineRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.TimelineItem, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync((DAL.Entities.Timeline.TimelineItem)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectErrorMessage_WhenTimelineItemDoesNotExist()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(1);

        _mockTimelineRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.TimelineItem, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync((DAL.Entities.Timeline.TimelineItem)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().Contain(
            e => e.Message.Contains("Cannot find timeline item"));
    }

    [Fact]
    public async Task Handle_ShouldNotCallDelete_WhenTimelineItemDoesNotExist()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(1);

        _mockTimelineRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.TimelineItem, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync((DAL.Entities.Timeline.TimelineItem)null!);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTimelineRepository.Verify(
            repo => repo.Delete(It.IsAny<DAL.Entities.Timeline.TimelineItem>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCallSaveChangesAsync_WhenTimelineItemDoesNotExist()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(1);

        _mockTimelineRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.TimelineItem, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync((DAL.Entities.Timeline.TimelineItem)null!);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo => repo.SaveChangesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenIdIsInvalid()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(-1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectErrorMessage_WhenIdIsInvalid()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(-1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().Contain(
            e => e.Message.Contains("must be greater than 0"));
    }

    [Fact]
    public async Task Handle_ShouldNotCallGetFirstOrDefaultAsync_WhenIdIsInvalid()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(-1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTimelineRepository.Verify(
            repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.TimelineItem, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<
                    DAL.Entities.Timeline.TimelineItem>,
                    Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<
                        DAL.Entities.Timeline.TimelineItem, object>>>()),
            Times.Never);
    }
}
