namespace AsyncReviewPoc.Domain.CacheModels
{
    public class Review
    {
        public string? Author { get; set; }
        public int Rating { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
