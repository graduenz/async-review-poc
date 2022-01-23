namespace AsyncReviewPoc.Domain.Dto
{
    public class ReviewDto
    {
        public Guid CompanyId { get; set; }
        public string? Author { get; set; }
        public int Rating { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
