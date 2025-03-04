using System.Linq.Expressions;
using Xunit;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.News.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;

using NewsEntity = Streetcode.DAL.Entities.News.News;

namespace Streetcode.XUnitTest.MediatRTests.News.Delete;

public class DeleteNewsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly DeleteNewsHandler _handler;

    public DeleteNewsHandlerTests() =>
        _handler = new(
            _repoMock.Object,
            _loggerMock.Object);

    [Fact]
    public async Task Handle_NewsExists_DeletesSuccessfully()
    {
        var newsEntity = new NewsEntity
            { Id = 1, Title = "Title1", Text = "Text1", URL = "URL1" };

        _repoMock
            .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(),
                null))
            .ReturnsAsync(newsEntity);
        _repoMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new(1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_NewsNotFound_FailsWithError()
    {
        _repoMock
            .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(),
                null))
            .ReturnsAsync((NewsEntity)null!);

        var command = new DeleteNewsCommand(
            2);

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        Assert.True(result.IsFailed);

        _loggerMock.Verify(
            l => l.LogError(command, $"No news found by entered Id - {command.Id}"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteFails_FailsWithError()
    {
        var newsEntity = new NewsEntity
            { Id = 3, Title = "Title3", Text = "Text3", URL = "URL3" };

        _repoMock
            .Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(),
                null))
            .ReturnsAsync(newsEntity);
        _repoMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var command = new DeleteNewsCommand(
            3);

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        Assert.True(result.IsFailed);

        _loggerMock.Verify(
            l => l.LogError(command, "Failed to delete news"),
            Times.Once);
    }
}
