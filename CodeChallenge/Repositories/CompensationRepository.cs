using CodeChallenge.Data;
using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS8632
namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly CompensationContext _compensationContext;
        private readonly ILogger<IEmployeeRepository> _logger;
        private readonly IEmployeeService _employeeService;

        public CompensationRepository(ILogger<IEmployeeRepository> logger, CompensationContext compensationContext, IEmployeeService employeeService)
        {
            _compensationContext = compensationContext;
            _logger = logger;
            _employeeService = employeeService;
        }

        public Compensation Add(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();

            Employee? employee = null;

            // use existing employee
            if(compensation.EmployeeId != null)
                employee = _employeeService.GetById(compensation.EmployeeId);

            // or create an employee
            if (employee == null && compensation.Employee != null)
                employee = _employeeService.Create(compensation.Employee);

            if (employee != null)
            {
                _compensationContext.Compensations.Add(compensation);
                compensation.Employee = employee;
            }

            return compensation;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            return
                _compensationContext.Compensations
                .Include(x => x.Employee)
                .AsNoTracking()
                .SingleOrDefault(x => x.Employee.EmployeeId == employeeId);
        }

        public Task SaveAsync()
        {
            return _compensationContext.SaveChangesAsync();
        }

        public Compensation Remove(Compensation compensation)
        {
            return _compensationContext.Remove(compensation).Entity;
        }

    }
}
#pragma warning restore CS8632