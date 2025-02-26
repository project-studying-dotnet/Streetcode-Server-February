using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Tag;

namespace Streetcode.BLL.MediatR.AdditionalContent.Tag.GetAll;

public record GetAllTagsQuery
    : IRequest<Result<IEnumerable<TagDTO>>>;
