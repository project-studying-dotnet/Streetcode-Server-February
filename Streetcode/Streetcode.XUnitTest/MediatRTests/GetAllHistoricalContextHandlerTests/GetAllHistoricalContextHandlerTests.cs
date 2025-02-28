namespace Streetcode.XUnitTest.MediatRTests.GetAllHistoricalContextHandlerTests;

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.GetAll;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

/// <summary>
/// Tests for <see cref="GetAllHistoricalContextHandler"/> class.
/// </summary>
public class GetAllHistoricalContextHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetAllHistoricalContextHandler _sut;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllHistoricalContextHandlerTests"/> class.
    /// </summary>
    public GetAllHistoricalContextHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();
        _sut = new GetAllHistoricalContextHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// GetAllAsync method of HistoricalContextRepository returns expected historical contexts.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task Handle_ShouldReturnHistoricalContexts_WhenHistoricalContextsExist()
    {
        var historicalContexts = new List<HistoricalContext>
        {
            new () { Id = 1, Title = "Title 1" },
            new () { Id = 2, Title = "Title 2" },
        };

        _repositoryWrapperMock.Setup(repo => repo.HistoricalContextRepository.GetAllAsync(
            It.IsAny<Expression<Func<HistoricalContext, bool>>?>(),
            It.IsAny<Func<IQueryable<HistoricalContext>, IIncludableQueryable<HistoricalContext, object>>?>()))
        .ReturnsAsync(historicalContexts);

        var historicalContextDtos = new List<HistoricalContextDTO>
        {
            new () { Id = 1, Title = "Title 1" },
            new () { Id = 1, Title = "Title 2" },
        };

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<HistoricalContextDTO>>(historicalContexts))
            .Returns(historicalContextDtos);

        var result = await _sut.Handle(new GetAllHistoricalContextQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(historicalContextDtos);
    }

    /// <summary>
    /// GetAllAsync method of HistoricalContextRepository doesn't return any historical contexts.
    /// Result is a failure in this case.
    /// Verifies that logger logs an error.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoHistoricalContextsExist()
    {
        var query = new GetAllHistoricalContextQuery();
        var errorMsg = "Cannot find any historical contexts";

        _repositoryWrapperMock.Setup(repo => repo.HistoricalContextRepository.GetAllAsync(
            It.IsAny<Expression<Func<HistoricalContext, bool>>?>(),
            It.IsAny<Func<IQueryable<HistoricalContext>, IIncludableQueryable<HistoricalContext, object>>?>()))
        .ReturnsAsync(Enumerable.Empty<HistoricalContext>);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Be(errorMsg);
        _loggerMock.Verify(
            logger => logger.LogError(query, errorMsg), Times.Once);
    }
}
