using FluentValidation;

namespace AsyncReviewPoc.Domain.Dto.Validation
{
    public class CompanyDtoValidator : AbstractValidator<CompanyDto>
    {
        public CompanyDtoValidator()
        {
            RuleFor(m => m.Name)
                .MinimumLength(3)
                .NotEmpty();

            RuleFor(m => m.OverallRating)
                .InclusiveBetween(0, 5);
        }
    }
}
