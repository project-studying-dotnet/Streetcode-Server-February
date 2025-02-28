/*using Xunit;
using Moq;
using AutoMapper;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.MediatR.News.Create;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

using NewsEntity = Streetcode.DAL.Entities.News.News;

namespace Streetcode.XUnitTest.MediatRTests.News.Create;

public class CreateNewsHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly CreateNewsHandler _handler;

    public CreateNewsHandlerTests() =>
        _handler = new(_mapperMock.Object, _repoMock.Object, _loggerMock.Object);

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenNewsCreated()
    {
        var newsDto = new NewsDTO { Title = "Test News", Text = "Content" };
        var newsEntity = new NewsEntity { Id = 1, Title = "Test News", Text = "Content", URL = "test.com" };
        _mapperMock.Setup(m => m.Map<NewsEntity>(newsDto)).Returns(newsEntity);
        _repoMock.Setup(r => r.NewsRepository.Create(newsEntity)).Returns(newsEntity);
        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(new(newsDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedNewsDTO_WhenNewsCreated()
    {
        var newsDto = new NewsDTO { Title = "Test News", Text = "Content" };
        var newsEntity = new NewsEntity { Id = 1, Title = "Test News", Text = "Content", URL = "test.com" };
        _mapperMock.Setup(m => m.Map<NewsEntity>(newsDto)).Returns(newsEntity);
        _mapperMock.Setup(m => m.Map<NewsDTO>(newsEntity)).Returns(newsDto);
        _repoMock.Setup(r => r.NewsRepository.Create(newsEntity)).Returns(newsEntity);
        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(new(newsDto), CancellationToken.None);

        Assert.Equal(newsDto, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMappingReturnsNull()
    {
        _mapperMock.Setup(m => m.Map<NewsEntity>(It.IsAny<NewsDTO>())).Returns((NewsEntity?)null!);

        var result = await _handler.Handle(new(new()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        var newsDto = new NewsDTO { Title = "Test News", Text = "Content" };
        var newsEntity = new NewsEntity { Title = "Test News", Text = "Content", URL = "test.com" };
        _mapperMock.Setup(m => m.Map<NewsEntity>(newsDto)).Returns(newsEntity);
        _repoMock.Setup(r => r.NewsRepository.Create(newsEntity)).Returns(newsEntity);
        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(new(newsDto), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
*/