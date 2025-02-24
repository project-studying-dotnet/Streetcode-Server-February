namespace Streetcode.XUnitTest.MediatRTests.ArtTests;
using Xunit;
using Moq;
using AutoMapper;
using FluentResults;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Streetcode.DAL.Entities.Media;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Interfaces.Logging;
using Repositories.Interfaces;
using Streetcode.DAL.Entities.Media.Images;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

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
        var arts = new List<Art> { new Art { Id = 1 } };
        var artsDto = new List<ArtDTO> { new ArtDTO { Id = 1 } };

        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>()))
            .ReturnsAsync(arts);

        _mockMapper.Setup(m => m.Map<IEnumerable<ArtDTO>>(It.IsAny<IEnumerable<Art>>())).Returns(artsDto);

        var handler = new GetAllArtsHandler(artRepository.Object, _mockMapper.Object, _mockLogger.Object);
        var result = await handler.Handle(new GetAllArtsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAllArts_ReturnsFail_WhenNoArtsExist()
    {
        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository.GetAllAsync(It.IsAny<Expression<Func<Art, bool>>>(), It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>()))
            .ReturnsAsync((IEnumerable<Art>)null);

        var handler = new GetAllArtsHandler(artRepository.Object, _mockMapper.Object, _mockLogger.Object);
        var result = await handler.Handle(new GetAllArtsQuery(), CancellationToken.None);
        Assert.True(result.IsFailed);
    }
}