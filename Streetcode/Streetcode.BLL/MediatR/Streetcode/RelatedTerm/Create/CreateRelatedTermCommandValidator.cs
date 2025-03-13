using FluentValidation;
using Streetcode.BLL.Constants;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;

public class CreateRelatedTermCommandValidator : AbstractValidator<CreateRelatedTermCommand>
{
    public CreateRelatedTermCommandValidator()
    {
        RuleFor(x => x.RelatedTerm.Word)
            .NotEmpty().WithMessage("Word is required.")
            .MaximumLength(ValidatorsConstants.RelatedTermWordMaxLength).WithMessage($"Word must not exceed {ValidatorsConstants.RelatedTermWordMaxLength} characters.");
    }
}