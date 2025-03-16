using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentResults;
using MediatR;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.HistoricalContext.Delete;

public class DeleteHistoryEventHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IHistoricalContextRepository> _mockHistoricalContextRepository;
    private readonly DeleteHistoryEventHandler _handler;

    public DeleteHistoryEventHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockHistoricalContextRepository = new Mock<IHistoricalContextRepository>();
        _mockLogger = new Mock<ILoggerService>();
        _mockRepositoryWrapper
            .Setup(repo => repo.HistoricalContextRepository)
            .Returns(_mockHistoricalContextRepository.Object);
        _handler = new DeleteHistoryEventHandler(
            _mockRepositoryWrapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenHistoricalEventExists()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(1);
        var historicalEvent = new DAL.Entities.Timeline.HistoricalContext { Id = 1 };
        _mockHistoricalContextRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.HistoricalContext, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync(historicalEvent);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallDelete_WhenHistoricalEventExists()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(1);
        var historicalEvent = new DAL.Entities.Timeline.HistoricalContext { Id = 1 };
        _mockHistoricalContextRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.HistoricalContext, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync(historicalEvent);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockHistoricalContextRepository.Verify(
            repo => repo.Delete(historicalEvent),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsync_WhenHistoricalEventExists()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(1);
        var historicalEvent = new DAL.Entities.Timeline.HistoricalContext { Id = 1 };
        _mockHistoricalContextRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.HistoricalContext, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync(historicalEvent);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo => repo.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogInformation_WhenHistoricalEventExists()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(1);
        var historicalEvent = new DAL.Entities.Timeline.HistoricalContext { Id = 1 };
        _mockHistoricalContextRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.HistoricalContext, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync(historicalEvent);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            logger => logger.LogInformation(
                It.Is<string>(s => s.Contains('1') && s.Contains("deleted"))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenHistoricalEventDoesNotExist()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(1);
        _mockHistoricalContextRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.HistoricalContext, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync((DAL.Entities.Timeline.HistoricalContext)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectErrorMessage_WhenHistoricalEventDoesNotExist()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(1);
        _mockHistoricalContextRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.HistoricalContext, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync((DAL.Entities.Timeline.HistoricalContext)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().Contain(
            e => e.Message.Contains("Cannot find historical event"));
    }

    [Fact]
    public async Task Handle_ShouldNotCallDelete_WhenHistoricalEventDoesNotExist()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(1);
        _mockHistoricalContextRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.HistoricalContext, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync((DAL.Entities.Timeline.HistoricalContext)null!);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockHistoricalContextRepository.Verify(
            repo => repo.Delete(It.IsAny<DAL.Entities.Timeline.HistoricalContext>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCallSaveChangesAsync_WhenHistoricalEventDoesNotExist()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(1);
        _mockHistoricalContextRepository
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.Is<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.HistoricalContext, bool>>>(
                    expr => true),
                null))
            .ReturnsAsync((DAL.Entities.Timeline.HistoricalContext)null!);

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
        var command = new DeleteHistoryEventCommand(-1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectErrorMessage_WhenIdIsInvalid()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(-1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().Contain(
            e => e.Message.Contains("must be more than 0"));
    }

    [Fact]
    public async Task Handle_ShouldNotCallGetFirstOrDefaultAsync_WhenIdIsInvalid()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(-1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockHistoricalContextRepository.Verify(
            repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<
                    System.Func<DAL.Entities.Timeline.HistoricalContext, bool>>>(),
                It.IsAny<System.Func<System.Linq.IQueryable<
                    DAL.Entities.Timeline.HistoricalContext>,
                    Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<
                        DAL.Entities.Timeline.HistoricalContext, object>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCallDelete_WhenIdIsInvalid()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(-1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockHistoricalContextRepository.Verify(
            repo => repo.Delete(It.IsAny<DAL.Entities.Timeline.HistoricalContext>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCallSaveChangesAsync_WhenIdIsInvalid()
    {
        // Arrange
        var command = new DeleteHistoryEventCommand(-1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo => repo.SaveChangesAsync(),
            Times.Never);
    }
}