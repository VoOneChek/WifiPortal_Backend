using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IJwtService
    {
        string GenerateToken(User? user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
