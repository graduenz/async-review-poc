using AsyncReviewPoc.Domain.Dto;
using AsyncReviewPoc.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace AsyncReviewPoc.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var company = await _companyService.GetCompanyAsync(id);

            if (company == null) return NotFound();

            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CompanyDto companyDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _companyService.AddCompanyAsync(companyDto);
            return Ok();
        }
    }
}
