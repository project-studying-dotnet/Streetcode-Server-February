using System.Linq.Expressions;
using Moq;
using Xunit;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Facts.Delete;

public class DeleteFactHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly DeleteFactHandler _handler;

    public DeleteFactHandlerTests() =>
        _handler = new(
            _repoMock.Object,
            _loggerMock.Object);

    [Fact]
    public async Task Handle_FactExists_DeletesSuccessfully()
    {
        var factEntity = new FactEntity
            { Id = 1, Title = "Title1" };

        _repoMock
            .Setup(r => r.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                null))
            .ReturnsAsync(factEntity);
        _repoMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new(1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_FactNotFound_FailsWithError()
    {
        _repoMock
            .Setup(r => r.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                null))
            .ReturnsAsync((FactEntity)null!);

        var command = new DeleteFactCommand(2);

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        Assert.True(result.IsFailed);

        _loggerMock.Verify(
            l => l.LogError(command, $"Cannot find fact with id: {command.Id}"),
            Times.Once);
    }
}
