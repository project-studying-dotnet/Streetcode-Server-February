using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners.GetByStreetcodeId;

public class GetPartnersByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetPartnersByStreetcodeIdHandler _handler;

    public GetPartnersByStreetcodeIdHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetPartnersByStreetcodeIdHandler(
            _mockMapper.Object, _mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenStreetcodeAndPartnersExist_ReturnsSuccessResult()
    {
        // Arrange
        var streetcode = new StreetcodeContent { Id = 1 };
        var partners = new List<Partner> { new Partner { Id = 1, Title = "Test" } };
        var partnersDto = new List<PartnerDTO> { new PartnerDTO { Id = 1, Title = "Test" } };

        _mockRepo.Setup(r => r.StreetcodeRepository.GetSingleOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeContent>,
                IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcode);

        _mockRepo.Setup(r => r.PartnersRepository.GetAllAsync(
            It.IsAny<Expression<Func<Partner, bool>>>(),
            It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync(partners);

        _mockMapper.Setup(m => m.Map<IEnumerable<PartnerDTO>>(partners))
            .Returns(partnersDto);

        // Act
        var result = await _handler.Handle(
            new GetPartnersByStreetcodeIdQuery(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(partnersDto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenStreetcodeNotFound_ReturnsFailure()
    {
        // Arrange
        StreetcodeContent? streetcode = null;

        _mockRepo.Setup(r => r.StreetcodeRepository.GetSingleOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeContent>,
                IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcode);

        // Act
        var result = await _handler.Handle(
            new GetPartnersByStreetcodeIdQuery(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<GetPartnersByStreetcodeIdQuery>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPartnersNotFound_ReturnsFailure()
    {
        // Arrange
        var streetcode = new StreetcodeContent { Id = 1 };
        List<Partner>? partners = null;

        _mockRepo.Setup(r => r.StreetcodeRepository.GetSingleOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeContent>,
                IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcode);

        _mockRepo.Setup(r => r.PartnersRepository.GetAllAsync(
            It.IsAny<Expression<Func<Partner, bool>>>(),
            It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync(partners);

        // Act
        var result = await _handler.Handle(
            new GetPartnersByStreetcodeIdQuery(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<GetPartnersByStreetcodeIdQuery>(),
                It.IsAny<string>()),
            Times.Once);
    }
}
