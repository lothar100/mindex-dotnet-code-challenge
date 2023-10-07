using CodeChallenge.Models;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        [DataRow( 4, "16a596ae-edd3-4847-99fe-c4518e82c86f", "John",    "Lennon" )]
        [DataRow( 0, "b7839309-3348-463b-a7e3-5de1c168beb3", "Paul",    "McCartney" )]
        [DataRow( 2, "03aa1462-ffa9-4978-901b-7c001562cf6f", "Ringo",   "Starr" )]
        [DataRow( 0, "62c1084e-6e34-4630-93fd-9153afb65309", "Pete",    "Best" )]
        [DataRow( 0, "c0c2293d-16bd-4603-8e08-638a9d18b22c", "George",  "Harrison" )]
        public void GetReportingStructure_Returns_Ok(
            int expectedNumberOfReports,
            string employeeId,
            string expectedFirstName,
            string expectedLastName
        )
        {
            // Arrange ... (DataRow)

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/reportingStructure/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var report = response.DeserializeContent<ReportingStructure>();

            Console.WriteLine("(expected, actual) title");
            Console.WriteLine($"({expectedFirstName}, {report.Employee.FirstName}) first name");
            Console.WriteLine($"({expectedLastName}, {report.Employee.LastName}) last name");
            Console.WriteLine($"({expectedNumberOfReports}, {report.NumberOfReports}) number of reports");

            Assert.AreEqual(expectedFirstName, report.Employee.FirstName);
            Assert.AreEqual(expectedLastName, report.Employee.LastName);
            Assert.AreEqual(expectedNumberOfReports, report.NumberOfReports);
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Salary = 100000d,
                EffectiveDate = DateTime.Now,
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation);
            Assert.IsNotNull(newCompensation.Employee);
            Assert.IsNotNull(newCompensation.Employee.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        [DataRow(75000d, "62c1084e-6e34-4630-93fd-9153afb65309")]
        [DataRow(54000d, "c0c2293d-16bd-4603-8e08-638a9d18b22c")]
        public void GetCompensationByEmployeeId_Returns_Ok(double salary, string employeeId)
        {
            // Arrange
            var inputCompensation = new Compensation()
            {
                Salary = salary,
                EffectiveDate = DateTime.Now,
                EmployeeId = employeeId
            };
            var requestContent = new JsonSerialization().ToJson(inputCompensation);
            var postRequestTask = _httpClient.PostAsync("api/employee/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var createResponse = postRequestTask.Result;
            Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);
            var outputCompensation = createResponse.DeserializeContent<Compensation>();

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{outputCompensation.Employee.EmployeeId}");
            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            Assert.IsNotNull(outputCompensation);
            Assert.IsNotNull(outputCompensation.Employee);
            Assert.AreEqual(inputCompensation.Salary, outputCompensation.Salary);
            Assert.AreEqual(inputCompensation.EffectiveDate, outputCompensation.EffectiveDate);

            var getCompensation = getResponse.DeserializeContent<Compensation>();
            Assert.IsNotNull(getCompensation);
            Assert.IsNotNull(getCompensation.Employee);
            Assert.AreEqual(outputCompensation.Employee.EmployeeId, getCompensation.Employee.EmployeeId);
            Assert.AreEqual(outputCompensation.Salary, getCompensation.Salary);
            Assert.AreEqual(outputCompensation.EffectiveDate, getCompensation.EffectiveDate);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_NotFound()
        {
            // Arrange
            var employeeId = "Invalid_Id";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{employeeId}");
            var getResponse = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

    }
}
