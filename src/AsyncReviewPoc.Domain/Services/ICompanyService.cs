using AsyncReviewPoc.Domain.CacheModels;
using AsyncReviewPoc.Domain.Dto;

namespace AsyncReviewPoc.Domain.Services
{
    public interface ICompanyService
    {
        Task<CompanyDto?> GetCompanyAsync(Guid id);
        Task AddCompanyAsync(CompanyDto companyDto);
        Task UpdateCompanyAsync(CompanyDto companyDto);
        Task AddReviewAsync(ReviewDto reviewDto);
    }
}
