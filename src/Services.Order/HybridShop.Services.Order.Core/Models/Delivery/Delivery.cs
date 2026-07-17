using System.Text.Json.Serialization;

namespace HybridShop.Services.Order.Core.Models.Delivery;

public class DeliveryMethod
{
    public DeliveryMethod(){}

    [JsonConstructor]
    private DeliveryMethod(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    [JsonPropertyName("name")]
    public string Name { get; private set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; private set; }

    public static DeliveryMethod ChooseDelivery(string name, decimal price)
    {
        return new DeliveryMethod(name, price);
    }

}