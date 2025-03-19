using FluentValidation;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Partners.Delete;

public class DeletePartnerCommandValidator : AbstractValidator<DeletePartnerCommand>
{
    public DeletePartnerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(ValidatorMessages.IdMustBeGreaterThanZero);
    }
}
