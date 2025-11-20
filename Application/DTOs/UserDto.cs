using Application.DTOs.AuthSessionDto;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserDto
{
    public class CreateUserDto
    {
        public required string PhoneNumber { get; set; }
        public string? SocialId { get; set; }
        public long? TelegramChatId { get; set; }
        public string? RadiusLogin { get; set; }
        public string? FullName { get; set; }
        public UserRole Role { get; set; } = UserRole.Guest;
    }

    public class ReadUserDto
    {
        public int Id { get; set; }
        public required string PhoneNumber { get; set; }
        public string? SocialId { get; set; }
        public long? TelegramChatId { get; set; }
        public string? RadiusLogin { get; set; }
        public string? FullName { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserDto
    {
        public string? PhoneNumber { get; set; }
        public string? SocialId { get; set; }
        public long? TelegramChatId { get; set; }
        public string? RadiusLogin { get; set; }
        public string? FullName { get; set; }
        public UserRole? Role { get; set; }
    }

    public class UserDetailDto
    {
        public int Id { get; set; }
        public required string PhoneNumber { get; set; }
        public string? SocialId { get; set; }
        public long? TelegramChatId { get; set; }
        public string? RadiusLogin { get; set; }
        public string? FullName { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ReadAuthSessionDto> Sessions { get; set; } = new();
    }
}
