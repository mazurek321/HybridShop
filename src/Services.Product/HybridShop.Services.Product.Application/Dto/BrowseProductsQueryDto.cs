namespace HybridShop.Services.Product.Application.Dto;
public class BrowseProductsQueryDto
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
    public string? Category { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public string? Search { get; set; }
}