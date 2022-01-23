namespace AsyncReviewPoc.Domain.Dto
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double OverallRating { get; set; }
        public IList<ReviewDto>? Reviews { get; set; }
    }
}
