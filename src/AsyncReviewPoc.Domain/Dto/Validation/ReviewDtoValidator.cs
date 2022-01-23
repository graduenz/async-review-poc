using FluentValidation;

namespace AsyncReviewPoc.Domain.Dto.Validation
{
    public class ReviewDtoValidator : AbstractValidator<ReviewDto>
    {
        public ReviewDtoValidator()
        {
            RuleFor(m => m.Author)
                .MinimumLength(3)
                .NotEmpty();

            RuleFor(m => m.Rating)
                .InclusiveBetween(1, 5);

            RuleFor(m => m.Description)
                .MinimumLength(3)
                .NotEmpty();
        }
    }
}
