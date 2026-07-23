using HybridShop.Grpc;
using HybridShop.Services.Chat.Core.Interfaces;

namespace HybridShop.Services.Chat.Infrastructure.Clients;

public class UserGrpcClient : IUserGrpcClient
{
    private readonly UserGrpcService.UserGrpcServiceClient _client;

    public UserGrpcClient(UserGrpcService.UserGrpcServiceClient client)
    {
        _client = client;
    }

    public async Task<string> GetUserFullNameAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetUserDetailsAsync(
                new UserRequest { Id = userId.ToString() },
                cancellationToken: cancellationToken
            );

            return $"{response.Name} {response.Lastname}".Trim();
        }
        catch
        {
            return "Użytkownik";
        }
    }
}