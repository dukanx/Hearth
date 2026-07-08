using FluentValidation;

namespace Hearth.Application.Features.Push.SubscribeToPush;

public sealed class SubscribeToPushCommandValidator : AbstractValidator<SubscribeToPushCommand>
{
    public SubscribeToPushCommandValidator()
    {
        RuleFor(c => c.Endpoint).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.P256dh).NotEmpty().MaximumLength(500);
        RuleFor(c => c.Auth).NotEmpty().MaximumLength(500);
    }
}
