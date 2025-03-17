using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext.Delete;

public record DeleteHistoryEventCommand(int Id)
    : IRequest<Result<Unit>>;