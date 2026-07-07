using FluentValidation;

namespace Hearth.Application.Features.Households.JoinHousehold;

public sealed class JoinHouseholdCommandValidator : AbstractValidator<JoinHouseholdCommand>
{
    public JoinHouseholdCommandValidator()
    {
        RuleFor(x => x.JoinCode)
            .NotEmpty()
            .Length(6);
    }
}
