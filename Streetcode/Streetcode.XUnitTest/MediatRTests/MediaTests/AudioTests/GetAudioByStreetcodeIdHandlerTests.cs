using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId;
using Streetcode.BLL.MediatR.ResultVariations;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests
{
    public class GetAudioByStreetcodeIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetAudioByStreetcodeIdQueryHandler _handler;

        public GetAudioByStreetcodeIdHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetAudioByStreetcodeIdQueryHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenStreetcodeNotFound()
        {
            // Arrange
            var query = new GetAudioByStreetcodeIdQuery(1);
            _repositoryWrapperMock
                .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync((StreetcodeContent)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            _loggerMock.Verify(logger => logger.LogError(query, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNullResult_WhenStreetcodeHasNoAudio()
        {
            // Arrange
            var query = new GetAudioByStreetcodeIdQuery(1);
            var streetcode = new StreetcodeContent { Id = 1, Audio = null };

            _repositoryWrapperMock
                .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync(streetcode);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result is NullResult<AudioDTO>);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenStreetcodeHasAudio()
        {
            // Arrange
            var query = new GetAudioByStreetcodeIdQuery(1);
            var streetcode = new StreetcodeContent
            {
                Id = 1,
                Audio = new Audio { Id = 100, BlobName = "test.mp3" }
            };
            var audioDto = new AudioDTO { BlobName = "test.mp3" };

            _repositoryWrapperMock
                .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync(streetcode);

            _mapperMock.Setup(mapper => mapper.Map<AudioDTO>(streetcode.Audio)).Returns(audioDto);
            _blobServiceMock.Setup(blob => blob.FindFileInStorageAsBase64(audioDto.BlobName)).Returns("base64encodeddata");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("base64encodeddata", result.Value.Base64);
        }
    }
}
