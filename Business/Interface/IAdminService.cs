using DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interface
{
    public interface IAdminService
    {
        Task<List<UserProfileDTO>> GetAllUserAsync(string? search = null, decimal RatingAvg = 0, string? role = null, string? sortBy="email", string? sortOrder ="asc");
        Task<UserProfileDTO> GetUserByIdAsync(int userId);
        Task<object> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
    }
}
