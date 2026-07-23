namespace HybridShop.Services.Chat.Core.Interfaces;

public interface IUserGrpcClient
{
    Task<string> GetUserFullNameAsync(Guid userId, CancellationToken cancellationToken = default);
}