using AsyncReviewPoc.Domain.CacheModels;
using AsyncReviewPoc.Domain.Dto;
using AutoMapper;
using MongoDB.Driver;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AsyncReviewPoc.Domain.Services.Implementations
{
    public class DefaultCompanyService : ICompanyService
    {
        private readonly IMongoDatabase _database;
        private readonly ConnectionFactory _rabbitmqConnectionFactory;
        private readonly IMapper _mapper;

        public DefaultCompanyService(
            IMongoDatabase mongoDatabase,
            ConnectionFactory rabbitmqConnectionFactory,
            IMapper mapper)
        {
            _database = mongoDatabase;
            _rabbitmqConnectionFactory = rabbitmqConnectionFactory;
            _mapper = mapper;
        }

        public async Task<CompanyDto?> GetCompanyAsync(Guid id)
        {
            var companiesCollection = _database.GetCollection<Company>("companies");

            var findResult = await companiesCollection.FindAsync(m => m.Id == id);
            var entity = await findResult.FirstOrDefaultAsync();

            return entity == null ? null : _mapper.Map<CompanyDto>(entity);
        }

        public async Task AddCompanyAsync(CompanyDto companyDto)
        {
            var company = _mapper.Map<Company>(companyDto);
            var companiesCollection = _database.GetCollection<Company>("companies");
            await companiesCollection.InsertOneAsync(company);
        }

        public async Task UpdateCompanyAsync(CompanyDto companyDto)
        {
            // Company must exist
            var entry = await GetCompanyAsync(companyDto.Id);
            if (entry == null) throw new InvalidOperationException($"Company {companyDto.Id} does not exist");

            var company = _mapper.Map<Company>(companyDto);

            var companiesCollection = _database.GetCollection<Company>("companies");
            await companiesCollection.ReplaceOneAsync(m => m.Id == company.Id, company);
        }

        public async Task AddReviewAsync(ReviewDto reviewDto)
        {
            // Company must exist
            var company = await GetCompanyAsync(reviewDto.CompanyId);
            if (company == null) throw new InvalidOperationException($"Company {reviewDto.CompanyId} does not exist");

            // Publishes message to add-review queue
            using (var connection = _rabbitmqConnectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "add-review",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(reviewDto));

                channel.BasicPublish(exchange: "",
                                     routingKey: "add-review",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
