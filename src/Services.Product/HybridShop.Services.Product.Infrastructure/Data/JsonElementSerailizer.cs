using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace HybridShop.Services.Product.Infrastructure;

public class JsonElementSerializer : SerializerBase<JsonElement>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JsonElement value)
    {
        var writer = context.Writer;

        switch (value.ValueKind)
        {
            case JsonValueKind.String:
                writer.WriteString(value.GetString());
                break;
            case JsonValueKind.Number:
                if (value.TryGetInt32(out int i))
                    writer.WriteInt32(i);
                else if (value.TryGetInt64(out long l))
                    writer.WriteInt64(l);
                else
                    writer.WriteDouble(value.GetDouble());
                break;
            case JsonValueKind.True:
                writer.WriteBoolean(true);
                break;
            case JsonValueKind.False:
                writer.WriteBoolean(false);
                break;
            case JsonValueKind.Null:
                writer.WriteNull();
                break;
            case JsonValueKind.Object:
                var document = BsonSerializer.Deserialize<BsonDocument>(value.GetRawText());
                BsonSerializer.Serialize(writer, document);
                break;
            case JsonValueKind.Array:
                var array = BsonSerializer.Deserialize<BsonArray>(value.GetRawText());
                BsonSerializer.Serialize(writer, array);
                break;
            default:
                writer.WriteNull();
                break;
        }
    }

    public override JsonElement Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        var currentBsonType = reader.GetCurrentBsonType();

        object? nativeValue = currentBsonType switch
        {
            BsonType.String => reader.ReadString(),
            BsonType.Int32 => reader.ReadInt32(),
            BsonType.Int64 => reader.ReadInt64(),
            BsonType.Double => reader.ReadDouble(),
            BsonType.Boolean => reader.ReadBoolean(),
            BsonType.Null => null,
            BsonType.Document => BsonSerializer.Deserialize<object>(reader),
            BsonType.Array => BsonSerializer.Deserialize<object>(reader),
            _ => throw new BsonSerializationException($"Unsupported BSON type for JsonElement deserialization: {currentBsonType}")
        };

        var jsonString = JsonSerializer.Serialize(nativeValue);
        using var doc = JsonDocument.Parse(jsonString);
        return doc.RootElement.Clone();
    }
}