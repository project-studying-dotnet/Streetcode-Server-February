using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Partners.Create;

namespace Streetcode.BLL.MediatR.Partners.Create;

public record CreatePartnerCommand(CreatePartnerDTO NewPartner)
    : IRequest<Result<PartnerDTO>>;
