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
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(WifiPortalContext context) : base(context) { }

        public async Task<User?> GetByPhoneAsync(string phoneNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User?> GetBySocialIdAsync(string socialId)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.SocialId == socialId);
        }
    }
}
