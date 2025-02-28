using System.Linq.Expressions;
using Xunit;
using Moq;
using AutoMapper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.ArtTests;

public class GetArtByIdHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public GetArtByIdHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetArtById_ReturnsSuccess_WhenArtExists()
    {
        var art = new Art { Id = 1 };
        var artDto = new ArtDTO { Id = 1 };

        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository).Returns(Mock.Of<IArtRepository>(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Art, bool>>>(), null) == Task.FromResult(art)));

        _mockMapper.Setup(m => m.Map<ArtDTO>(It.IsAny<Art>())).Returns(artDto);

        var handler = new GetArtByIdHandler(artRepository.Object, _mockMapper.Object, _mockLogger.Object);
        var result = await handler.Handle(new GetArtByIdQuery(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetArtById_ReturnsFail_WhenArtDoesNotExist()
    {
        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository).Returns(Mock.Of<IArtRepository>(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Art, bool>>>(), null) ==
                Task.FromResult((Art)null)));

        var handler = new GetArtByIdHandler(artRepository.Object, _mockMapper.Object, _mockLogger.Object);
        var result = await handler.Handle(new GetArtByIdQuery(999), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}