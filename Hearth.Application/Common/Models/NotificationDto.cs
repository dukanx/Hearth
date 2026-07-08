namespace Hearth.Application.Common.Models;

public sealed record NotificationDto(
    Guid Id,
    string Message,
    bool IsRead,
    DateTime CreatedAt);
