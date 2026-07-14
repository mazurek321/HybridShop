namespace HybridShop.Services.Product.Core.Product;

public class Product
{
    public Product()
    {
        Price = null!;
        Quantity = null!;
        Status = null!;
    }
    private Product(
        Guid id,
        string title,
        string slug,
        string? description,
        Price price,
        Quantity quantity,
        ProductStatus status,
        Guid sellerId,
        DateTime createdAt,
        bool isDeleted
    )
    {
        Id = id;
        Title = title;
        Slug = slug;
        Description = description;
        Price = price;
        Quantity = quantity;
        Status = status;
        SellerId = sellerId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        IsDeleted = isDeleted;

    }
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public Price Price { get; private set; }
    public Quantity Quantity { get; private set; }
    public ProductStatus Status { get; private set; }
    public Guid SellerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }


    public static Product NewProduct(
        string title,
        string slug,
        string? description,
        Price price,
        Quantity quantity,
        Guid sellerId
    )
    {
        return new Product(
            Guid.NewGuid(), title, slug, description, price, quantity, ProductStatus.Active(), sellerId, DateTime.UtcNow, false
        );
    }

    public void Update(
        string title,
        string slug,
        string? description,
        Price price,
        Quantity quantity
    )
    {
        Title = title;
        Slug = slug;
        Description = description;
        Price = price;
        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;   
    }
}