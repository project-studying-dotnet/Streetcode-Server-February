using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Team;
using TeamEntity = Streetcode.DAL.Entities.Team.TeamMember;

namespace Streetcode.XUnitTest.MediatRTests.Team.GetAll;

public class GetAllTeamHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetAllTeamHandler _handler;

    public GetAllTeamHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new GetAllTeamHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenNoTeamFound()
    {
        _repositoryWrapperMock
        .Setup(r => r.TeamRepository.GetAllAsync(
        It.IsAny<Expression<Func<TeamMember, bool>>>(),
        It.IsAny<Func<IQueryable<TeamMember>,
        IIncludableQueryable<TeamMember, object>>>()))
        .ReturnsAsync((IEnumerable<TeamMember>)null!);

        var result = await _handler.Handle(
            new GetAllTeamQuery(), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenNoTeamFound()
    {
        _repositoryWrapperMock
        .Setup(r => r.TeamRepository.GetAllAsync(
        It.IsAny<Expression<Func<TeamMember, bool>>>(),
        It.IsAny<Func<IQueryable<TeamMember>,
        IIncludableQueryable<TeamMember, object>>>()))
        .ReturnsAsync((IEnumerable<TeamMember>)null!);

        await _handler.Handle(new GetAllTeamQuery(), CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogError(
                It.IsAny<GetAllTeamQuery>(), It.Is<string>(
                    msg => msg.Contains("Cannot find any team"))), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenTeamFound()
    {
        var teams = new List<TeamEntity> { new() };
        _repositoryWrapperMock
        .Setup(r => r.TeamRepository.GetAllAsync(
        It.IsAny<Expression<Func<TeamMember, bool>>>(),
        It.IsAny<Func<IQueryable<TeamEntity>,
        IIncludableQueryable<TeamMember, object>>>()))
        .ReturnsAsync(teams);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<TeamMemberDTO>>(teams))
            .Returns(new List<TeamMemberDTO>());

        var result = await _handler.Handle(
            new GetAllTeamQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldMapTeams_WhenTeamFound()
    {
        var teams = new List<TeamEntity> { new() };
        _repositoryWrapperMock
        .Setup(r => r.TeamRepository.GetAllAsync(
        It.IsAny<Expression<Func<TeamMember, bool>>>(),
        It.IsAny<Func<IQueryable<TeamMember>,
        IIncludableQueryable<TeamMember, object>>>()))
        .ReturnsAsync(teams);
        await _handler.Handle(new GetAllTeamQuery(), CancellationToken.None);

        _mapperMock.Verify(
            m => m.Map<IEnumerable<TeamMemberDTO>>(teams), Times.Once);
    }
}
