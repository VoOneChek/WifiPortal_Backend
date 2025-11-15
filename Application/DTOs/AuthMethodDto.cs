using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AuthMethodDto
{
    public class CreateAuthMethodDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsEnabled { get; set; } = true;
    }

    public class ReadAuthMethodDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsEnabled { get; set; }
    }
}
