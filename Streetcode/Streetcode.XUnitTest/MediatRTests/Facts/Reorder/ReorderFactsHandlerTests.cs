using System.Linq.Expressions;
using FluentResults;
using Moq;
using Xunit;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Facts.Reorder;

public class ReorderFactsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly ReorderFactsHandler _handler;

    public ReorderFactsHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new ReorderFactsHandler(
            _mockRepo.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenFactsExist_ReturnsSuccessResult()
    {
        // Arrange
        var facts = new List<ReorderFactDTO>
        {
            new() { Id = 1, Index = 0 }
        };

        var fact = new Fact { Id = 1 };
        Fact? updatedFact = null;

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                null))
            .ReturnsAsync(fact);

        _mockRepo.Setup(r => r.FactRepository.Update(It.IsAny<Fact>()))
            .Callback<Fact>(f => updatedFact = f);

        _mockRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(
            new ReorderFactsCommand(facts),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepo.Verify(
            r => r.FactRepository.Update(It.IsAny<Fact>()),
            Times.Once);
        Assert.NotNull(updatedFact);
        Assert.Equal(0, updatedFact.Index);
    }

    [Fact]
    public async Task Handle_WhenFactNotFound_ReturnsFailure()
    {
        // Arrange
        var facts = new List<ReorderFactDTO>
        {
            new() { Id = 1, Index = 0 }
        };

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                null))
            .ReturnsAsync((Fact?)null);

        // Act
        var result = await _handler.Handle(
            new ReorderFactsCommand(facts),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<ReorderFactsCommand>(),
                It.IsAny<string>()),
            Times.Once);
    }
}
