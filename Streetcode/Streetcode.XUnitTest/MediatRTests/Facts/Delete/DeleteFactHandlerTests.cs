using System.Linq.Expressions;
using FluentResults;
using Moq;
using Xunit;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.Facts.Delete;

public class DeleteFactHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly DeleteFactHandler _handler;

    public DeleteFactHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new DeleteFactHandler(
            _mockRepo.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenFactExists_ReturnsSuccessResult()
    {
        // Arrange
        var factId = 1;
        var fact = new Fact { Id = factId };

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                null))
            .ReturnsAsync(fact);

        _mockRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(
            new DeleteFactCommand(factId),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenFactNotFound_ReturnsFailure()
    {
        // Arrange
        var factId = 1;

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                null))
            .ReturnsAsync((Fact?)null);

        // Act
        var result = await _handler.Handle(
            new DeleteFactCommand(factId),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<DeleteFactCommand>(),
                It.IsAny<string>()),
            Times.Once);
    }
}
