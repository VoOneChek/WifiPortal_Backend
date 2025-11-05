using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AuthMethod
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // "SMS", "RADIUS", "VK"
        public string Description { get; set; } = null!;
        public bool IsEnabled { get; set; } = true;
    }
}
