using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using SubtitleEntity = Streetcode.DAL.Entities.AdditionalContent;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Subtitle.GetAll;

public class GetAllSubtitlesHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetAllSubtitlesHandler _handler;

    public GetAllSubtitlesHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new GetAllSubtitlesHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSubtitles_WhenSubtitlesExist()
    {
        var subtitles = new List<SubtitleEntity.Subtitle> { new(), new() };

        _repositoryWrapperMock.Setup(r => r.SubtitleRepository
            .GetAllAsync(null, null))
            .ReturnsAsync(subtitles);

        var query = new GetAllSubtitlesQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedSubtitles_WhenSubtitlesExist()
    {
        var subtitles = new List<SubtitleEntity.Subtitle> { new(), new() };
        var subtitleDTOs = new List<SubtitleDTO> { new(), new() };

        _repositoryWrapperMock.Setup(r => r.SubtitleRepository
            .GetAllAsync(null, null))
            .ReturnsAsync(subtitles);
        _mapperMock.Setup(m => m.Map<IEnumerable<SubtitleDTO>>(subtitles))
            .Returns(subtitleDTOs);

        var query = new GetAllSubtitlesQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(subtitleDTOs, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSubtitlesAreNull()
    {
        _repositoryWrapperMock.Setup(r => r.SubtitleRepository
            .GetAllAsync(null, null))
            .ReturnsAsync((IEnumerable<SubtitleEntity.Subtitle>)null!);

        var query = new GetAllSubtitlesQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenSubtitlesAreNull()
    {
        _repositoryWrapperMock.Setup(r => r.SubtitleRepository
            .GetAllAsync(null, null))
            .ReturnsAsync((IEnumerable<SubtitleEntity.Subtitle>)null!);

        var query = new GetAllSubtitlesQuery();

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogError(query, "Cannot find any subtitles"), Times.Once);
    }
}
