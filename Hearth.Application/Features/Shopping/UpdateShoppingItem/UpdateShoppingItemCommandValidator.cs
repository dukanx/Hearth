using FluentValidation;

namespace Hearth.Application.Features.Shopping.UpdateShoppingItem;

public sealed class UpdateShoppingItemCommandValidator : AbstractValidator<UpdateShoppingItemCommand>
{
    public UpdateShoppingItemCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
