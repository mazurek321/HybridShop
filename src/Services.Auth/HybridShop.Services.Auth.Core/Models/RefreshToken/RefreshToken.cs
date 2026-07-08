namespace HybridShop.Services.Auth.Core.Models;

public class RefreshToken
{
    public RefreshToken() {}

    private RefreshToken(Guid id, string token, DateTime expiresAt, Guid userId, DateTime createdAt)
    {
        Id = id;
        Token = token;
        ExpiresAt = expiresAt;
        UserId = userId;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    public static RefreshToken Create(string token, TimeSpan lifetime, Guid userId)
    {
        var now = DateTime.UtcNow;
        return new RefreshToken(Guid.NewGuid(), token, now.Add(lifetime), userId, now);
    }

    public void Revoke()
    {
        if (!IsRevoked)
        {
            RevokedAt = DateTime.UtcNow;
        }
    }
}