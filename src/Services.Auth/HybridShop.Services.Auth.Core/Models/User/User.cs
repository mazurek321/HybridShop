namespace HybridShop.Services.Auth.Core.Models;
public class User
{
    public User(){}
    private User(
        Guid id, 
        string email, 
        string passwordHash, 
        string name, 
        string lastname, 
        char gender, 
        UserRole role,
        DateOnly birthday,
        DateTime createdAt
    )
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
        Lastname = lastname;
        Gender = gender;
        Role = role;
        Birthday = birthday;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Lastname { get; private set; } = string.Empty;
    public char Gender { get; private set; }
    public UserRole Role { get; private set; }
    public DateOnly Birthday { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public static User NewUser(
        string email, 
        string passwordHash, 
        string name, 
        string lastname, 
        char gender, 
        UserRole role, 
        DateOnly birthday
    )
    {
        return new User(Guid.NewGuid(), email, passwordHash, name, lastname, gender, role, birthday, DateTime.UtcNow);
    }

    public void AddRefreshToken(string token, TimeSpan lifetime)
    {
        var refreshToken = RefreshToken.Create(token, lifetime, Id);
        _refreshTokens.Add(refreshToken);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RevokeRefreshToken(string token)
    {
        var existingToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
        if (existingToken is { IsActive: true })
        {
            existingToken.Revoke();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RevokeAllRefreshTokens()
    {
        var activeTokens = _refreshTokens.Where(t => t.IsActive);
        foreach (var token in activeTokens)
        {
            token.Revoke();
        }
        UpdatedAt = DateTime.UtcNow;
    }

}