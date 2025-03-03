using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Update;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Streetcode.XUnitTest.MediatRTests.Partners.Update;

public class UpdatePartnerHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly UpdatePartnerHandler _handler;

    public UpdatePartnerHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new UpdatePartnerHandler(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenUpdateSuccessful_ReturnsSuccessResult()
    {
        // Arrange
        var partnerDto = new CreatePartnerDTO
        {
            Id = 1,
            Title = "Test",
            Streetcodes = new List<StreetcodeShortDTO> { new() { Id = 1 } },
            PartnerSourceLinks = new List<CreatePartnerSourceLinkDTO>()
        };

        var partner = new Partner
        {
            Id = 1,
            Title = "Test",
            PartnerSourceLinks = new List<PartnerSourceLink>(),
            Streetcodes = new List<StreetcodeContent>()
        };

        var oldLinks = new List<PartnerSourceLink>();
        var oldStreetcodes = new List<StreetcodePartner>();

        _mockMapper.Setup(m => m.Map<Partner>(partnerDto))
            .Returns(partner);
        _mockMapper.Setup(m => m.Map<PartnerDTO>(partner))
            .Returns(new PartnerDTO { Id = 1, Title = "Test" });

        _mockRepo.Setup(r => r.PartnerSourceLinkRepository.GetAllAsync(
            It.IsAny<Expression<Func<PartnerSourceLink, bool>>>(),
            It.IsAny<Func<IQueryable<PartnerSourceLink>, IIncludableQueryable<PartnerSourceLink, object>>>()))
            .ReturnsAsync(oldLinks);

        _mockRepo.Setup(r => r.PartnerStreetcodeRepository.GetAllAsync(
            It.IsAny<Expression<Func<StreetcodePartner, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodePartner>, IIncludableQueryable<StreetcodePartner, object>>>()))
            .ReturnsAsync(oldStreetcodes);

        _mockRepo.Setup(r => r.PartnersRepository.Update(It.IsAny<Partner>()));
        _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(new UpdatePartnerCommand(partnerDto), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ReturnsFailure()
    {
        // Arrange
        var partnerDto = new CreatePartnerDTO { Id = 1, Title = "Test" };
        var partner = new Partner { Id = 1, Title = "Test" };

        _mockMapper.Setup(m => m.Map<Partner>(partnerDto))
            .Returns(partner);

        _mockRepo.Setup(r => r.PartnerSourceLinkRepository.GetAllAsync(
            It.IsAny<Expression<Func<PartnerSourceLink, bool>>>(),
            It.IsAny<Func<IQueryable<PartnerSourceLink>, IIncludableQueryable<PartnerSourceLink, object>>>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _handler.Handle(new UpdatePartnerCommand(partnerDto), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<UpdatePartnerCommand>(),
                It.IsAny<string>()),
            Times.Once);
    }
}
