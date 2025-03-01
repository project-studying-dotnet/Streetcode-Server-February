using Xunit;
using Moq;
using Streetcode.BLL.MediatR.Instagram.GetAll;
using Streetcode.BLL.Interfaces.Instagram;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Instagram;

namespace Streetcode.XUnitTest.MediatRTests.Instagram.GetAll;

public class GetAllPostsHandlerTests
{
    private readonly Mock<IInstagramService> _instagramServiceMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly GetAllPostsHandler _handler;

    public GetAllPostsHandlerTests() =>
        _handler = new(_instagramServiceMock.Object, _loggerMock.Object);

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenPostsRetrieved()
    {
        var posts = new List<InstagramPost>
        {
            new()
            {
                Id = "01",
                MediaType = "VIDEO",
                MediaUrl = "https://mediaUrl.test",
                Permalink = "https://permalink.test",
                ThumbnailUrl = "https://thumbnailUrl.test"
            }
        };

        _instagramServiceMock.Setup(s => s.GetPostsAsync())
            .ReturnsAsync(posts);

        var query = new GetAllPostsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectPosts_WhenPostsRetrieved()
    {
        var posts = new List<InstagramPost>
        {
            new()
            {
                Id = "01",
                MediaType = "VIDEO",
                MediaUrl = "https://mediaUrl.test",
                Permalink = "https://permalink.test",
                ThumbnailUrl = "https://thumbnailUrl.test"
            }
        };

        _instagramServiceMock.Setup(s => s.GetPostsAsync())
            .ReturnsAsync(posts);

        var query = new GetAllPostsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(posts, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoPostsAvailable()
    {
        _instagramServiceMock.Setup(s => s.GetPostsAsync())
            .ReturnsAsync(new List<InstagramPost>());

        var query = new GetAllPostsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(result.Value);
    }
}
