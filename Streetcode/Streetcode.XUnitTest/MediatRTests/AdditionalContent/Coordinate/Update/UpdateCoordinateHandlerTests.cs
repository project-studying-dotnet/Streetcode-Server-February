using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Update;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Repositories.Interfaces.AdditionalContent;

namespace Streetcode.XUnitTest.MediatRTests
    .AdditionalContent.Coordinate.Update;

public class UpdateCoordinateHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateCoordinateHandler _handler;

    public UpdateCoordinateHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _repositoryWrapperMock.Setup(r => r.StreetcodeCoordinateRepository)
                      .Returns(Mock.Of<IStreetcodeCoordinateRepository>());
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateCoordinateHandler(
            _repositoryWrapperMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnFail_WhenStreetcodeCoordinateIsNull()
    {
        var command = new UpdateCoordinateCommand(null!);
        _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(null))
                   .Returns((StreetcodeCoordinate)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldCall_UpdateMethodOnce()
    {
        var streetcodeCoordinate = new StreetcodeCoordinate();
        var command = new UpdateCoordinateCommand(
            new StreetcodeCoordinateDTO { StreetcodeId = 1 });

        _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(It.IsAny<object>()))
                   .Returns(streetcodeCoordinate);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryWrapperMock.Verify(
            r => r.StreetcodeCoordinateRepository
            .Update(streetcodeCoordinate), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesAsyncFails()
    {
        var streetcodeCoordinate = new StreetcodeCoordinate();
        var command = new UpdateCoordinateCommand(
            new StreetcodeCoordinateDTO { StreetcodeId = 1 });

        _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(It.IsAny<object>()))
                   .Returns(streetcodeCoordinate);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ReturnSuccess_WhenSaveChangesAsyncGreaterZero()
    {
        var streetcodeCoordinate = new StreetcodeCoordinate();
        var command = new UpdateCoordinateCommand(
            new StreetcodeCoordinateDTO { StreetcodeId = 1 });

        _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(It.IsAny<object>()))
                   .Returns(streetcodeCoordinate);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
