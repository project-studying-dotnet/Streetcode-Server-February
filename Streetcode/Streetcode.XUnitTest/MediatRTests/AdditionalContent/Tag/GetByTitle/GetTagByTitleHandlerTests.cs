using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByTitle;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TagEntity = Streetcode.DAL.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag.GetByTitle;

public class GetTagByTitleHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetTagByTitleHandler _handler;

    public GetTagByTitleHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new GetTagByTitleHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenTagNotFound()
    {
        var query = new GetTagByTitleQuery("NonExistingTitle");
        _repositoryWrapperMock.Setup(repo => repo.TagRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TagEntity, bool>>>(), null))
            .ReturnsAsync((TagEntity)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldCallLogger_WhenTagNotFound()
    {
        var query = new GetTagByTitleQuery("NonExistingTitle");
        _repositoryWrapperMock.Setup(repo => repo.TagRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TagEntity, bool>>>(), null))
            .ReturnsAsync((TagEntity)null!);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            logger => logger.LogError(
                query, It.Is<string>(
                    s => s.Contains("Cannot find any tag"))), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenTagFound()
    {
        var tag = new TagEntity { Title = "ExistingTitle" };
        var query = new GetTagByTitleQuery("ExistingTitle");

        _repositoryWrapperMock.Setup(repo => repo.TagRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TagEntity, bool>>>(), null))
            .ReturnsAsync(tag);

        _mapperMock.Setup(mapper => mapper.Map<TagDTO>(tag))
            .Returns(new TagDTO { Title = "ExistingTitle" });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
