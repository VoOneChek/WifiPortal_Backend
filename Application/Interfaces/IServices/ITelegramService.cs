using Application.Common.ResultInfo;
using Application.DTOs.OtpCodeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface ITelegramService
    {
        Task<Result> SendMessageAsync(long chatId, string message);
        Task<Result> SendPhoneRequestAsync(long chatId);
        Task<Result> SendTelegramAsync(string phoneNumber, string code);
    }
}
