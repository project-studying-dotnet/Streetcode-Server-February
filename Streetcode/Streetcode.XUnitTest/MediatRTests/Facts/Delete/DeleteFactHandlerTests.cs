using System.Linq.Expressions;
using Moq;
using Xunit;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using DALFact = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Fact.Delete;

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

    [Xunit.Fact]
    public async Task Handle_WhenFactExists_ReturnsSuccessResult()
    {
        // Arrange
        const int factId = 1;
        var fact = new DALFact { Id = factId };

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DALFact, bool>>>(),
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

    [Xunit.Fact]
    public async Task Handle_WhenFactNotFound_ReturnsFailure()
    {
        // Arrange
        const int factId = 1;

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DALFact, bool>>>(),
                null))
            .ReturnsAsync((DALFact?)null);

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
