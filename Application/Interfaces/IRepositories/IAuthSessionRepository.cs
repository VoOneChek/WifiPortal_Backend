using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IRepositories
{
    public interface IAuthSessionRepository : IRepository<AuthSession>
    {
        Task<AuthSession?> GetActiveByMacAsync(string macAddress);
        Task<IEnumerable<AuthSession>> GetByUserAsync(int userId);
    }
}
