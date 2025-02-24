using FluentAssertions;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.Delete;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.MediaTests.AudioTests
{
    public class DeleteAudioHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IAudioRepository> _audioRepositoryMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly DeleteAudioHandler _handler;

        public DeleteAudioHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _audioRepositoryMock = new Mock<IAudioRepository>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _repositoryWrapperMock.Setup(r => r.AudioRepository).Returns(_audioRepositoryMock.Object);
            _handler = new DeleteAudioHandler(_repositoryWrapperMock.Object, _blobServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenAudioNotFound()
        {
            // Arrange
            var command = new DeleteAudioCommand ((int)Guid.NewGuid().GetHashCode());
            _audioRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Audio, bool>>>(), null))
                .ReturnsAsync((Audio)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            _loggerMock.Verify(l => l.LogError(command, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldDeleteAudioAndReturnSuccess_WhenAudioExists()
        {
            // Arrange
            var command = new DeleteAudioCommand ((int)Guid.NewGuid().GetHashCode());
            var audio = new Audio { Id = command.Id, BlobName = "test-blob" };

            _audioRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Audio, bool>>>(), null))
                .ReturnsAsync(audio);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _audioRepositoryMock.Verify(r => r.Delete(audio), Times.Once);
            _blobServiceMock.Verify(b => b.DeleteFileInStorage(audio.BlobName), Times.Once);
            _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenDeletionFails()
        {
            // Arrange
            var command = new DeleteAudioCommand ((int)Guid.NewGuid().GetHashCode());
            var audio = new Audio { Id = command.Id, BlobName = "test-blob" };

            _audioRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Audio, bool>>>(), null))
                .ReturnsAsync(audio);
            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            _loggerMock.Verify(l => l.LogError(command, It.IsAny<string>()), Times.Once);
        }
    }
}

