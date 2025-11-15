using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByPhoneAsync(string phoneNumber);
        Task<User?> GetBySocialIdAsync(string socialId);
    }
}
