using HybridShop.Services.Product.Application.Dto;
using HybridShop.Services.Product.Application.Services;
using HybridShop.Services.Product.Core.Dto;
using HotChocolate;

namespace HybridShop.Services.Product.Api.GraphQL;

public class Query
{
    public async Task<IEnumerable<ProductDto>> BrowseProductsAsync(
        [Service] ProductService productService,
        int skip = 0,
        int take = 10,
        string? category = null,
        decimal? priceFrom = null,
        decimal? priceTo = null,
        string? search = null,
        CancellationToken cancellationToken = default
    )
    {
        var queryDto = new BrowseProductsQueryDto
        {
            Skip = skip,
            Take = take,
            Category = category,
            PriceFrom = priceFrom,
            PriceTo = priceTo,
            Search = search
        };

        return await productService.BrowseProductsAsync(queryDto, cancellationToken);
    }

    public async Task<ProductDto?> GetProductByIdAsync(
        [Service] ProductService productService,
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return await productService.GetProductAsync(id, cancellationToken);
        }
        catch (Exception) 
        {
            return null;
        }
    }
}