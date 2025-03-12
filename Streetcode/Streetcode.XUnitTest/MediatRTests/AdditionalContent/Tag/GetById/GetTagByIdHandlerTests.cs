using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TagEntity = Streetcode.DAL.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag.GetById;

public class GetTagByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetTagByIdHandler _handler;

    public GetTagByIdHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new GetTagByIdHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_TagExists_ReturnsMappedTag()
    {
        var tag = new TagEntity { Id = 1, Title = "TestTag" };
        var query = new GetTagByIdQuery(1);
        var expectedDto = new TagDTO { Title = "TestTag" };

        _repositoryMock.Setup(r => r.TagRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TagEntity, bool>>>(), null))
            .ReturnsAsync(tag);
        _mapperMock.Setup(m => m.Map<TagDTO>(tag))
            .Returns(expectedDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_TagExists_ReturnsCorrectTagDTO()
    {
        var tag = new TagEntity { Id = 1, Title = "TestTag" };
        var query = new GetTagByIdQuery(1);
        var expectedDto = new TagDTO { Title = "TestTag" };

        _repositoryMock.Setup(r => r.TagRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TagEntity, bool>>>(), null))
            .ReturnsAsync(tag);
        _mapperMock.Setup(m => m.Map<TagDTO>(tag))
            .Returns(expectedDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(expectedDto, result.Value);
    }

    [Fact]
    public async Task Handle_TagDoesNotExist_ReturnsFailure()
    {
        var query = new GetTagByIdQuery(99);

        _repositoryMock.Setup(r => r.TagRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TagEntity, bool>>>(), null))
            .ReturnsAsync((TagEntity)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_TagDoesNotExist_LogsError()
    {
        var query = new GetTagByIdQuery(99);
        _repositoryMock.Setup(r => r.TagRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TagEntity, bool>>>(), null))
            .ReturnsAsync((TagEntity)null!);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogError(
                query, It.Is<string>(
                    s => s.Contains("Cannot find a Tag"))), Times.Once);
    }
}
