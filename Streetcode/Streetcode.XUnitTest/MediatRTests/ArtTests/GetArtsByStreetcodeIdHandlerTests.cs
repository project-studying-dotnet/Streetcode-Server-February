using System.Linq.Expressions;
using Xunit;
using Moq;
using AutoMapper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.DAL.Entities.Media.Images;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;

namespace Streetcode.XUnitTest.MediatRTests.ArtTests;

public class GetArtsByStreetcodeIdHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IBlobService> _mockBlobService;

    public GetArtsByStreetcodeIdHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _mockBlobService = new Mock<IBlobService>();
    }

    [Fact]
    public async Task GetArtsByStreetcodeId_ReturnsSuccess_WhenArtsExist()
    {
        var arts = new List<Art>
        {
            new Art { Id = 1, Image = new Image { Id = 1 } }
        };
        var artsDto = new List<ArtDTO>
        {
            new ArtDTO { Id = 1, Image = new ImageDTO { Id = 1 } }
        };

        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository).Returns(Mock.Of<IArtRepository>(r => r.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>())
                == Task.FromResult((IEnumerable<Art>)arts)));

        _mockMapper.Setup(m => m.Map<IEnumerable<ArtDTO>>(It.IsAny<IEnumerable<Art>>()))
            .Returns(artsDto);

        var handler = new GetArtsByStreetcodeIdHandler(
            artRepository.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        var result = await handler.Handle(new GetArtsByStreetcodeIdQuery(1), CancellationToken.None);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetArtsByStreetcodeId_ReturnsFail_WhenNoArtsExist()
    {
        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository).Returns(Mock.Of<IArtRepository>(r => r.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>())
                == Task.FromResult((IEnumerable<Art>)null)));

        var handler = new GetArtsByStreetcodeIdHandler(
            artRepository.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        var result = await handler.Handle(new GetArtsByStreetcodeIdQuery(999), CancellationToken.None);
        Assert.True(result.IsFailed);
    }
}
