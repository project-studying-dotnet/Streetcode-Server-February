using Xunit;
using Moq;
using AutoMapper;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.News.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;

using NewsEntity = Streetcode.DAL.Entities.News.News;

namespace Streetcode.XUnitTest.MediatRTests.News.Create;

public class CreateNewsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly CreateNewsHandler _handler;

    public CreateNewsHandlerTests() =>
        _handler = new(
            _mapperMock.Object,
            _repoMock.Object,
            _loggerMock.Object);

    [Fact]
    public async Task Handle_ValidNews_CreatesSuccessfully()
    {
        var newsDto = new NewsDTO
            { Title = "Title1", Text = "Text1", URL = "URL1" };
        var newsEntity = new NewsEntity
            { Id = 1, Title = "Title1", Text = "Text1", URL = "URL1" };

        _mapperMock
            .Setup(m => m.Map<NewsEntity>(newsDto))
            .Returns(newsEntity);
        _repoMock
            .Setup(r => r.NewsRepository.Create(newsEntity))
            .Returns(newsEntity);
        _repoMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(
            new(newsDto),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_NullMapping_FailsWithError()
    {
        _mapperMock
            .Setup(m => m.Map<NewsEntity>(It.IsAny<NewsDTO>()))
            .Returns((NewsEntity)null!);

        var command = new CreateNewsCommand(
            new NewsDTO
            {
                Title = "Title2",
                Text = "Text2",
                URL = "URL2"
            });

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        Assert.True(result.IsFailed);

        _loggerMock.Verify(
            l => l.LogError(command, "Cannot convert null to news"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_SaveFails_FailsWithError()
    {
        var newsDto = new NewsDTO
            { Title = "Title3", Text = "Text3", URL = "URL3" };
        var newsEntity = new NewsEntity
            { Title = "Title3", Text = "Text3", URL = "URL3" };

        _mapperMock
            .Setup(m => m.Map<NewsEntity>(newsDto))
            .Returns(newsEntity);
        _repoMock
            .Setup(r => r.NewsRepository.Create(newsEntity))
            .Returns(newsEntity);
        _repoMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var command = new CreateNewsCommand(
            newsDto);

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        Assert.True(result.IsFailed);

        _loggerMock.Verify(
            l => l.LogError(command, "Failed to create a news"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ImageIdZero_SetsToNull()
    {
        var newsEntity = new NewsEntity
            { ImageId = 0, Title = "Title4", Text = "Text4", URL = "URL4" };
        var newsDto = new NewsDTO
            { Title = "Title4", Text = "Text4", URL = "URL4" };

        _mapperMock
            .Setup(m => m.Map<NewsEntity>(newsDto))
            .Returns(newsEntity);
        _repoMock
            .Setup(r => r.NewsRepository.Create(newsEntity))
            .Returns(newsEntity);
        _repoMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        await _handler.Handle(
            new(newsDto),
            CancellationToken.None);

        Assert.Null(newsEntity.ImageId);
    }
}
