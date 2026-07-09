using HybridShop.Services.Product.Application.Dto;
using HybridShop.Services.Product.Core.Interface;
using HybridShop.Services.Product.Core.Product;

namespace HybridShop.Services.Product.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(
        IProductRepository productRepository
    )
    {
        _productRepository = productRepository;
    }

    public async Task<Core.Product.Product> AddProductAsync(AddNewProductDto dto, Guid userId)
    {
       return null;
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