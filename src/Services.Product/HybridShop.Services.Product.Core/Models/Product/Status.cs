namespace HybridShop.Services.Product.Core.Product;

public enum PStatus{ Draft, Active, Suspended, Archived }
public class ProductStatus
{
    public ProductStatus(){}
    public ProductStatus(PStatus value)
    {
        if(!Enum.IsDefined<PStatus>(value))
            throw new Exception("Invalid product status.");

        Value = value;
    }
    public PStatus Value { get; private set; }

    public static ProductStatus Draft() => new(PStatus.Draft);
    public static ProductStatus Active() => new(PStatus.Active);
    public static ProductStatus Suspended() => new(PStatus.Suspended);
    public static ProductStatus Archived() => new(PStatus.Archived);
    
}