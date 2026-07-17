using Grpc.Net.Client;
using HybridShop.Services.Product.Grpc;
using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Core.Models.Dto;
using Microsoft.Extensions.Configuration;

namespace HybridShop.Services.Order.Infrastructure.Clients;

public class ProductServiceClient : IProductServiceClient
{
    private readonly ProductGrpcService.ProductGrpcServiceClient _client;

    public ProductServiceClient(IConfiguration configuration)
    {
        var url = configuration["InternalServices:ProductGrpcUrl"] 
            ?? throw new ArgumentNullException("Product gRPC URL is missing.");
            
        var channel = GrpcChannel.ForAddress(url);
        _client = new ProductGrpcService.ProductGrpcServiceClient(channel);
    }

    public async Task<IEnumerable<ProductExternalDto>> GetProductsByIdsAsync(IEnumerable<Guid> ids)
    {
        var request = new GetProductsByIdsRequest();
        request.ProductIds.AddRange(ids.Select(id => id.ToString()));

        var response = await _client.GetProductsByIdsAsync(request);

        return response.Products.Select(p => new ProductExternalDto
        {
            Id = Guid.Parse(p.Id),
            SellerId = Guid.TryParse(p.SellerId, out var sellerId) ? sellerId : Guid.Empty,
            Title = p.Title,
            Price = (decimal)p.Price,
            Quantity = p.Quantity
        });
    }

    public async Task<ProductExternalDto?> GetProductBySkuIdAsync(Guid skuId)
    {
        try
        {
            var request = new GetProductBySkuIdRequest { SkuId = skuId.ToString() };
            var response = await _client.GetProductBySkuIdAsync(request);

            if (response == null || string.IsNullOrEmpty(response.ProductId))
                return null;

            return new ProductExternalDto
            {
                Id = Guid.Parse(response.ProductId),
                SellerId = Guid.TryParse(response.SellerId, out var sellerId) ? sellerId : Guid.Empty,
                Title = response.Title,
                Price = (decimal)response.Price,
                Quantity = response.Quantity
            };
        }
        catch
        {
            return null;
        }
    }
}