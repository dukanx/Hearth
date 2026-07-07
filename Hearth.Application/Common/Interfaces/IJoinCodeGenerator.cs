namespace Hearth.Application.Common.Interfaces;

public interface IJoinCodeGenerator
{
    /// <summary>Nasumičan kod od 6 znakova (velika slova + cifre, bez dvosmislenih).</summary>
    string Generate();
}
