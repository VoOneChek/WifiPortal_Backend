using Application.Common.ResultInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IOtpSenderService
    {
        Task<Result> SendSmsAsync(string phoneNumber);
        Task<Result> ResendSmsAsync(string phoneNumber);
        Task<Result> SendTelegramAsync(string phoneNumber);
        Task<Result> ResendTelegramAsync(string phoneNumber);
    }
}
