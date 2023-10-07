using CodeChallenge.Models;
using System;

#pragma warning disable CS8632
namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        Compensation Create(Compensation compensation);
        Compensation? GetByEmployeeId(String employeeId);
    }
}
#pragma warning restore CS8632