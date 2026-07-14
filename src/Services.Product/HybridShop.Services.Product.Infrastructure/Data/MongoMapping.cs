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
    }
}