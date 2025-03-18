using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetById;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.Interfaces.CacheService;

namespace Streetcode.XUnitTest.MediatRTests.Partners.GetById;

public class GetPartnerByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IRedisCacheService> _mockCacheService;
    private readonly GetPartnerByIdHandler _handler;

    public GetPartnerByIdHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _mockCacheService = new Mock<IRedisCacheService>();
        _handler = new GetPartnerByIdHandler(
            _mockRepo.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockCacheService.Object);
    }

    [Fact]
    public async Task Handle_WhenPartnerExists_ReturnsSuccessResult()
    {
        // Arrange
        var partner = new Partner { Id = 1, Title = "Test" };
        var partnerDto = new PartnerDTO { Id = 1, Title = "Test" };

        _mockRepo.Setup(r => r.PartnersRepository.GetSingleOrDefaultAsync(
            It.IsAny<Expression<Func<Partner, bool>>>(),
            It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync(partner);

        _mockMapper.Setup(m => m.Map<PartnerDTO>(partner))
            .Returns(partnerDto);

        // Act
        var result = await _handler.Handle(
            new GetPartnerByIdQuery(1),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(partnerDto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenPartnerNotFound_ReturnsFailure()
    {
        // Arrange
        Partner? partner = null!;

        _mockRepo.Setup(r => r.PartnersRepository.GetSingleOrDefaultAsync(
            It.IsAny<Expression<Func<Partner, bool>>>(),
            It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync(partner);

        // Act
        var result = await _handler.Handle(
            new GetPartnerByIdQuery(1),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<GetPartnerByIdQuery>(),
                It.IsAny<string>()),
            Times.Once);
    }
}
