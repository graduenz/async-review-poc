using AsyncReviewPoc.Domain.CacheModels;
using AsyncReviewPoc.Domain.Dto;
using AutoMapper;

namespace AsyncReviewPoc.Domain.AutoMapper
{
    public class CacheToDtoMapping : Profile
    {
        public CacheToDtoMapping()
        {
            CreateMap<Company, CompanyDto>();
            CreateMap<Review, ReviewDto>();
        }
    }
}
