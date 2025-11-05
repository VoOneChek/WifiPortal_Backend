using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AuthLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }

        public string Method { get; set; } = null!;
        public bool Success { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
