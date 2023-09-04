using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
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

        [HttpGet("{id:guid}")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _services.CompanyService.GetCompany(id, trackChanges: false);
            return Ok(company);
        }

    }
}
