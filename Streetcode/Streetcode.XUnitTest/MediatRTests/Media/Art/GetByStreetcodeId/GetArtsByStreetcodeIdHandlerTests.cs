﻿using System.Linq.Expressions;
using Xunit;
using Moq;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;
using ImageEntity = Streetcode.DAL.Entities.Media.Images.Image;

namespace Streetcode.XUnitTest.MediatRTests.Media.Art.GetByStreetcodeId;

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
        var arts = new List<ArtEntity>
        {
            new ArtEntity { Id = 1, Image = new ImageEntity { Id = 1 } }
        };
        var artsDto = new List<ArtDTO>
        {
            new ArtDTO { Id = 1, Image = new ImageDTO { Id = 1 } }
        };

        var artRepository = new Mock<IRepositoryWrapper>();
        artRepository.Setup(x => x.ArtRepository).Returns(Mock.Of<IArtRepository>(r => r.GetAllAsync(It.IsAny<Expression<Func<ArtEntity, bool>>>(), It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>())
                == Task.FromResult((IEnumerable<ArtEntity>)arts)));

        _mockMapper.Setup(m => m.Map<IEnumerable<ArtDTO>>(It.IsAny<IEnumerable<ArtEntity>>()))
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
        artRepository.Setup(x => x.ArtRepository).Returns(Mock.Of<IArtRepository>(r => r.GetAllAsync(It.IsAny<Expression<Func<ArtEntity, bool>>>(), It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>())
                == Task.FromResult((IEnumerable<ArtEntity>?)null)));

        var handler = new GetArtsByStreetcodeIdHandler(
            artRepository.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        var result = await handler.Handle(new GetArtsByStreetcodeIdQuery(999), CancellationToken.None);
        Assert.True(result.IsFailed);
    }
}
