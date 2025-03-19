using AutoMapper;
using Moq;
using Xunit;
using FluentAssertions;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.Position.Create;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Team;

namespace Streetcode.XUnitTest.MediatRTests.Team.Position.Create;
public class CreatePositionHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreatePositionHandler _handler;

    public CreatePositionHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new CreatePositionHandler(
            _mapperMock.Object, _repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenPositionIsCreated()
    {
        var request = new CreatePositionCommand(
            new PositionDTO { Position = "Test" });
        var position = new Positions { Position = "Test" };

        _repositoryMock.Setup(
                r => r.PositionRepository.CreateAsync(It.IsAny<Positions>()))
            .ReturnsAsync(position);
        _mapperMock.Setup(m => m.Map<PositionDTO>(It.IsAny<Positions>()))
            .Returns(request.Position);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsync_WhenPositionIsCreated()
    {
        var request = new CreatePositionCommand(
            new PositionDTO { Position = "Test" });
        var position = new Positions { Position = "Test" };

        _repositoryMock.Setup(
            r => r.PositionRepository.CreateAsync(It.IsAny<Positions>()))
            .ReturnsAsync(position);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(request, CancellationToken.None);

        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionOccurs()
    {
        var request = new CreatePositionCommand(
            new PositionDTO { Position = "Test" });

        _repositoryMock.Setup(r => r.PositionRepository)
            .Returns(Mock.Of<IPositionRepository>());

        _repositoryMock.Setup(r => r.SaveChangesAsync())
            .ThrowsAsync(new Exception("Database error"));

        await _handler.Handle(request, CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogError(request, "Database error"), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenExceptionOccurs()
    {
        var request = new CreatePositionCommand(
            new PositionDTO { Position = "Test" });

        _repositoryMock.Setup(r => r.PositionRepository)
        .Returns(Mock.Of<IPositionRepository>());

        _repositoryMock.Setup(r => r.SaveChangesAsync())
            .ThrowsAsync(new Exception("Database error"));

        _loggerMock.Setup(
            l => l.LogError(It.IsAny<object>(), It.IsAny<string>()));

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}
