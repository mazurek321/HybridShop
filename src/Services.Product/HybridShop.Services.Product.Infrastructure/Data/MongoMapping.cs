using System.Text.Json;
using HybridShop.Services.Product.Core.Product;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace HybridShop.Services.Product.Infrastructure;

public static class MongoMapping
{
    public static void Register()
    {
        
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.TryRegisterSerializer(new JsonElementSerializer());

        BsonClassMap.RegisterClassMap<Core.Product.Product>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(p => p.Id).SetIdGenerator(GuidGenerator.Instance);
            
            cm.MapProperty(p => p.Title);
            cm.MapProperty(p => p.Slug);
            cm.MapProperty(p => p.Description);
            cm.MapProperty(p => p.Price);
            cm.MapProperty(p => p.Quantity);
            cm.MapProperty(p => p.Status);
            cm.MapProperty(p => p.SellerId);
            cm.MapProperty(p => p.CreatedAt);
            cm.MapProperty(p => p.UpdatedAt);
            cm.MapProperty(p => p.IsDeleted);
            cm.MapProperty(p => p.Category);

            cm.MapProperty(p => p.Attributes)
                    .SetSerializer(new DictionaryInterfaceImplementerSerializer<Dictionary<string, object>>(
                        MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document, 
                        new StringSerializer(), 
                        new ObjectSerializer(type => 
                            type == typeof(string) || 
                            type == typeof(int) || 
                            type == typeof(long) || 
                            type == typeof(double) || 
                            type == typeof(decimal) || 
                            type == typeof(bool) || 
                            type == typeof(DateTime) ||
                            type == typeof(JsonElement) ||
                            type == typeof(object)) 
                    ));

            cm.MapProperty(p => p.Variants);
        });

        BsonClassMap.RegisterClassMap<Price>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(p => p.Value); 
        });

        BsonClassMap.RegisterClassMap<Quantity>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(q => q.Value);
        });

        BsonClassMap.RegisterClassMap<ProductStatus>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(s => s.Value);
        });

        BsonClassMap.RegisterClassMap<ProductCategory>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(s => s.Value);
        });

        BsonClassMap.RegisterClassMap<ProductVariant>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(pv => pv.SkuId);
            cm.MapProperty(pv => pv.Price);
            cm.MapProperty(pv => pv.Quantity);

            cm.MapProperty(pv => pv.Attributes)
              .SetSerializer(new DictionaryInterfaceImplementerSerializer<Dictionary<string, object>>(
                  MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document, 
                  new StringSerializer(), 
                  new ObjectSerializer(type => 
                      type == typeof(string) || 
                      type == typeof(int) || 
                      type == typeof(long) || 
                      type == typeof(double) || 
                      type == typeof(decimal) || 
                      type == typeof(bool) || 
                      type == typeof(DateTime) ||
                      type == typeof(JsonElement) ||
                      type == typeof(object))
              ));
        });
    }
}