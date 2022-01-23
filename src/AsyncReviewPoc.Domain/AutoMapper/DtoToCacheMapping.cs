using AsyncReviewPoc.Domain.CacheModels;
using AsyncReviewPoc.Domain.Dto;
using AutoMapper;

namespace AsyncReviewPoc.Domain.AutoMapper
{
    public class DtoToCacheMapping : Profile
    {
        public DtoToCacheMapping()
        {
            CreateMap<CompanyDto, Company>();
            CreateMap<ReviewDto, Review>();
        }
    }
}
