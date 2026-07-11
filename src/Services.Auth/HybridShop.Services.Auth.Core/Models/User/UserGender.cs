using HybridShop.Services.Auth.Core.Exceptions;

namespace HybridShop.Services.Auth.Core.Models;

public class UserGender
{
    public enum UGender { M, F }
    private UserGender() { }

    public UserGender(UGender value)
    {
        if (!Enum.IsDefined(typeof(UGender), value))
            throw new InvalidGenderException();

        Value = value;
    }

    public UGender Value { get; private set; }

    public static UserGender FromChar(char genderChar)
    {
        var enumValue = char.ToUpper(genderChar) switch
        {
            'M' => UGender.M,
            'F' => UGender.F,
            _ => throw new InvalidGenderException()
        };

        return new UserGender(enumValue);
    }
    public override string ToString() => Value.ToString();
}