using System.Linq.Expressions;
using FluentResults;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Update;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore.Query;

namespace Streetcode.XUnitTest.MediatRTests.Facts.Update;

public class UpdateFactHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly UpdateFactHandler _handler;

    public UpdateFactHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new UpdateFactHandler(
            _mockRepo.Object,
            new Mock<AutoMapper.IMapper>().Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenFactExists_ReturnsSuccessResult()
    {
        // Arrange
        var factId = 1;
        var factDto = new FactUpdateCreateDTO { Id = factId, Title = "Updated Title" };
        var existingFact = new Fact { Id = factId };

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Fact, bool>>>(), null))
            .ReturnsAsync(existingFact);

        // Act
        var result = await _handler.Handle(
            new UpdateFactCommand(factDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockRepo.Verify(r => r.FactRepository.Update(It.IsAny<Fact>()), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenFactNotFound_ReturnsFailure()
    {
        // Arrange
        var factId = 1;
        var factDto = new FactUpdateCreateDTO { Id = factId };

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Fact, bool>>>(), It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync((Fact?)null);

        // Act
        var result = await _handler.Handle(
            new UpdateFactCommand(factDto),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<UpdateFactCommand>(),
                It.IsAny<string>()),
            Times.Once);
    }
}
