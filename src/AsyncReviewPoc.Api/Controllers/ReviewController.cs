using AsyncReviewPoc.Domain.Dto;
using AsyncReviewPoc.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace AsyncReviewPoc.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public ReviewController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReviewDto reviewDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _companyService.AddReviewAsync(reviewDto);
            return Ok();
        }
    }
}