namespace Hearth.Application.Common.Models;

public sealed record AuthResponse(string Token, DateTime ExpiresAt);
