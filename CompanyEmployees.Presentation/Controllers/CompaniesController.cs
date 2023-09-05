using CompanyEmployees.Presentation.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _services;

        public CompaniesController(IServiceManager services) => _services = services;

        [HttpGet]
        public IActionResult GetCompanies()
        {
            //throw new Exception("Exception");
            var companies = _services.CompanyService.GetAllCompanies(trackChanges: false);
                return Ok(companies);
           
        }

        [HttpGet("{id:guid}", Name = "CompanyById")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _services.CompanyService.GetCompany(id, trackChanges: false);
            return Ok(company);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public IActionResult GetCompanyCollection([ModelBinder(BinderType =typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            var companies = _services.CompanyService.GetByIds(ids, trackChanges: false);

            return Ok(companies);
        }

        [HttpPost]
        public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
        {
            if(company is null)
                return BadRequest("CompanyForCreationDto object is null");

            var createdCompany = _services.CompanyService.CreateCompany(company);

            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpPost("collection")]
        public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result = _services.CompanyService.CreateCompanyCollection(companyCollection);

            return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteCompany(Guid id)
        {
            _services.CompanyService.DeleteCompany(id, trackChanges: false);

            return NoContent();
        }
    }
}
