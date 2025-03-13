using System.Linq.Expressions;
using Moq;
using Xunit;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests
    .AdditionalContent.Coordinate.Delete;

public class DeleteCoordinateHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly DeleteCoordinateHandler _handler;

    public DeleteCoordinateHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _handler = new DeleteCoordinateHandler(_repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCoordinateNotFound()
    {
        var request = new DeleteCoordinateCommand(1);
        _repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCoordinate, bool>>>(),
                null))
            .ReturnsAsync((StreetcodeCoordinate)null!);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var request = new DeleteCoordinateCommand(1);
        var coordinate = new StreetcodeCoordinate { Id = 1 };

        _repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCoordinate, bool>>>(),
                null))
            .ReturnsAsync(coordinate);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCoordinateDeleteSuccess()
    {
        var request = new DeleteCoordinateCommand(1);
        var coordinate = new StreetcodeCoordinate { Id = 1 };

        _repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCoordinate, bool>>>(),
                null))
            .ReturnsAsync(coordinate);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
