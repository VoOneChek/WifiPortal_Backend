using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OtpCodeDto
{
    public class CreateOtpCodeDto
    {
        public required string PhoneNumber { get; set; }
    }

    public class OtpStatusDto
    {
        public string PhoneNumber { get; set; } = null!;
        public bool HasActiveOtp { get; set; }
        public string Message { get; set; } = null!;
    }

    public class VerifyOtpRequestDto
    {
        public required string PhoneNumber { get; set; }
        public required string Code { get; set; }
    }
}
