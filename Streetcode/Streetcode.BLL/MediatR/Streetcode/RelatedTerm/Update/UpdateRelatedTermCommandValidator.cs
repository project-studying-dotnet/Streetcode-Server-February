using FluentValidation;
using Streetcode.BLL.Constants;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;

public class UpdateRelatedTermCommandValidator : AbstractValidator<UpdateRelatedTermCommand>
{
    public UpdateRelatedTermCommandValidator()
    {
        RuleFor(x => x.RelatedTerm.Word)
            .NotEmpty().WithMessage("Word is required.")
            .MaximumLength(ValidatorsConstants.RelatedTermWordMaxLength).WithMessage($"Word must not exceed {ValidatorsConstants.RelatedTermWordMaxLength} characters.");
    }
}