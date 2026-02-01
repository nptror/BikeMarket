using Business.Interface;
using DataAccess.Interface;
using DataAccess.Models;
using DTO.User;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Service
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AdminService> _logger;
        private readonly VehicleMarketContext _context;

        public AdminService(IUserRepository userRepository, ILogger<AdminService> logger, VehicleMarketContext context)
        {
            _userRepository = userRepository;
            _logger = logger;
            _context = context;
        }

        public async Task<List<UserProfileDTO>> GetAllUserAsync(
            string? search = null,
            decimal RatingAvg = 0,
            string? role = null,
            string? sortBy = "email",
            string? sortOrder = "asc")
        {
            var users = await _userRepository.GetAllAsync(search, RatingAvg, role, sortBy, sortOrder);

            return users.Select(u => new UserProfileDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role,
                RatingAvg = u.RatingAvg ?? 0
            }).ToList();
        }

        public async Task<UserProfileDTO> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return null;
            }
            return new UserProfileDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                RatingAvg = user.RatingAvg ?? 0
            };
        }

        public async Task<object> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update.", userId);
                return null;
            }

            //Update user properties
            if (!string.IsNullOrWhiteSpace(updateUserDto.Phone))
            {
                user.Phone = updateUserDto.Phone;
            }
            if (!string.IsNullOrWhiteSpace(updateUserDto.Role))
            {
                user.Role = updateUserDto.Role;
            }

            await _userRepository.UpdateAsync(user);
            return new { Message = "User updated successfully." };
        }
    }
}
