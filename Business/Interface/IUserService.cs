using Business.Models;
using DataAccess.Models;
using DTO.User;

namespace Business.Interface;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<List<UserProfileDTO>> GetAllUserAsync(
        string? search = null,
        decimal ratingAvg = 0,
        string? role = null,
        string? sortBy = "email",
        string? sortOrder = "asc");
    Task<User?> GetByIdAsync(int id);
    Task<AuthResult> RegisterAsync(UserRegisterDTO registerDto);
    Task<AuthResult> AuthenticateAsync(UserLoginDTO loginDto);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
