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
            Price = (double)p.Price.Value,
            Quantity = p.Quantity.Value,
            SellerId = p.SellerId.ToString()
        }));

        return response;
    }

    public override async Task<GetProductBySkuIdResponse> GetProductBySkuId(
        GetProductBySkuIdRequest request, 
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.SkuId, out var skuId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid SKU ID format."));
        }

        var product = await _productRepository.GetBySkuIdAsync(skuId);
        
        if (product is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Product variant with specified SKU ID not found."));
        }

        var variant = product.Variants.First(v => v.SkuId == skuId);

        return new GetProductBySkuIdResponse
        {
            ProductId = product.Id.ToString(),
            SkuId = variant.SkuId.ToString(),
            Title = $"{product.Title} ({string.Join(", ", variant.Attributes.Values)})",
            Price = (double)variant.Price.Value,
            Quantity = variant.Quantity.Value,
            SellerId = product.SellerId.ToString()
        };
    }
}