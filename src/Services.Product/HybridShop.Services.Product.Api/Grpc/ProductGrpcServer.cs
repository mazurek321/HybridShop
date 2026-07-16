using Grpc.Core;
using HybridShop.Services.Product.Core.Interfaces;
using HybridShop.Services.Product.Grpc;

namespace HybridShop.Services.Product.Api.Grpc;

public class ProductGrpcServer : ProductGrpcService.ProductGrpcServiceBase
{
    private readonly IProductRepository _productRepository;

    public ProductGrpcServer(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public override async Task<GetProductsByIdsResponse> GetProductsByIds(
        GetProductsByIdsRequest request, 
        ServerCallContext context)
    {
        var stringIds = request.ProductIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();

        var guids = stringIds
            .Select(id => Guid.TryParse(id, out var g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty)
            .ToList();

        var products = new List<Core.Product.Product>();

        foreach (var id in guids)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is not null)
            {
                products.Add(product);
            }
        }

        var response = new GetProductsByIdsResponse();
        
        response.Products.AddRange(products.Select(p => new ProductGrpcModel
        {
            Id = p.Id.ToString(),
            Title = p.Title,
            Price = (double)p.Price.Value 
        }));

        return response;
    }
}