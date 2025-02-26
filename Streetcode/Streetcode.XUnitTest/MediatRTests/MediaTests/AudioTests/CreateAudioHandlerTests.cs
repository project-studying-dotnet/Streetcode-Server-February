/*
using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests
{
    
    public class CreateAudioHandlerTests
    {
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly CreateAudioHandler _handler;

        public CreateAudioHandlerTests()
        {
            _blobServiceMock = new Mock<IBlobService>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new CreateAudioHandler(
                _blobServiceMock.Object,
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAudioIsCreated()
        {
            var audioDTO = new AudioFileBaseCreateDTO
            {
                Title = "Test",
                BaseFormat = "mp3",
                Extension = "mp3"
            };

            var command = new CreateAudioCommand(audioDTO);

            var audioEntity = new DAL.Entities.Media.Audio();
            var savedAudioDto = new AudioDTO { Id = 1, BlobName = "hash.mp3", MimeType = "audio/mpeg" };

            _blobServiceMock
                .Setup(bs => bs.SaveFileInStorage("mp3", "Test", "mp3"))
                .Returns("hash");

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.Media.Audio>(command.Audio))
                .Returns(audioEntity);

            _repositoryWrapperMock
                .Setup(rw => rw.AudioRepository.CreateAsync(It.IsAny<DAL.Entities.Media.Audio>()))
                .ReturnsAsync((DAL.Entities.Media.Audio audio) => audio);

            _repositoryWrapperMock
                .Setup(rw => rw.SaveChangesAsync())
                .ReturnsAsync(1);

            _mapperMock
                .Setup(m => m.Map<AudioDTO>(audioEntity))
                .Returns(savedAudioDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(savedAudioDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenSaveFails()
        {
            // Arrange
            var audioDTO = new AudioFileBaseCreateDTO
            {
                Title = "Test",
                BaseFormat = "mp3",
                Extension = "mp3"
            };

            var command = new CreateAudioCommand(audioDTO);

            var audioEntity = new DAL.Entities.Media.Audio();

            _blobServiceMock
                .Setup(bs => bs.SaveFileInStorage("mp3", "Test", "mp3"))
                .Returns("hash");

            _mapperMock
                .Setup(m => m.Map<DAL.Entities.Media.Audio>(command.Audio))
                .Returns(audioEntity);

            _repositoryWrapperMock
                .Setup(rw => rw.AudioRepository.CreateAsync(It.IsAny<DAL.Entities.Media.Audio>()))
                .ReturnsAsync((DAL.Entities.Media.Audio audio) => audio);

            _repositoryWrapperMock
                .Setup(rw => rw.SaveChangesAsync())
                .ReturnsAsync(0);

            _loggerMock
                .Setup(l => l.LogError(It.IsAny<CreateAudioCommand>(), It.IsAny<string>()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.Message.Should().Be("Failed to create an audio");

            _loggerMock.Verify(l => l.LogError(command, "Failed to create an audio"), Times.Once);
        }
    }
}
*/