using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using SubtitleEntity = Streetcode.DAL.Entities.AdditionalContent.Subtitle;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Subtitle.GetById;

public class GetSubtitleByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetSubtitleByIdHandler _handler;

    public GetSubtitleByIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new GetSubtitleByIdHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_SubtitleExists_ReturnsOkResult()
    {
        var subtitle = new SubtitleEntity { Id = 1 };
        var query = new GetSubtitleByIdQuery(1);

        _repositoryWrapperMock.Setup(r => r.SubtitleRepository
            .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<SubtitleEntity, bool>>>(), null))
            .ReturnsAsync(subtitle);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_SubtitleExists_ReturnsMappedSubtitle()
    {
        var subtitle = new SubtitleEntity { Id = 1 };
        var query = new GetSubtitleByIdQuery(1);
        var subtitleDTO = new SubtitleDTO();

        _repositoryWrapperMock.Setup(r => r.SubtitleRepository
            .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<SubtitleEntity, bool>>>(), null))
            .ReturnsAsync(subtitle);

        _mapperMock.Setup(m => m.Map<SubtitleDTO>(subtitle))
            .Returns(subtitleDTO);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(subtitleDTO, result.Value);
    }

    [Fact]
    public async Task Handle_SubtitleDoesNotExist_ReturnsFailResult()
    {
        var query = new GetSubtitleByIdQuery(99);

        _repositoryWrapperMock.Setup(r => r.SubtitleRepository
            .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<SubtitleEntity, bool>>>(), null))
            .ReturnsAsync((SubtitleEntity)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_SubtitleDoesNotExist_LogsError()
    {
        var query = new GetSubtitleByIdQuery(99);

        _repositoryWrapperMock.Setup(r => r.SubtitleRepository
            .GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<SubtitleEntity, bool>>>(), null))
            .ReturnsAsync((SubtitleEntity)null!);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogError(query, It.Is<string>(s => s.Contains(
                "Cannot find a subtitle"))), Times.Once);
    }
}
