using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, 
            EmployeeParameters employeeParameters,bool trackChanges)
        {
            var employees = await FindByCondition(c => c.CompanyId.Equals(companyId), trackChanges)
                                 .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
                                 .Search(employeeParameters.SearchTerm)
                                 .Sort(employeeParameters.OrderBy)
                                 .ToListAsync();

            return PagedList<Employee>
                .ToPagedList(employees, employeeParameters.PageNumber, employeeParameters.pageSize);

            /* **
             * This solution works great with a small amount of data, but with bigger 
                tables with millions of rows, we can improve it by modifying the function to be like that --> **
             *
             * 
             var employees = await FindByCondition(c => c.CompanyId.Equals(companyId), trackChanges)
                                 .OrderBy(e => e.Name)
                                 .Skip((employeeParameters.PageNumber - 1) * employeeParameters.pageSize)
                                 .Take(employeeParameters.pageSize)
                                 .ToListAsync();

            var count = await FindByCondition(e=>e.CompanyId.Equals(companyId), trackChanges).CountAsync();

            return new PagedList<Employee>(employees, count,employeeParameters.PageNumber, employeeParameters.pageSize);
             */

        }


        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id) , trackChanges)
            .SingleOrDefaultAsync();

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee) => Delete(employee);
     
    }
}
