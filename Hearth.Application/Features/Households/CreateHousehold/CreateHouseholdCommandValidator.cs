using FluentValidation;

namespace Hearth.Application.Features.Households.CreateHousehold;

public sealed class CreateHouseholdCommandValidator : AbstractValidator<CreateHouseholdCommand>
{
    public CreateHouseholdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
