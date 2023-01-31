﻿using HardwareShop.Business.Dtos;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Services
{
    public interface IUserService
    {
        Task<CreatedUserDto> CreateUserAsync(string username, string password);
        Task<List<UserDto>> GetUserDtosAsync();
        Task<LoginDto?> LoginAsync(string username, string password);
        Task<LoginDto?> LoginByTokenAsync(string token);
        Task<IAssetTable?> GetCurrentUserAvatarAsync();

    }
}
