using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Fact.GetById;

public class GetFactByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetFactByIdHandler _handler;

    public GetFactByIdHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetFactByIdHandler(
            _mockRepo.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenFactExists_ReturnsSuccessResult()
    {
        // Arrange
        var factId = 1;
        var fact = new FactEntity { Id = factId, Title = "Test Fact" };
        var factDto = new FactDTO { Id = factId, Title = "Test Fact" };

        _mockRepo
            .Setup(r => r.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                null))
            .ReturnsAsync(fact);

        _mockMapper
            .Setup(m => m.Map<FactDTO>(fact))
            .Returns(factDto);

        // Act
        var result = await _handler.Handle(
            new GetFactByIdQuery(factId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(factDto);
    }

    [Fact]
    public async Task Handle_WhenFactDoesNotExist_ReturnsFailure()
    {
        // Arrange
        var factId = 999;
        FactEntity? fact = null;

        _mockRepo
            .Setup(r => r.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                null))
            .ReturnsAsync(fact);

        // Act
        var result = await _handler.Handle(
            new GetFactByIdQuery(factId),
            CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        var expectedMsg = $"Cannot find any fact with corresponding id: {factId}";
        result.Errors.Should().Contain(e => e.Message == expectedMsg);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<GetFactByIdQuery>(),
                expectedMsg),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ReturnsFailure()
    {
        // Arrange
        var factId = 1;
        var exceptionMessage = "Test exception";

        _mockRepo
            .Setup(r => r.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                null))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(
            new GetFactByIdQuery(factId),
            CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectId()
    {
        // Arrange
        var factId = 1;
        var fact = new FactEntity { Id = factId, Title = "Test Fact" };

        _mockRepo
            .Setup(r => r.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                null))
            .ReturnsAsync(fact);

        _mockMapper
            .Setup(m => m.Map<FactDTO>(fact))
            .Returns(new FactDTO { Id = factId, Title = "Test Fact" });

        // Act
        await _handler.Handle(
            new GetFactByIdQuery(factId),
            CancellationToken.None);

        // Assert
        _mockRepo.Verify(
            r => r.FactRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                null),
            Times.Once);
    }
}
