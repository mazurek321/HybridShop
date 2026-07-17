using System.Text.Json.Serialization;
using HybridShop.Services.Order.Core.Exceptions; // Zakładam, że tu trzymasz wyjątki domenowe

namespace HybridShop.Services.Order.Core.Models.Delivery;

public enum DeliveryType
{
    CourierDpd = 1,
    PaczkomatInPost = 2,
    PersonalPickup = 3
}

public class DeliveryOption
{
    public DeliveryOption() {}

    [JsonConstructor]
    public DeliveryOption(DeliveryType type, string name, decimal price)
    {
        Type = type;
        Name = name;
        Price = price;
    }

    [JsonPropertyName("type")]
    public DeliveryType Type { get; private set; }

    [JsonPropertyName("name")]
    public string Name { get; private set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; private set; }

    public static DeliveryOption CourierDpd() => 
        new(DeliveryType.CourierDpd, "Kurier DPD", 15.00m);

    public static DeliveryOption PaczkomatInPost() => 
        new(DeliveryType.PaczkomatInPost, "Paczkomat InPost", 12.00m);

    public static DeliveryOption PersonalPickup() => 
        new(DeliveryType.PersonalPickup, "Odbiór osobisty", 0.00m);

    public static DeliveryOption Create(DeliveryType type)
    {
        return type switch
        {
            DeliveryType.CourierDpd => CourierDpd(),
            DeliveryType.PaczkomatInPost => PaczkomatInPost(),
            DeliveryType.PersonalPickup => PersonalPickup(),
            _ => throw new InvalidDeliveryTypeException() 
        };
    }
}