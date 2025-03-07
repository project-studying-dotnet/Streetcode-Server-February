using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;
using Streetcode.DAL.Repositories.Interfaces.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
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
        // Arrange
        var streetcodeCoordinateDTO = new StreetcodeCoordinateDTO();
        var command = new CreateCoordinateCommand(streetcodeCoordinateDTO);
        _mapperMock.Setup(m => m.Map<DAL.Entities.AdditionalContent.Coordinates
            .Types.StreetcodeCoordinate>(It.IsAny<StreetcodeCoordinateDTO>()))
            .Returns((DAL.Entities.AdditionalContent.Coordinates
            .Types.StreetcodeCoordinate)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldCallCreate_WhenMappingIsSuccessful()
    {
        // Arrange
        var streetcodeCoordinateDTO = new StreetcodeCoordinateDTO();
        var command = new CreateCoordinateCommand(streetcodeCoordinateDTO);
        var mappedEntity = new DAL.Entities.AdditionalContent.Coordinates
            .Types.StreetcodeCoordinate();

        _mapperMock.Setup(m => m.Map<DAL.Entities.AdditionalContent
            .Coordinates.Types.StreetcodeCoordinate>(
            It.IsAny<StreetcodeCoordinateDTO>())).Returns(mappedEntity);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryWrapperMock.Verify(
            r => r.StreetcodeCoordinateRepository
                .Create(mappedEntity), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenSaveChangeReturnPositive()
    {
        // Arrange
        var streetcodeCoordinateDTO = new StreetcodeCoordinateDTO();
        var command = new CreateCoordinateCommand(streetcodeCoordinateDTO);
        var mappedEntity = new DAL.Entities.AdditionalContent.Coordinates
            .Types.StreetcodeCoordinate();
        _mapperMock.Setup(m => m.Map<DAL.Entities.AdditionalContent.Coordinates
            .Types.StreetcodeCoordinate>(It.IsAny<StreetcodeCoordinateDTO>()))
            .Returns(mappedEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSaveChangesReturnsZero()
    {
        // Arrange
        var streetcodeCoordinateDTO = new StreetcodeCoordinateDTO();
        var command = new CreateCoordinateCommand(streetcodeCoordinateDTO);
        var mappedEntity = new DAL.Entities.AdditionalContent.Coordinates
            .Types.StreetcodeCoordinate();
        _mapperMock.Setup(m => m.Map<DAL.Entities.AdditionalContent.Coordinates
            .Types.StreetcodeCoordinate>(It.IsAny<StreetcodeCoordinateDTO>()))
            .Returns(mappedEntity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
