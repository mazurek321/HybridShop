using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Core.Models.Dto;
using HybridShop.Services.Product.Grpc; 

namespace HybridShop.Services.Order.Infrastructure.Services;

public class ProductServiceClient : IProductServiceClient
{
    private readonly ProductGrpcService.ProductGrpcServiceClient _grpcClient;

    public ProductServiceClient(ProductGrpcService.ProductGrpcServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<List<ProductExternalDto>> GetProductsByIdsAsync(IEnumerable<Guid> productIds)
    {
        var request = new GetProductsByIdsRequest();
        request.ProductIds.AddRange(productIds.Select(id => id.ToString()));

        var response = await _grpcClient.GetProductsByIdsAsync(request);

        return response.Products.Select(p => new ProductExternalDto
        {
            Id = Guid.Parse(p.Id),
            Title = p.Title,
            Price = (decimal)p.Price
        }).ToList();
    }
}