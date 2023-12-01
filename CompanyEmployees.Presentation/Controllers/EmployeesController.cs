using Microsoft.AspNetCore.JsonPatch;
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
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _service;

        public EmployeesController(IServiceManager service) => _service = service;

        /*we have the companyId parameter in our action and this parameter will be mapped from the main route
         * For that reason, 
         * we didn’t place it in the [HttpGet] attribute as we did with the GetCompany action*/
        [HttpGet] 
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        {
            var employees = await _service.EmployeeService.GetEmployeesAsync(companyId, trackChanges: false);

            return Ok(employees);
        }

        [HttpGet("{id:guid}" , Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var employee = await _service.EmployeeService.GetEmployeeAsync(companyId, id, trackChanges: false);

            return Ok(employee);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            /*
            if(employee is null)
                return BadRequest("EmployeeForCreationDto object is null");

            if(!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            */
            var employeeToReturn = await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee,trackChanges:false);

            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id,trackChanges:false);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            /*
            if(employee is null)
                return BadRequest("EmployeeForUpdateDto object is null");

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            */
            await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee, 
                compTrackChanges: false, empTrackChanges: true);

            return NoContent();

        }
        // error here : send patchDoc equal null , i think because i want to update to .net 7
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("patchDoc object sent from client is null.");

            var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId,id,compTrackChanges:false,empTrackChanges:true);
            /* for validation */
            //patchDoc.ApplyTo(employeeToPatch, ModelState);
            //TryValidateModel(result.employeeToPatch);
            /*
             if (!ModelState.IsValid)
                   return UnprocessableEntity(ModelState);
             */

            patchDoc.ApplyTo(result.employeeToPatch);

            await _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity);

            return NoContent();
        }

    }
}
