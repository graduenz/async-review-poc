using AsyncReviewPoc.Domain.CacheModels;
using AsyncReviewPoc.Domain.Dto;
using AsyncReviewPoc.Domain.Services;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class ConsumerHostedService : IHostedService
{
    private readonly ILogger<ConsumerHostedService> _logger;
    private readonly ConnectionFactory _rabbitmqConnectionFactory;
    private readonly ICompanyService _companyService;
    private readonly IMapper _mapper;
    private IConnection? _connection;
    private IModel? _channel;

    public ConsumerHostedService(
        ILogger<ConsumerHostedService> logger,
        ConnectionFactory rabbitmqConnectionFactory,
        ICompanyService companyService,
        IMapper mapper)
    {
        _logger = logger;
        _rabbitmqConnectionFactory = rabbitmqConnectionFactory;
        _companyService = companyService;
        _mapper = mapper;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = _rabbitmqConnectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "add-review",
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += Consume;
        
        _channel.BasicConsume(queue: "add-review",
                              autoAck: true,
                              consumer: consumer);

        _logger.LogInformation($"Consumer has been started");

        return Task.CompletedTask;
    }

    private async Task Consume(object model, BasicDeliverEventArgs args)
    {
        var body = args.Body.ToArray();
        var reviewDto = JsonSerializer.Deserialize<ReviewDto>(Encoding.UTF8.GetString(body));

        if (reviewDto == null) throw new InvalidOperationException("Consumed data model is null");

        _logger.LogInformation($"Processing review with rating {reviewDto.Rating} for company {reviewDto.CompanyId} by {reviewDto.Author}");

        var companyDto = await _companyService.GetCompanyAsync(reviewDto.CompanyId);
        var company = _mapper.Map<Company>(companyDto);

        if (company.Reviews == null) company.Reviews = new List<Review>();

        var review = _mapper.Map<Review>(reviewDto);
        review.CreatedAt = DateTimeOffset.Now;

        company.Reviews.Add(review);

        company.OverallRating = Math.Round(company.Reviews.Average(m => m.Rating), 1);

        companyDto = _mapper.Map<CompanyDto>(company);

        await _companyService.UpdateCompanyAsync(companyDto);

        _logger.LogInformation($"Review with rating {reviewDto.Rating} for company {reviewDto.CompanyId} by {reviewDto.Author} has been processed");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_connection != null) _connection.Close();

        return Task.CompletedTask;
    }
}