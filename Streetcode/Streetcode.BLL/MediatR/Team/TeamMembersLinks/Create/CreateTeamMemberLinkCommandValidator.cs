using FluentValidation;

namespace Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;

public class CreateTeamMemberLinkCommandValidator : AbstractValidator<CreateTeamMemberLinkCommand>
{
    public CreateTeamMemberLinkCommandValidator()
    {
        RuleFor(x => x.TeamMember.TargetUrl)
            .NotEmpty().WithMessage("URL is required.");
    }
}