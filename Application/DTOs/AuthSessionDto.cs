using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AuthSessionDto
{
    public class CreateAuthSessionDto
    {
        public int UserId { get; set; }
        public required string MacAddress { get; set; }
        public string? IpAddress { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class ReadAuthSessionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string MacAddress { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}
