using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TagEntity = Streetcode.DAL.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.MediatRTests
    .AdditionalContent.Tag.GetByStreetcodeId;

public class GetTagByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetTagByStreetcodeIdHandler _handler;

    public GetTagByStreetcodeIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new GetTagByStreetcodeIdHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenTagsNotFound()
    {
        var query = new GetTagByStreetcodeIdQuery(1);
        _repositoryWrapperMock
            .Setup(repo => repo.StreetcodeTagIndexRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeTagIndex, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeTagIndex>,
                IIncludableQueryable<StreetcodeTagIndex, object>>?>()))
            .ReturnsAsync((IEnumerable<StreetcodeTagIndex>)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenTagsNotFound()
    {
        var query = new GetTagByStreetcodeIdQuery(1);
        _repositoryWrapperMock
            .Setup(repo => repo.StreetcodeTagIndexRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeTagIndex, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeTagIndex>,
                IIncludableQueryable<StreetcodeTagIndex, object>>?>()))
            .ReturnsAsync((IEnumerable<StreetcodeTagIndex>)null!);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            logger => logger.LogError(
                It.IsAny<object>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderedTags_WhenTagsFound()
    {
        var query = new GetTagByStreetcodeIdQuery(1);
        var tags = new List<StreetcodeTagIndex>
        {
            new() { Index = 2, Tag = new TagEntity { Title = "TestTag" } },
            new() { Index = 1, Tag = new TagEntity { Title = "TestTag" } }
        };

        _repositoryWrapperMock
            .Setup(repo => repo.StreetcodeTagIndexRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeTagIndex, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeTagIndex>,
                IIncludableQueryable<StreetcodeTagIndex, object>>?>()))
            .ReturnsAsync(tags);

        _mapperMock.Setup(mapper => mapper
            .Map<IEnumerable<StreetcodeTagDTO>>(
                It.IsAny<IEnumerable<StreetcodeTagIndex>>()))
            .Returns(new List<StreetcodeTagDTO>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
