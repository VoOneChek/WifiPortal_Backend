using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AuthSessionRepository : Repository<AuthSession>, IAuthSessionRepository
    {
        public AuthSessionRepository(WifiPortalContext context) : base(context) { }

        public async Task<AuthSession?> GetInactiveByMacAsync(string macAddress)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.MacAddress == macAddress && !s.IsActive);
        }

        public async Task<IEnumerable<AuthSession>> GetByUserAsync(int userId)
        {
            return await _dbSet.Where(s => s.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<AuthSession>> GetActiveByUserIdAsync(int userId)
        {
            return await _context.AuthSessions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();
        }

        public async Task<bool> IsMacAddressExistsAsync(string macAddress)
        {
            return await _context.AuthSessions
                .AnyAsync(s => s.MacAddress.ToLower() == macAddress.ToLower());
        }

    }
}
