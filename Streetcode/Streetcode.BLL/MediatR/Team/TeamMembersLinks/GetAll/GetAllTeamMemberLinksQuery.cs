using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Team;

namespace Streetcode.BLL.MediatR.Team.TeamMembersLinks.GetAll
{
    public record GetAllTeamMemberLinksQuery : IRequest<Result<IEnumerable<TeamMemberLinkDTO>>>;
}
