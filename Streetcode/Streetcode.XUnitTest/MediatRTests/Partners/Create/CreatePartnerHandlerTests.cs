using System;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners.Create
{
    public class CreatePartnerHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly CreatePartnerHandler _handler;

        public CreatePartnerHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
            _handler = new CreatePartnerHandler(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_WhenCreatePartnerSuccessful_ReturnsSuccessResult()
        {
            // Arrange
            var createPartnerDto = new CreatePartnerDTO
            {
                Id = 1,
                Title = "Test Partner",
                Streetcodes = new List<StreetcodeShortDTO>
                {
                    new StreetcodeShortDTO { Id = 1 }
                }
            };

            var partner = new Partner
            {
                Id = 1,
                Title = "Test Partner",
                Streetcodes = new List<StreetcodeContent>()
            };

            var streetcodes = new List<StreetcodeContent>
            {
                new StreetcodeContent { Id = 1 }
            };

            var partnerDto = new PartnerDTO { Id = 1, Title = "Test Partner" };

            _mockMapper.Setup(m => m.Map<Partner>(createPartnerDto))
                .Returns(partner);

            _mockRepo.Setup(r => r.PartnersRepository.CreateAsync(partner))
                .ReturnsAsync(partner);

            _mockRepo.Setup(r => r.StreetcodeRepository.GetAllAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    null))
                .ReturnsAsync(streetcodes);

            _mockMapper.Setup(m => m.Map<PartnerDTO>(partner))
                .Returns(partnerDto);

            // Act
            var result = await _handler.Handle(new CreatePartnerQuery(createPartnerDto), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(partnerDto, result.Value);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_WhenExceptionOccurs_ReturnsFailure()
        {
            // Arrange
            var createPartnerDto = new CreatePartnerDTO
            {
                Id = 1,
                Title = "Test Partner",
                Streetcodes = new List<StreetcodeShortDTO>()
            };

            var partner = new Partner
            {
                Id = 1,
                Title = "Test Partner",
                Streetcodes = new List<StreetcodeContent>()
            };

            var exceptionMessage = "Test exception";

            _mockMapper.Setup(m => m.Map<Partner>(createPartnerDto))
                .Returns(partner);

            _mockRepo.Setup(r => r.PartnersRepository.CreateAsync(partner))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _handler.Handle(new CreatePartnerQuery(createPartnerDto), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(exceptionMessage, result.Errors.First().Message);
            _mockLogger.Verify(
                l => l.LogError(
                    It.IsAny<CreatePartnerQuery>(),
                    It.Is<string>(s => s == exceptionMessage)),
                Times.Once);
        }
    }
}
