using HybridShop.Services.Product.Application.Dto;
using HybridShop.Services.Product.Application.Exceptions;
using HybridShop.Services.Product.Core.Dto;
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
        var product = Core.Product.Product.NewProduct(
            dto.Title,
            slug,
            dto.Description,
            new Price(dto.Price),
            new Quantity(dto.Quantity),
            userId
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

        return new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Slug = product.Slug,
            Description = product.Description!,
            Price = product.Price.Value,
            Quantity = product.Quantity.Value,
            Seller = seller ?? new SellerDto
            {
                Id = product.SellerId,
                Email = "unknown@shop.local",
                Name = "Nieznany",
                Lastname = "Sprzedawca"
            }
        };
    }

    public async Task<IEnumerable<ProductDto>> BrowseProductsAsync(int skip, int take)
    {
        var products = await _productRepository.BrowseProductsAsync(skip, take);

        var tasks = products.Select(async product =>
        {
            var seller = await _userServiceClient.GetSellerDetailsAsync(product.SellerId);

            return new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Slug = product.Slug,
                Description = product.Description ?? string.Empty,
                Price = product.Price.Value,
                Quantity = product.Quantity.Value,
                Seller = seller ?? new SellerDto
                {
                    Id = product.SellerId,
                    Email = "unknown@shop.local",
                    Name = "Nieznany",
                    Lastname = "Sprzedawca"
                }
            };
        });

        return await Task.WhenAll(tasks);
    }

    public async Task UpdateProduct(Guid productId, Guid userId, string role, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if(product is null)
            throw new ProductNotFoundException();

        if(product.SellerId != userId || role != "Admin")
            throw new DontHavePermissionsException();
        
        var newSlug = GenerateSlug(dto.Title);

        product.Update(
            dto.Title,
            newSlug,
            dto.Description,
            new Price(dto.Price),
            new Quantity(dto.Quantity)
        );

        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteProduct(Guid productId, Guid userId, string role)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if(product is null)
            throw new ProductNotFoundException();

        if(product.SellerId != userId || role != "Admin")
            throw new DontHavePermissionsException();

        product.Delete();
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