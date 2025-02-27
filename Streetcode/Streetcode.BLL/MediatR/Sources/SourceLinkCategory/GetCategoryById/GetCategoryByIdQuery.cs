using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryById;

public record GetCategoryByIdQuery(int Id)
    : IRequest<Result<SourceLinkCategoryDTO>>;
