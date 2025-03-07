using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Tag;

namespace Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByTitle;

public record GetTagByTitleQuery(string Title)
    : IRequest<Result<TagDTO>>;
