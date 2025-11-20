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
        Task<AuthSession?> GetInactiveByMacAsync(string macAddress);
        Task<IEnumerable<AuthSession>> GetByUserAsync(int userId);
        Task<IEnumerable<AuthSession>> GetActiveByUserIdAsync(int userId);
        Task<bool> IsMacAddressExistsAsync(string macAddress);
    }
}
