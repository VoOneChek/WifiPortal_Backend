using Application.Common.ResultInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface ISmsService
    {
        Task<Result> SendSmsAsync(string phoneNumber, string message);
    }
}
