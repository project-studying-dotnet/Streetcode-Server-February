using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Team;

namespace Streetcode.BLL.MediatR.Team.Position.Create;

public record CreatePositionCommand(PositionDTO Position)
    : IRequest<Result<PositionDTO>>;
