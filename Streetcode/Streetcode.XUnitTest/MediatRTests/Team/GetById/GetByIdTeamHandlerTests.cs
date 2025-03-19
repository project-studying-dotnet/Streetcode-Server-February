using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Team.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Team;

namespace Streetcode.XUnitTest.MediatRTests.Team.GetById;

public class GetByIdTeamHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetByIdTeamHandler _handler;

    public GetByIdTeamHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetByIdTeamHandler(
            _mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenTeamNotFound()
    {
        _mockRepo.Setup(r => r.TeamRepository.GetSingleOrDefaultAsync(
            It.IsAny<Expression<Func<TeamMember, bool>>>(),
            It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync((TeamMember)null!);

        var request = new GetByIdTeamQuery(99);
        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenTeamNotFound()
    {
        var request = new GetByIdTeamQuery(99);
        _mockRepo.Setup(r => r.TeamRepository.GetSingleOrDefaultAsync(
            It.IsAny<Expression<Func<TeamMember, bool>>>(),
            It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync((TeamMember)null!);

        await _handler.Handle(request, CancellationToken.None);

        _mockLogger.Verify(
            l => l.LogError(request, $"Cannot find any team with corresponding id: {99}"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTeamFound()
    {
        var team = new TeamMember { Id = 1 };
        _mockRepo.Setup(r => r.TeamRepository.GetSingleOrDefaultAsync(
            It.IsAny<Expression<Func<TeamMember, bool>>>(),
            It.IsAny<Func<IQueryable<TeamMember>, IIncludableQueryable<TeamMember, object>>>()))
            .ReturnsAsync(team);
        _mockMapper.Setup(m => m.Map<TeamMemberDTO>(team))
            .Returns(new TeamMemberDTO());

        var request = new GetByIdTeamQuery(team.Id);
        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
