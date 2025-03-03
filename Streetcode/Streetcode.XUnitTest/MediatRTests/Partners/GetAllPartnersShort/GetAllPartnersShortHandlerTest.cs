using System.Linq.Expressions;
using AutoMapper;
using Xunit;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetAllPartnersShort;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore.Query;

namespace Streetcode.XUnitTest.MediatRTests.Partners.GetAllPartnersShort;

public class GetAllPartnersShortHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllPartnersShortHandler _handler;

    public GetAllPartnersShortHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetAllPartnersShortHandler(
            _mockRepo.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenPartnersExist_ReturnsSuccessResult()
    {
        // Arrange
        var partners = new List<Partner> { new Partner { Title = "Test" } };
        var partnersDto = new List<PartnerShortDTO>
        {
            new PartnerShortDTO { Title = "Test" }
        };

        _mockRepo
            .Setup(r => r.PartnersRepository.GetAllAsync(
                It.IsAny<Expression<Func<Partner, bool>>>(),
                It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync(partners);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PartnerShortDTO>>(partners))
            .Returns(partnersDto);

        // Act
        var result = await _handler.Handle(
            new GetAllPartnersShortQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(partnersDto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenNoPartners_ReturnsEmptyList()
    {
        // Arrange
        var partners = new List<Partner>();
        var partnersDto = new List<PartnerShortDTO>();

        _mockRepo
            .Setup(r => r.PartnersRepository.GetAllAsync(
                It.IsAny<Expression<Func<Partner, bool>>>(),
                It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync(partners);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PartnerShortDTO>>(partners))
            .Returns(partnersDto);

        // Act
        var result = await _handler.Handle(
            new GetAllPartnersShortQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_WhenPartnersNull_ReturnsFailure()
    {
        // Arrange
        List<Partner> partners = null!;

        _mockRepo
            .Setup(r => r.PartnersRepository.GetAllAsync(
                It.IsAny<Expression<Func<Partner, bool>>>(),
                It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync(partners);

        // Act
        var result = await _handler.Handle(
            new GetAllPartnersShortQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<GetAllPartnersShortQuery>(),
                It.IsAny<string>()),
            Times.Once);
    }
}
