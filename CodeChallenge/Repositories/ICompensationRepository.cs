using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation Add(Compensation compensation);
        Compensation GetByEmployeeId(string employeeId);
        Task SaveAsync();
        Compensation Remove(Compensation compensation);
    }
}