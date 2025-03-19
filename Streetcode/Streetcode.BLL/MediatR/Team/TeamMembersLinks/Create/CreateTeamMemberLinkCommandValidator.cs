using FluentValidation;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;

public class CreateTeamMemberLinkCommandValidator : AbstractValidator<CreateTeamMemberLinkCommand>
{
    public CreateTeamMemberLinkCommandValidator()
    {
        RuleFor(x => x.TeamMember.TargetUrl)
            .NotEmpty().WithMessage(ValidatorMessages.UrlIsRequired);
    }
}
