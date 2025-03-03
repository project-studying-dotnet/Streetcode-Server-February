using System.Linq.Expressions;
using Xunit;
using Moq;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.MediatRTests.Media.Art.GetAll;

public class GetAllArtsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public GetAllArtsHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetAllArts_ReturnsSuccess_WhenArtsExist()
    {
        var arts = new List<ArtEntity> { new ArtEntity { Id = 1 } };
        var artsDto = new List<ArtDTO> { new ArtDTO { Id = 1 } };

        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<ArtEntity, bool>>>(), It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()))
            .ReturnsAsync(arts);

        _mockMapper.Setup(m => m.Map<IEnumerable<ArtDTO>>(It.IsAny<IEnumerable<ArtEntity>>())).Returns(artsDto);

        var handler = new GetAllArtsHandler(artRepository.Object, _mockMapper.Object, _mockLogger.Object);
        var result = await handler.Handle(new GetAllArtsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAllArts_ReturnsFail_WhenNoArtsExist()
    {
        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<ArtEntity, bool>>>(), It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()))
            .ReturnsAsync((IEnumerable<ArtEntity>?)null);

        var handler = new GetAllArtsHandler(artRepository.Object, _mockMapper.Object, _mockLogger.Object);
        var result = await handler.Handle(new GetAllArtsQuery(), CancellationToken.None);
        Assert.True(result.IsFailed);
    }
}
