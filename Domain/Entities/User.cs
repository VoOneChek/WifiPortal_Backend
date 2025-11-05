using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string? SocialId { get; set; } // ID из VK, SMS, RADIUS
        public string? RadiusLogin { get; set; }
        public string? FullName { get; set; }

        public UserRole Role { get; set; } = UserRole.Guest;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<AuthSession> Sessions { get; set; } = new List<AuthSession>();
    }
}
