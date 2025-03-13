using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;

public class CreateRelatedTermCommandValidator : AbstractValidator<CreateRelatedTermCommand>
{
    public CreateRelatedTermCommandValidator()
    {
        RuleFor(x => x.RelatedTerm.Word)
            .NotEmpty().WithMessage(ValidatorMessages.WordIsRequired)
            .MaximumLength(ValidatorConstants.RelatedTermWordMaxLength).WithFormatedMessage(ValidatorMessages.RelatedTermWordMaxLength, ValidatorConstants.RelatedTermWordMaxLength);
    }
}
