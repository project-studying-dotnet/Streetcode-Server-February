using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;

public record GetParsedTextForAdminPreviewQuery(string TextToParse)
    : IRequest<Result<string>>;
