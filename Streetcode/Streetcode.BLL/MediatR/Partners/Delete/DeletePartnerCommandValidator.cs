using FluentValidation;

namespace Streetcode.BLL.MediatR.Partners.Delete;

public class DeletePartnerCommandValidator : AbstractValidator<DeletePartnerCommand>
{
    public DeletePartnerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0 for an update.");
    }
}