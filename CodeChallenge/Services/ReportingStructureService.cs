using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;
using System;

#pragma warning disable CS8632
namespace CodeChallenge.Services
{
    public class ReportingStructureService : IReportingStructureService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<ReportingStructureService> _logger;

        public ReportingStructureService(ILogger<ReportingStructureService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public ReportingStructure? Read(String id)

        {
            if (String.IsNullOrEmpty(id))
                return null;

            // recursively fill direct reports for all children,
            // otherwise direct reports of children are seen as null
            var employee = getEmployeeWithDirectReports(id);
            if(employee == null)
                return null;

            int numberOfReports = calcNumberOfReports(employee);

            return new ReportingStructure
            {
                Employee = employee,
                NumberOfReports = numberOfReports
            };
        }

        private Employee? getEmployeeWithDirectReports(String id)
        {
            try
            {
                var employee = _employeeRepository.GetById(id);
                if (employee == null)
                    return null;

                if (employee.DirectReports == null || employee.DirectReports.Count == 0)
                    return employee;

                for(int i = 0; i < employee.DirectReports.Count; i++)
                {
                    var curDirectReport = employee.DirectReports[i];
                    var filledDirectReport = getEmployeeWithDirectReports(curDirectReport.EmployeeId);

                    if (filledDirectReport != null)
                        employee.DirectReports[i] = filledDirectReport;
                }

                return employee;

            }
            catch
            {
                return null;
            }
        }

        private int calcNumberOfReports(Employee employee)
        {
            if(employee.DirectReports == null || employee.DirectReports.Count == 0)
                return 0;

            int numberOfReports = 0;
            foreach(var directReport in employee.DirectReports)
            {
                // +1 for this employee
                numberOfReports++;
                // +n for the number of directReports to this employee
                numberOfReports += calcNumberOfReports(directReport);
            }

            return numberOfReports;
        }

    }
}
#pragma warning restore CS8632