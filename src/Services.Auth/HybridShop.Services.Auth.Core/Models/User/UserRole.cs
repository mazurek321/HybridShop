namespace HybridShop.Services.Auth.Core.Models;

public enum URole { Admin, Moderator, User }

public class UserRole{

    public UserRole(URole value)
    {
        if(!Enum.IsDefined<URole>(value))
            throw new Exception("Invalid user role.");

        Value = value;
    }

    public URole Value { get; private set; }
    public static UserRole Admin() => new(URole.Admin);
    public static UserRole Moderator() => new(URole.Moderator);
    public static UserRole User() => new(URole.User);
}