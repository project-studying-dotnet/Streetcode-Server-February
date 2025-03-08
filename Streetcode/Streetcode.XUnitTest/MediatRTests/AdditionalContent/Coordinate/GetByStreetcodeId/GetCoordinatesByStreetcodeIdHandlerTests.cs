using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent
    .Coordinate.GetByStreetcodeId;
public class GetCoordinatesByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly GetCoordinatesByStreetcodeIdHandler _handler;

    public GetCoordinatesByStreetcodeIdHandlerTests()
    {
        _handler = new GetCoordinatesByStreetcodeIdHandler(
            _repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock
            .Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenStreetcodeDoesNotExist()
    {
        _repositoryWrapperMock.Setup(r => r.StreetcodeRepository
        .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync((StreetcodeContent)null!);

        var query = new GetCoordinatesByStreetcodeIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorMess_WhenStreetcodeDoesNotExist()
    {
        _repositoryWrapperMock.Setup(r => r.StreetcodeRepository
        .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync((StreetcodeContent)null!);

        var query = new GetCoordinatesByStreetcodeIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Errors.First().Message.Should()
            .Contain("Cannot find a coordinates by a streetcode id");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenCoordinatesAreNull()
    {
        _repositoryWrapperMock.Setup(r => r.StreetcodeRepository
        .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync(new StreetcodeContent());

        _repositoryWrapperMock.Setup(r => r
        .StreetcodeCoordinateRepository.GetAllAsync(
            It.IsAny<Expression<Func<StreetcodeCoordinate, bool>>>(), null))
            .ReturnsAsync((IEnumerable<StreetcodeCoordinate>)null!);

        var query = new GetCoordinatesByStreetcodeIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenCoordinatesAreNull()
    {
        _repositoryWrapperMock.Setup(r => r.StreetcodeRepository
        .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync(new StreetcodeContent());

        _repositoryWrapperMock.Setup(r => r
        .StreetcodeCoordinateRepository.GetAllAsync(
            It.IsAny<Expression<Func<StreetcodeCoordinate, bool>>>(), null))
            .ReturnsAsync((IEnumerable<StreetcodeCoordinate>)null!);

        var query = new GetCoordinatesByStreetcodeIdQuery(1);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock
            .Verify(l => l.LogError(query, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedCoordinate_WhenCoordinateExist()
    {
        var coordinates = new List<StreetcodeCoordinate>
        { new() };
        var mappedCoordinates = new List<StreetcodeCoordinateDTO>
        { new() };

        _repositoryWrapperMock.Setup(r => r.StreetcodeRepository
        .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
            .ReturnsAsync(new StreetcodeContent());

        _repositoryWrapperMock.Setup(r => r
        .StreetcodeCoordinateRepository.GetAllAsync(
            It.IsAny<Expression<Func<StreetcodeCoordinate, bool>>>(), null))
            .ReturnsAsync(coordinates);

        _mapperMock.Setup(m => m.Map<IEnumerable<StreetcodeCoordinateDTO>>(
            coordinates)).Returns(mappedCoordinates);

        var query = new GetCoordinatesByStreetcodeIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Value.Should().BeEquivalentTo(mappedCoordinates);
    }
}
