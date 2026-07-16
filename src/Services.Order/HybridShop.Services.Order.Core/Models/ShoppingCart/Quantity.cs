using System.Text.Json.Serialization;
using HybridShop.Services.Order.Core.Exceptions;

namespace HybridShop.Services.Order.Core.Models.ShoppingCart;
public class Quantity
{
    public Quantity(){}

    [JsonConstructor]
    public Quantity(int value)
    {
        if(value < 0) 
            throw new InvalidQuantityException();
        Value = value;
    }

    [JsonPropertyName("value")]
    public int Value { get; private set; }

}