using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TeamLink = Streetcode.DAL.Entities.Team.TeamMemberLink;

namespace Streetcode.XUnitTest.MediatRTests.Team.TeamMembersLinks.GetAll;

public class GetAllTeamMemberLinksHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetAllTeamMemberLinksHandler _handler;

    public GetAllTeamMemberLinksHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new GetAllTeamMemberLinksHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoTeamLinks()
    {
        _repositoryWrapperMock.Setup(rw => rw.TeamLinkRepository
            .GetAllAsync(null, null))
            .ReturnsAsync((IEnumerable<TeamLink>)null!);

        var result = await _handler.Handle(
            new GetAllTeamMemberLinksQuery(), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenNoTeamLinks()
    {
        _repositoryWrapperMock.Setup(rw => rw.TeamLinkRepository
            .GetAllAsync(null, null))
            .ReturnsAsync((IEnumerable<TeamLink>)null!);

        await _handler.Handle(
            new GetAllTeamMemberLinksQuery(), CancellationToken.None);

        _loggerMock.Verify(
            logger => logger.LogError(
                It.IsAny<GetAllTeamMemberLinksQuery>(),
                It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTeamLinksExist()
    {
        var teamLinks = new List<TeamLink> { new() };
        _repositoryWrapperMock.Setup(rw => rw.TeamLinkRepository
            .GetAllAsync(null, null)).ReturnsAsync(teamLinks);
        _mapperMock.Setup(m => m.Map<IEnumerable<TeamMemberLinkDTO>>(
            teamLinks)).Returns(new List<TeamMemberLinkDTO>());

        var result = await _handler.Handle(
            new GetAllTeamMemberLinksQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
