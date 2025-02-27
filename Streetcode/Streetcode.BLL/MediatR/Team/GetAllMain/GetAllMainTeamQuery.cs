using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Team;

namespace Streetcode.BLL.MediatR.Team.GetAllMain;

public record GetAllMainTeamQuery
    : IRequest<Result<IEnumerable<TeamMemberDTO>>>;
