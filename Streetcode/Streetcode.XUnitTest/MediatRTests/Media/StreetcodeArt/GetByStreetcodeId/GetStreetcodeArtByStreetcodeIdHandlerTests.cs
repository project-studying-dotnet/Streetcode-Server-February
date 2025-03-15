using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.StreetcodeArt.GetByStreetcodeId;
using Streetcode.DAL.Repositories.Interfaces.Base;
using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;
using StreetcodeArtEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeArt;

namespace Streetcode.XUnitTest.MediatRTests.Media
    .StreetcodeArt.GetByStreetcodeId;

public class GetStreetcodeArtByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetStreetcodeArtByStreetcodeIdHandler _handler;

    public GetStreetcodeArtByStreetcodeIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new GetStreetcodeArtByStreetcodeIdHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _blobServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenNoArtFound()
    {
        var query = new GetStreetcodeArtByStreetcodeIdQuery(1);
        _repositoryWrapperMock.Setup(r =>
            r.StreetcodeArtRepository.GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeArtEntity>,
                IIncludableQueryable<StreetcodeArtEntity, object>>>()))
            .ReturnsAsync((List<StreetcodeArtEntity>)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenNoArtFound()
    {
        var query = new GetStreetcodeArtByStreetcodeIdQuery(1);
        _repositoryWrapperMock.Setup(r =>
            r.StreetcodeArtRepository.GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeArtEntity>,
                IIncludableQueryable<StreetcodeArtEntity, object>>>()))
            .ReturnsAsync((List<StreetcodeArtEntity>)null!);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogError(
                query, It.Is<string>(
                    s => s.Contains("Cannot find an art"))), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenArtExists()
    {
        var query = new GetStreetcodeArtByStreetcodeIdQuery(1);
        var arts = new List<StreetcodeArtEntity>
        { new() { Art = new ArtEntity() } };
        _repositoryWrapperMock.Setup(r =>
            r.StreetcodeArtRepository.GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeArtEntity>,
                IIncludableQueryable<StreetcodeArtEntity, object>>>()))
            .ReturnsAsync(arts);
        _mapperMock.Setup(m => m.Map<IEnumerable<StreetcodeArtDTO>>(arts))
            .Returns(new List<StreetcodeArtDTO>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
