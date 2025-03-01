using System.Linq.Expressions;
using Xunit;
using Moq;
using AutoMapper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.MediatRTests.Media.Art.GetById;

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
        var art = new ArtEntity { Id = 1 };
        var artDto = new ArtDTO { Id = 1 };

        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository).Returns(Mock.Of<IArtRepository>(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<ArtEntity, bool>>>(), null) == Task.FromResult(art)));

        _mockMapper.Setup(m => m.Map<ArtDTO>(It.IsAny<ArtEntity>())).Returns(artDto);

        var handler = new GetArtByIdHandler(artRepository.Object, _mockMapper.Object, _mockLogger.Object);
        var result = await handler.Handle(new GetArtByIdQuery(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetArtById_ReturnsFail_WhenArtDoesNotExist()
    {
        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository).Returns(Mock.Of<IArtRepository>(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<ArtEntity, bool>>>(), null) ==
                Task.FromResult((ArtEntity?)null)));

        var handler = new GetArtByIdHandler(artRepository.Object, _mockMapper.Object, _mockLogger.Object);
        var result = await handler.Handle(new GetArtByIdQuery(999), CancellationToken.None);

        Assert.True(result.IsFailed);
    }
}
