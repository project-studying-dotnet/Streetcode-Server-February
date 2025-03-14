using System.Linq.Expressions;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Streetcode;
using MediatR;
using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Fact.Create;
public class CreateFactHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreateFactHandler _handler;

    public CreateFactHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new CreateFactHandler(
            _mapperMock.Object,
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenFactIsSuccessfullyCreated()
    {
        // Arrange
        var factDto = new FactUpdateCreateDTO
        {
            Title = "Fact Title",
            FactContent = "Fact Content",
            StreetcodeId = 1
        };

        var factEntity = new FactEntity();

        var streetcodeEntity = new StreetcodeContent { Id = 1 };

        _mapperMock
            .Setup(m => m.Map<FactEntity>(factDto))
            .Returns(factEntity);

        _repositoryMock
            .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync(streetcodeEntity);

        _repositoryMock
            .Setup(r => r.FactRepository.CreateAsync(factEntity))
            .ReturnsAsync(factEntity);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(
            new CreateFactCommand(factDto),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenStreetcodeNotFound()
    {
        // Arrange
        var factDto = new FactUpdateCreateDTO
        {
            Title = "Fact Title",
            FactContent = "Fact Content",
            StreetcodeId = 1
        };

        var factEntity = new FactEntity();

        StreetcodeContent? streetcodeEntity = null;

        _mapperMock
            .Setup(m => m.Map<FactEntity>(factDto))
            .Returns(factEntity);

        _repositoryMock
            .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync(streetcodeEntity);

        var command = new CreateFactCommand(factDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(
            e => e.Message == "No existing streetcode with the id");

        _loggerMock.Verify(
            l => l.LogError(command, "No existing streetcode with the id"),
            Times.Once);
    }
}
