namespace Hearth.Application.Common.Models;

// "Service" je host push servisa (web.push.apple.com = iPhone/iPad,
// fcm.googleapis.com = Chrome/Android...) — dovoljno da korisnik
// prepozna svoje uređaje, bez otkrivanja punog endpoint URL-a.
public sealed record PushSubscriptionDto(
    string Service,
    DateTime CreatedAt);
