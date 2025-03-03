using System.Linq.Expressions;
using AutoMapper;
using Xunit;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Delete;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.Partners.Delete;

public class DeletePartnerHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly DeletePartnerHandler _handler;

    public DeletePartnerHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new DeletePartnerHandler(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenPartnerExists_ReturnsSuccessResult()
    {
        // Arrange
        Partner? partner = null!;
        var partnerId = 1;
        var partnerTitle = "testTitle";
        partner = new Partner { Id = partnerId, Title = partnerTitle };
        var partnerDto = new PartnerDTO { Id = partnerId, Title = partnerTitle };

        _mockRepo.Setup(r => r.PartnersRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Partner, bool>>>(), null))
            .ReturnsAsync(partner);

        _mockMapper.Setup(m => m.Map<PartnerDTO>(partner))
            .Returns(partnerDto);

        // Act
        var result = await _handler.Handle(new DeletePartnerCommand(partnerId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(partnerDto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenPartnerNotFound_ReturnsFailure()
    {
        // Arrange
        var partnerId = 1;

        _mockRepo.Setup(r => r.PartnersRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<Partner, bool>>>(), null))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _handler.Handle(new DeletePartnerCommand(partnerId), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }
}
