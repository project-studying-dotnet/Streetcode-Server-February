using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;
using Streetcode.DAL.Repositories.Interfaces.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Coordinate.Create;
public class CreateCoordinateHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IStreetcodeCoordinateRepository> _coordinateRepMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCoordinateHandler _handler;

    public CreateCoordinateHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _coordinateRepMock = new Mock<IStreetcodeCoordinateRepository>();
        _mapperMock = new Mock<IMapper>();

        _repositoryWrapperMock.Setup(r => r.StreetcodeCoordinateRepository)
            .Returns(_coordinateRepMock.Object);

        _mapperMock = new Mock<IMapper>();
        _handler = new CreateCoordinateHandler(
            _repositoryWrapperMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenMappingReturnsNull()
    {
        var streetcodeCoordinateDTO = new StreetcodeCoordinateDTO();
        var command = new CreateCoordinateCommand(streetcodeCoordinateDTO);
        _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(
            It.IsAny<StreetcodeCoordinateDTO>()))
            .Returns((StreetcodeCoordinate)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldCallCreate_WhenMappingIsSuccessful()
    {
        var streetcodeCoordinateDTO = new StreetcodeCoordinateDTO();
        var command = new CreateCoordinateCommand(streetcodeCoordinateDTO);
        var mappedEntity = new StreetcodeCoordinate();

        _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(
            It.IsAny<StreetcodeCoordinateDTO>())).Returns(mappedEntity);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryWrapperMock.Verify(
            r => r.StreetcodeCoordinateRepository
                .Create(mappedEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenSaveChangeReturnPositive()
    {
        var streetcodeCoordinateDTO = new StreetcodeCoordinateDTO();
        var command = new CreateCoordinateCommand(streetcodeCoordinateDTO);
        var mappedEntity = new StreetcodeCoordinate();
        _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(
            It.IsAny<StreetcodeCoordinateDTO>()))
            .Returns(mappedEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaveChangesReturnsZero()
    {
        var streetcodeCoordinateDTO = new StreetcodeCoordinateDTO();
        var command = new CreateCoordinateCommand(streetcodeCoordinateDTO);
        var mappedEntity = new StreetcodeCoordinate();
        _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(
            It.IsAny<StreetcodeCoordinateDTO>()))
            .Returns(mappedEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}
