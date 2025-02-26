/*
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests
{
    public class GetByIDAudioHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetAudioByIdHandler _handler;

        public GetByIDAudioHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetAudioByIdHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAudioIsFound()
        {
            // Arrange
            var query = new GetAudioByIdQuery(1);

            var audioEntity = new DAL.Entities.Media.Audio { Id = 1, BlobName = "audio123.mp3" };
            var expectedAudioDto = new AudioDTO { Id = 1, BlobName = "audio123.mp3", Base64 = "base64string" };

            _repositoryWrapperMock
                .Setup(rw => rw.AudioRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<DAL.Entities.Media.Audio, bool>>>(),
                    null))
                .ReturnsAsync(audioEntity);

            _mapperMock
                .Setup(m => m.Map<AudioDTO>(audioEntity))
                .Returns(expectedAudioDto);

            _blobServiceMock
                .Setup(bs => bs.FindFileInStorageAsBase64("audio123.mp3"))
                .Returns("base64string");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedAudioDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenAudioNotFound()
        {
            // Arrange
            var query = new GetAudioByIdQuery(1);

            _repositoryWrapperMock
                .Setup(rw => rw.AudioRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<DAL.Entities.Media.Audio, bool>>>(),
                    null))
                .ReturnsAsync((DAL.Entities.Media.Audio)null);

            _loggerMock
                .Setup(l => l.LogError(query, It.IsAny<string>()));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle()
                .Which.Message.Should().Be("Cannot find an audio with corresponding id: 1");

            _loggerMock.Verify(l => l.LogError(query, "Cannot find an audio with corresponding id: 1"), Times.Once);
        }
    }
}
*/