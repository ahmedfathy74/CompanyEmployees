using CompanyEmployees.Presentation.ModelBinders;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.ActionFilters;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    //[Route("api/[controller]")]
    //[ApiVersion("1.0")]
    [Route("api/companies")]
    [ApiController]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _services;

        public CompaniesController(IServiceManager services) => _services = services;

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }

        [HttpGet(Name = "GetCompanies")]
        [Authorize]
        public async Task<IActionResult> GetCompanies()
        {
            //throw new Exception("Exception");
            var companies = await _services.CompanyService.GetAllCompaniesAsync(trackChanges: false);
                return Ok(companies);
           
        }

        [HttpGet("{id:guid}", Name = "CompanyById")]
        //[ResponseCache(Duration = 60)] //--> this for caching in the header and select the duration
        //[HttpCacheExpiration(CacheLocation =CacheLocation.Public,MaxAge =60)]
        //[HttpCacheValidation(MustRevalidate =false)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _services.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType =typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            var companies = await _services.CompanyService.GetByIdsAsync(ids, trackChanges: false);

            return Ok(companies);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            /* // we will replace that with one in the validation filter
            if(company is null)
                return BadRequest("CompanyForCreationDto object is null");

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            */
            var createdCompany = await _services.CompanyService.CreateCompanyAsync(company);

            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result = await _services.CompanyService.CreateCompanyCollectionAsync(companyCollection);

            return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _services.CompanyService.DeleteCompanyAsync(id, trackChanges: false);

            return NoContent();
        }

        // we have here an issue when i try to update company and add nes employee return 500 internal server (somethig related with saving in the entities) 
        //https://localhost:7085/api/companies/C9D4C053-49B6-410C-BC78-2D54A9991870
        /*
         * {
            "name": "Advansys",
            "address": "Free Zone, Nasr City",
            "country": "Masr",
            "employees": [
                {
                    "name": "ahmed mohamed fathy",
                    "age": 22,
                    "position": "senior Software developer"
                }
            ]
            }
         */
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            /*
            if (company is null)
                return BadRequest("CompanyForUpdateDto object is null");

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            */
            await _services.CompanyService.UpdateCompanyAsync(id,company,trackChanges: true);

            return NoContent();
        }
    }
}
