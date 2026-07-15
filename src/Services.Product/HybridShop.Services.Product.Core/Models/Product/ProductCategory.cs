using HybridShop.Services.Product.Core.Exceptions;

namespace HybridShop.Services.Product.Core.Product;

public enum PCategory { Electronics, Clothes, Other }
public class ProductCategory
{
    public ProductCategory(){}
    public ProductCategory(PCategory value)
    {
        if(!Enum.IsDefined<PCategory>(value))
            throw new InvalidProductException();

        Value = value;
    }
    public PCategory Value { get; private set; }

    public static ProductCategory Electronics() => new(PCategory.Electronics);
    public static ProductCategory Clothes() => new(PCategory.Clothes);
    public static ProductCategory Other() => new(PCategory.Other);
}