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
    public class AuthMethodRepository : Repository<AuthMethod>, IAuthMethodRepository
    {
        public AuthMethodRepository(WifiPortalContext context) : base(context) { }

        public async Task<AuthMethod?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Name == name);
        }
    }
}
