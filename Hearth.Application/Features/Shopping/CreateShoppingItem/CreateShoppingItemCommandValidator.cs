using FluentValidation;

namespace Hearth.Application.Features.Shopping.CreateShoppingItem;

public sealed class CreateShoppingItemCommandValidator : AbstractValidator<CreateShoppingItemCommand>
{
    public CreateShoppingItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
