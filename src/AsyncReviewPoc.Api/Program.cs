using AsyncReviewPoc.Domain.AutoMapper;
using AsyncReviewPoc.Domain.Dto.Validation;
using AsyncReviewPoc.Domain.Services;
using AsyncReviewPoc.Domain.Services.Implementations;
using AutoMapper;
using FluentValidation.AspNetCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using RabbitMQ.Client;

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(typeof(ReviewDtoValidator).Assembly));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application dependencies
builder.Services.AddScoped<ConnectionFactory>(ctx => {
    return new ConnectionFactory() { Uri = new Uri(builder.Configuration["ConnectionStrings:RabbitMQ"]) };
});

builder.Services.AddScoped<IMongoDatabase>(ctx => {
    var client = new MongoClient(builder.Configuration["ConnectionStrings:MongoDB"]);
    return client.GetDatabase("async-review-poc");
});

var mapperConfig = new MapperConfiguration(mc => {
    mc.AddProfile(new CacheToDtoMapping());
    mc.AddProfile(new DtoToCacheMapping());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Add domain services
builder.Services.AddScoped<ICompanyService, DefaultCompanyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
