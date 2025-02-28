using Xunit;
using Moq;
using Streetcode.BLL.MediatR.News.Create;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

using NewsEntity = Streetcode.DAL.Entities.News.News;

namespace Streetcode.XUnitTest.MediatRTests.News.Create;

public class CreateNewsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly CreateNewsHandler _handler;

    public CreateNewsHandlerTests() =>
        _handler = new(null!, _repoMock.Object, _loggerMock.Object);

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenNewsCreated()
    {
        var newsEntity = new NewsEntity { Id = 1, Title = "Test News", Text = "Content", URL = "test.com" };
        _repoMock.Setup(r => r.NewsRepository.Create(It.IsAny<NewsEntity>())).Returns(newsEntity);
        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(new(new()), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCreatedEntityIsNull()
    {
        _repoMock.Setup(r => r.NewsRepository.Create(It.IsAny<NewsEntity>())).Returns((NewsEntity?)null);

        var result = await _handler.Handle(new(new()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        _repoMock.Setup(r => r.NewsRepository.Create(It.IsAny<NewsEntity>())).Returns(new NewsEntity());
        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(new(new()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldSetImageIdToNull_WhenImageIdIsZero()
    {
        var newsEntity = new NewsEntity { ImageId = 0 };
        _repoMock.Setup(r => r.NewsRepository.Create(It.IsAny<NewsEntity>())).Returns(newsEntity);
        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(new(new()), CancellationToken.None);

        Assert.Null(newsEntity.ImageId);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenExceptionThrown()
    {
        _repoMock.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new Exception("DB error"));

        var result = await _handler.Handle(new(new()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
