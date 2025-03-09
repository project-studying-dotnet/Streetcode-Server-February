using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetByStreetcodeId;
using Streetcode.DAL.Repositories.Interfaces.Base;
using SubtitleEntity = Streetcode.DAL.Entities.AdditionalContent.Subtitle;

namespace Streetcode.XUnitTest.MediatRTests
    .AdditionalContent.Subtitle.GetByStreetcodeId;

public class GetSubtitlesByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetSubtitlesByStreetcodeIdHandler _handler;

    public GetSubtitlesByStreetcodeIdHandlerTests()
    {
        _repositoryMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new GetSubtitlesByStreetcodeIdHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNullResult_WhenSubtitleNotFound()
    {
        _repositoryMock.Setup(r => r.SubtitleRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<SubtitleEntity, bool>>>(), null))
            .ReturnsAsync((SubtitleEntity)null!);

        var query = new GetSubtitlesByStreetcodeIdQuery(1);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedSubtitle_WhenSubtitleExists()
    {
        var subtitle = new SubtitleEntity { StreetcodeId = 1 };
        var subtitleDto = new SubtitleDTO();

        _repositoryMock.Setup(r => r.SubtitleRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<SubtitleEntity, bool>>>(), null))
            .ReturnsAsync(subtitle);

        _mapperMock.Setup(m => m.Map<SubtitleDTO>(subtitle))
            .Returns(subtitleDto);

        var query = new GetSubtitlesByStreetcodeIdQuery(1);
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(subtitleDto, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldLogWarning_WhenSubtitleNotFound()
    {
        _repositoryMock.Setup(r => r.SubtitleRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<SubtitleEntity, bool>>>(), null))
            .ReturnsAsync((SubtitleEntity)null!);

        var query = new GetSubtitlesByStreetcodeIdQuery(1);
        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogWarning(It.IsAny<string>()), Times.Once);
    }
}
