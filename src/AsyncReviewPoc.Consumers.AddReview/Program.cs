using AsyncReviewPoc.Domain.AutoMapper;
using AsyncReviewPoc.Domain.Services;
using AsyncReviewPoc.Domain.Services.Implementations;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using RabbitMQ.Client;

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

await Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(hostConfig =>
    {
        hostConfig.SetBasePath(Directory.GetCurrentDirectory());
        hostConfig.AddJsonFile("hostsettings.json");
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<ConsumerHostedService>();

        services.AddScoped<ConnectionFactory>(ctx => {
            var cfg = ctx.GetRequiredService<IConfiguration>();
            return new ConnectionFactory() {
                Uri = new Uri(cfg["ConnectionStrings:RabbitMQ"]),
                DispatchConsumersAsync = true,
            };
        });

        services.AddScoped<IMongoDatabase>(ctx => {
            var cfg = ctx.GetRequiredService<IConfiguration>();
            var client = new MongoClient(cfg["ConnectionStrings:MongoDB"]);
            return client.GetDatabase("async-review-poc");
        });

        var mapperConfig = new MapperConfiguration(mc => {
            mc.AddProfile(new CacheToDtoMapping());
            mc.AddProfile(new DtoToCacheMapping());
        });
        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        services.AddScoped<ICompanyService, DefaultCompanyService>();
    })
    .Build()
    .RunAsync();