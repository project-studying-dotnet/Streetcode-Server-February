using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Team.TeamMembersLinks.Create;

public class CreateTeamMemberLinkHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreateTeamMemberLinkHandler _handler;

    public CreateTeamMemberLinkHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IRepositoryWrapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new CreateTeamMemberLinkHandler(
            _mapperMock.Object, _repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMappingFails()
    {
        _mapperMock.Setup(
            m => m.Map<DAL.Entities.Team.TeamMemberLink>(It.IsAny<object>()))
            .Returns((DAL.Entities.Team.TeamMemberLink)null!);

        var result = await _handler.Handle(
            new CreateTeamMemberLinkCommand(
                new TeamMemberLinkDTO()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenRepositoryReturnsNull()
    {
        _mapperMock.Setup(
            m => m.Map<DAL.Entities.Team.TeamMemberLink>(It.IsAny<object>()))
            .Returns(new DAL.Entities.Team.TeamMemberLink());

        _repositoryMock.Setup(
            r => r.TeamLinkRepository.Create(
                It.IsAny<DAL.Entities.Team.TeamMemberLink>()))
            .Returns((DAL.Entities.Team.TeamMemberLink)null!);

        var result = await _handler.Handle(
            new CreateTeamMemberLinkCommand(
                new TeamMemberLinkDTO()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        _mapperMock.Setup(
            m => m.Map<DAL.Entities.Team.TeamMemberLink>(It.IsAny<object>()))
            .Returns(new DAL.Entities.Team.TeamMemberLink());

        _repositoryMock.Setup(
            r => r.TeamLinkRepository.Create(
                It.IsAny<DAL.Entities.Team.TeamMemberLink>()))
            .Returns(new DAL.Entities.Team.TeamMemberLink());

        _repositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(
            new CreateTeamMemberLinkCommand(
                new TeamMemberLinkDTO()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMappingToDtoFails()
    {
        _mapperMock.Setup(
            m => m.Map<DAL.Entities.Team.TeamMemberLink>(It.IsAny<object>()))
            .Returns(new DAL.Entities.Team.TeamMemberLink());

        _repositoryMock.Setup(
            r => r.TeamLinkRepository.Create(
                It.IsAny<DAL.Entities.Team.TeamMemberLink>()))
            .Returns(new DAL.Entities.Team.TeamMemberLink());
        _repositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock.Setup(
            m => m.Map<TeamMemberLinkDTO>(
                It.IsAny<DAL.Entities.Team.TeamMemberLink>()))
            .Returns((TeamMemberLinkDTO)null!);

        var result = await _handler.Handle(
            new CreateTeamMemberLinkCommand(
                new TeamMemberLinkDTO()), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAllStepsSucceed()
    {
        _mapperMock.Setup(
            m => m.Map<DAL.Entities.Team.TeamMemberLink>(It.IsAny<object>()))
            .Returns(new DAL.Entities.Team.TeamMemberLink());

        _repositoryMock.Setup(
            r => r.TeamLinkRepository.Create(
                It.IsAny<DAL.Entities.Team.TeamMemberLink>()))
            .Returns(new DAL.Entities.Team.TeamMemberLink());

        _repositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock.Setup(
            m => m.Map<TeamMemberLinkDTO>(
                It.IsAny<DAL.Entities.Team.TeamMemberLink>()))
            .Returns(new TeamMemberLinkDTO());

        var result = await _handler.Handle(
            new CreateTeamMemberLinkCommand(
                new TeamMemberLinkDTO()), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
