using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;

public class UpdateRelatedTermCommandValidator : AbstractValidator<UpdateRelatedTermCommand>
{
    public UpdateRelatedTermCommandValidator()
    {
        RuleFor(x => x.RelatedTerm.Word)
            .NotEmpty().WithMessage(ValidatorMessages.WordIsRequired)
            .MaximumLength(ValidatorConstants.RelatedTermWordMaxLength).WithFormatedMessage(ValidatorMessages.RelatedTermWordMaxLength, ValidatorConstants.RelatedTermWordMaxLength);
    }
}
