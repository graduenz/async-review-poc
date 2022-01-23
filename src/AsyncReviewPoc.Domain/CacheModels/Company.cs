using MongoDB.Bson.Serialization.Attributes;

namespace AsyncReviewPoc.Domain.CacheModels
{
    public class Company
    {
        [BsonId]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double OverallRating { get; set; }
        public IList<Review>? Reviews { get; set; }
    }
}
