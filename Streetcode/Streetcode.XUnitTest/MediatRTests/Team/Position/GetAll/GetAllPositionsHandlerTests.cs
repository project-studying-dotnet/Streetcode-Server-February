using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.Position.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using PositionEntity = Streetcode.DAL.Entities.Team.Positions;

namespace Streetcode.XUnitTest.MediatRTests.Team.Position.GetAll;

public class GetAllPositionsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetAllPositionsHandler _handler;

    public GetAllPositionsHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new GetAllPositionsHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenPositionsAreNull()
    {
        _repositoryWrapperMock
            .Setup(r => r.PositionRepository.GetAllAsync(null, null))
            .ReturnsAsync((IEnumerable<PositionEntity>)null!);

        var result = await _handler.Handle(
            new GetAllPositionsQuery(),
            CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenPositionsAreNull()
    {
        _repositoryWrapperMock
            .Setup(r => r.PositionRepository.GetAllAsync(null, null))
            .ReturnsAsync((IEnumerable<PositionEntity>)null!);

        await _handler.Handle(
            new GetAllPositionsQuery(),
            CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogError(
                It.IsAny<GetAllPositionsQuery>(),
                "Cannot find any positions"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenPositionsExist()
    {
        var positions = new List<PositionEntity> { new() };

        _repositoryWrapperMock
            .Setup(r => r.PositionRepository.GetAllAsync(null, null))
            .ReturnsAsync(positions);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PositionDTO>>(positions))
            .Returns(new List<PositionDTO> { new() });

        var result = await _handler.Handle(
            new GetAllPositionsQuery(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
