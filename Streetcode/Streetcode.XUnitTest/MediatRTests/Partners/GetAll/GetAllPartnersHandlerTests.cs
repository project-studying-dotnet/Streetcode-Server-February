using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Moq;
using AutoMapper;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetAll;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.Partners.GetAll;

public class GetAllPartnersHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetAllPartnersHandler _handler;

    public GetAllPartnersHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetAllPartnersHandler(
            _mockRepo.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenPartnersExist_ReturnsSuccessResult()
    {
        // Arrange
        var partners = new List<Partner>
        {
            new Partner { Id = 1, Title = "Test Partner" }
        };

        var partnerDtos = new List<PartnerDTO>
        {
            new PartnerDTO
            {
                Id = 1,
                Title = "Test Partner",
                Description = null,
                IsKeyPartner = false,
                IsVisibleEverywhere = false,
                LogoId = 0
            }
        };

        _mockRepo
            .Setup(r => r.PartnersRepository.GetAllAsync(null, null))
            .ReturnsAsync(partners);

        _mockMapper
            .Setup(m => m.Map<IEnumerable<PartnerDTO>>(
                It.IsAny<IEnumerable<Partner>>()))
            .Returns(partnerDtos);

        // Act
        var result = await _handler.Handle(
            new GetAllPartnersQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(partnerDtos, result.Value);
    }

    [Fact]
    public async Task Handle_WhenPartnersNotFound_ReturnsFailure()
    {
        // Arrange
        _mockRepo
            .Setup(r => r.PartnersRepository.GetAllAsync(
                null,
                It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync((List<Partner>?)null);

        // Act
        var result = await _handler.Handle(
            new GetAllPartnersQuery(),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<GetAllPartnersQuery>(),
                It.IsAny<string>()),
            Times.Once);
    }
}
