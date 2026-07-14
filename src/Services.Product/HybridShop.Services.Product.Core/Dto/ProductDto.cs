namespace HybridShop.Services.Product.Core.Dto;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public SellerDto Seller { get; set; } = null!;
}

