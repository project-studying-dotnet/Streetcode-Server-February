using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TagEntity = Streetcode.DAL.Entities.AdditionalContent.Tag;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Tag.Create;

public class CreateTagHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreateTagHandler _handler;

    public CreateTagHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new CreateTagHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTagIsCreated()
    {
        var command = new CreateTagCommand(new CreateTagDTO
        { Title = "TestTag" });
        var tagEntity = new TagEntity { Title = "TestTag" };
        var tagDTO = new TagDTO { Title = "TestTag" };

        _repositoryWrapperMock.Setup(r => r.TagRepository.CreateAsync(
                It.IsAny<TagEntity>()))
            .ReturnsAsync(tagEntity);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<TagDTO>(tagEntity))
            .Returns(tagDTO);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedTagDTO_WhenSuccess()
    {
        var command = new CreateTagCommand(new CreateTagDTO
        { Title = "TestTag" });
        var tagEntity = new TagEntity { Title = "TestTag" };
        var tagDTO = new TagDTO { Title = "TestTag" };

        _repositoryWrapperMock.Setup(r => r.TagRepository.CreateAsync(
                It.IsAny<TagEntity>()))
            .ReturnsAsync(tagEntity);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<TagDTO>(tagEntity))
            .Returns(tagDTO);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(tagDTO, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenSaveFails()
    {
        var command = new CreateTagCommand(new CreateTagDTO
        { Title = "TestTag" });
        var tagEntity = new TagEntity { Title = "TestTag" };
        var exceptionMessage = "Database error";

        _repositoryWrapperMock.Setup(r => r.TagRepository.CreateAsync(
                It.IsAny<TagEntity>()))
            .ReturnsAsync(tagEntity);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        var result = await _handler.Handle(command, CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogError(command, It.Is<string>(
                s => s.Contains(exceptionMessage))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenSaveFails()
    {
        var command = new CreateTagCommand(new CreateTagDTO
        { Title = "TestTag" });
        var tagEntity = new TagEntity { Title = "TestTag" };
        var exceptionMessage = "Database error";

        _repositoryWrapperMock.Setup(r => r.TagRepository.CreateAsync(
                It.IsAny<TagEntity>()))
            .ReturnsAsync(tagEntity);

        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}
