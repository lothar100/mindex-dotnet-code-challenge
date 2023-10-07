using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IReportingStructureService _reportingStructureService;
        private readonly ICompensationService _compensationService;

        public EmployeeController(
            ILogger<EmployeeController> logger,
            IEmployeeService employeeService,
            IReportingStructureService reportingStructureService,
            ICompensationService compensationService
        )
        {
            _logger = logger;
            _employeeService = employeeService;
            _reportingStructureService = reportingStructureService;
            _compensationService = compensationService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        [HttpGet("reportingStructure/{id}")]
        public IActionResult GetReportingStructure(String id)
        {
            _logger.LogDebug($"Received reporting structure get request for '{id}'");
            var reportingStructure = _reportingStructureService.Read(id);
            if (reportingStructure == null)
                return NotFound();

            return Ok(reportingStructure);
        }

        [HttpPost("compensation")]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            var employeeDescription =
                compensation.EmployeeId != null
                ? compensation.EmployeeId
                : $"{compensation.Employee?.FirstName ?? "null"} {compensation.Employee?.LastName ?? "null"}";

            _logger.LogDebug($"Received compensation create request for '{employeeDescription}'");

            var newCompensation = _compensationService.Create(compensation);

            var locationUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}/{newCompensation.Employee.EmployeeId}");

            return Created(locationUri, newCompensation);
        }

        [HttpGet("compensation/{id}")]
        public IActionResult GetCompensation(String id)
        {
            _logger.LogDebug($"Received compensation get request for '{id}'");

            var compensation = _compensationService.GetByEmployeeId(id);
            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }

    }
}
