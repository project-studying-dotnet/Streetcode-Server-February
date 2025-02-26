using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Team;

namespace Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;

public record CreateTeamMemberLinkCommand(TeamMemberLinkDTO TeamMember)
    : IRequest<Result<TeamMemberLinkDTO>>;
