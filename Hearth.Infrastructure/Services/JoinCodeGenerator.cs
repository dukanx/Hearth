using System.Security.Cryptography;
using Hearth.Application.Common.Interfaces;

namespace Hearth.Infrastructure.Services;

public sealed class JoinCodeGenerator : IJoinCodeGenerator
{
    // Bez dvosmislenih znakova: nema 0/O, 1/I/L.
    private const string Alphabet = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";
    private const int Length = 6;

    public string Generate()
    {
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];

        return new string(chars);
    }
}
