using HybridShop.Services.Product.Application.Dto;
using HybridShop.Services.Product.Application.Exceptions;
using HybridShop.Services.Product.Core.Dto;
using HybridShop.Services.Product.Core.Exceptions;
using HybridShop.Services.Product.Core.Interfaces;
using HybridShop.Services.Product.Core.Product;

namespace HybridShop.Services.Product.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserServiceClient _userServiceClient;

    public ProductService(
        IProductRepository productRepository,
        IUserServiceClient userServiceClient
    )
    {
        _productRepository = productRepository;
        _userServiceClient = userServiceClient;
    }

    public async Task<Core.Product.Product> AddProductAsync(AddNewProductDto dto, Guid userId)
    {
        var slug = GenerateSlug(dto.Title);

        if (!Enum.TryParse<PCategory>(dto.Category, true, out var parsedCategory))
            throw new InvalidProductException();

        var domainPrice = new Price(dto.Price);
        var domainQuantity = new Quantity(dto.Quantity);
        var domainVariants = MapVariants(dto.Variants);

        var product = Core.Product.Product.NewProduct(
            dto.Title,
            slug,
            dto.Description,
            domainPrice,
            domainQuantity,
            userId,
            new ProductCategory(parsedCategory),
            dto.Attributes,
            domainVariants
        );

        await _productRepository.AddAsync(product);
        return product;
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
            throw new ProductNotFoundException();

        var seller = await _userServiceClient.GetSellerDetailsAsync(product.SellerId);

        return MapToDto(product, seller);
    }

    public async Task<IEnumerable<ProductDto>> BrowseProductsAsync(BrowseProductsQueryDto query)
    {
        PCategory? parsedCategory = null;
        if (!string.IsNullOrWhiteSpace(query.Category) && Enum.TryParse<PCategory>(query.Category, true, out var categoryVal))
        {
            parsedCategory = categoryVal;
        }

        var products = await _productRepository.BrowseProductsAsync(
            query.Skip, 
            query.Take, 
            parsedCategory, 
            query.PriceFrom, 
            query.PriceTo, 
            query.Search
        );

        var tasks = products.Select(async product =>
        {
            var seller = await _userServiceClient.GetSellerDetailsAsync(product.SellerId);
            return MapToDto(product, seller);
        });

        return await Task.WhenAll(tasks);
    }

    public async Task UpdateProduct(Guid productId, Guid userId, string role, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null)
            throw new ProductNotFoundException();

        if (product.SellerId != userId && role != "Admin")
            throw new DontHavePermissionsException();
        
        if (!Enum.TryParse<PCategory>(dto.Category, true, out var parsedCategory))
            throw new InvalidProductException();

        var domainPrice = new Price(dto.Price);
        var domainQuantity = new Quantity(dto.Quantity);
        var domainVariants = MapVariants(dto.Variants);
        var newSlug = GenerateSlug(dto.Title);

        product.Update(
            dto.Title,
            newSlug,
            dto.Description,
            domainPrice,
            domainQuantity,
            new ProductCategory(parsedCategory),
            dto.Attributes,
            domainVariants
        );

        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteProduct(Guid productId, Guid userId, string role)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null)
            throw new ProductNotFoundException();

        if (product.SellerId != userId && role != "Admin")
            throw new DontHavePermissionsException();

        product.Delete();
        await _productRepository.UpdateAsync(product);
    }

    private static ProductDto MapToDto(Core.Product.Product product, SellerDto? seller)
    {
        return new ProductDto
        {
            Id = product.Id,
            Title = product.Title ?? string.Empty,
            Slug = product.Slug ?? string.Empty,
            Description = product.Description ?? string.Empty,
            Price = product.Price?.Value ?? 0, 
            Quantity = product.Quantity?.Value ?? 0,
            Category = product.Category?.Value.ToString() ?? string.Empty,
            Attributes = product.Attributes ?? new Dictionary<string, object>(),
            Variants = (product.Variants ?? new List<ProductVariant>()).Select(v => new ProductVariantDto
            {
                SkuId = v.SkuId,
                Price = v.Price?.Value ?? 0,
                Quantity = v.Quantity?.Value ?? 0,
                Attributes = v.Attributes ?? new Dictionary<string, object>()
            }).ToList(),
            Seller = seller ?? new SellerDto
            {
                Id = product.SellerId,
                Email = "unknown@shop.local",
                Name = "Nieznany",
                Lastname = "Sprzedawca"
            },
            IsDeleted = product.IsDeleted,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    private static List<ProductVariant> MapVariants(List<AddProductVariantDto>? dtos)
    {
        return dtos is not null && dtos.Any()
            ? dtos.Select(v => new ProductVariant(Guid.NewGuid(), new Price(v.Price), new Quantity(v.Quantity), v.Attributes)).ToList()
            : new List<ProductVariant>();
    }

    private static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        string str = title.ToLowerInvariant().Trim();

        str = str.Replace("ą", "a").Replace("ć", "c").Replace("ę", "e")
                .Replace("ł", "l").Replace("ń", "n").Replace("ó", "o")
                .Replace("ś", "s").Replace("ź", "z").Replace("ż", "z");

        System.Text.StringBuilder sb = new();
        foreach (char c in str)
        {
            if (char.IsLetterOrDigit(c) || c == ' ' || c == '-')
            {
                sb.Append(c);
            }
        }
        str = sb.ToString();

        str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", "-");
        str = System.Text.RegularExpressions.Regex.Replace(str, @"-+", "-");

        return str;
    }
}