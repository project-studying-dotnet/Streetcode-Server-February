using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Fact.GetByStreetcodeId;

public class GetFactByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetFactByStreetcodeIdHandler _handler;

    public GetFactByStreetcodeIdHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetFactByStreetcodeIdHandler(
            _mockRepo.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenFactsExistForStreetcode_ReturnsSuccessResult()
    {
        // Arrange
        var streetcodeId = 1;
        var facts = new List<FactEntity>
        {
            new FactEntity { Id = 1, Title = "Fact 1", StreetcodeId = streetcodeId },
            new FactEntity { Id = 2, Title = "Fact 2", StreetcodeId = streetcodeId }
        };

        var factsDto = new List<FactDTO>
        {
            new FactDTO { Id = 1, Title = "Fact 1" },
            new FactDTO { Id = 2, Title = "Fact 2" }
        };

        _mockRepo
            .Setup(r => r.FactRepository.GetAllAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FactEntity>,
                    IIncludableQueryable<FactEntity, object>>>()))
            .ReturnsAsync(facts);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<FactDTO>>(facts))
            .Returns(factsDto);

        // Act
        var result = await _handler.Handle(
            new GetFactByStreetcodeIdQuery(streetcodeId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(factsDto);
    }

    [Fact]
    public async Task Handle_WhenNoFactsExistForStreetcode_ReturnsEmptyList()
    {
        // Arrange
        var streetcodeId = 999;
        var facts = new List<FactEntity>();
        var factsDto = new List<FactDTO>();

        _mockRepo
            .Setup(r => r.FactRepository.GetAllAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FactEntity>,
                    IIncludableQueryable<FactEntity, object>>>()))
            .ReturnsAsync(facts);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<FactDTO>>(facts))
            .Returns(factsDto);

        // Act
        var result = await _handler.Handle(
            new GetFactByStreetcodeIdQuery(streetcodeId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNull_ReturnsFailure()
    {
        // Arrange
        var streetcodeId = 1;
        IEnumerable<FactEntity>? facts = null;

        _mockRepo
            .Setup(r => r.FactRepository.GetAllAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FactEntity>,
                    IIncludableQueryable<FactEntity, object>>>()))
            .ReturnsAsync(facts);

        // Act
        var result = await _handler.Handle(
            new GetFactByStreetcodeIdQuery(streetcodeId),
            CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        var expectedMsg = $"Cannot find any fact by the streetcode id: {streetcodeId}";
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<GetFactByStreetcodeIdQuery>(),
                expectedMsg),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ReturnsFailure()
    {
        // Arrange
        var streetcodeId = 1;
        var exceptionMessage = "Test exception";

        _mockRepo
            .Setup(r => r.FactRepository.GetAllAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FactEntity>,
                    IIncludableQueryable<FactEntity, object>>>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(
            new GetFactByStreetcodeIdQuery(streetcodeId),
            CancellationToken.None));
    }
}
