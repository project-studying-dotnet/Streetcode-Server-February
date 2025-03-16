using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.GetAllMain;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.XUnitTest.MediatRTests.Team.GetAllMain;

public class GetAllMainTeamHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetAllMainTeamHandler _handler;

    public GetAllMainTeamHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();
        _handler = new GetAllMainTeamHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenTeamIsNull()
    {
        _repositoryWrapperMock
            .Setup(r => r.TeamRepository.GetAllAsync(
            It.IsAny<Expression<Func<TeamMember, bool>>>(),
            It.IsAny<Func<IQueryable<TeamMember>,
            IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync((IEnumerable<TeamMember>)null!);

        var result = await _handler.Handle(
            new GetAllMainTeamQuery(), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenTeamIsNull()
    {
        _repositoryWrapperMock
            .Setup(r => r.TeamRepository.GetAllAsync(
            It.IsAny<Expression<Func<TeamMember, bool>>>(),
            It.IsAny<Func<IQueryable<TeamMember>,
            IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync((IEnumerable<TeamMember>)null!);

        await _handler.Handle(
            new GetAllMainTeamQuery(), CancellationToken.None);

        _loggerMock.Verify(
            l => l.LogError(
                It.IsAny<GetAllMainTeamQuery>(), "Cannot find any team"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTeamExists()
    {
        var teamMembers = new List<TeamMember> { new() { IsMain = true } };

        _repositoryWrapperMock
            .Setup(r => r.TeamRepository.GetAllAsync(
            It.IsAny<Expression<Func<TeamMember, bool>>>(),
            It.IsAny<Func<IQueryable<TeamMember>,
            IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync(teamMembers);

        _mapperMock
            .Setup(
            m => m.Map<IEnumerable<TeamMemberDTO>>(
                It.IsAny<IEnumerable<TeamMember>>()))
            .Returns(new List<TeamMemberDTO>());

        var result = await _handler.Handle(
            new GetAllMainTeamQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
