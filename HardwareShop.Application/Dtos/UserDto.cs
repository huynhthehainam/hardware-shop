﻿namespace HardwareShop.Application.Dtos
{
    public class UserDto
    {
        public int Id { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

    }
}
