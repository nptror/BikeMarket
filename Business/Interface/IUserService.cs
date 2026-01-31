using Business.Models;
using DataAccess.Models;
using DTO.User;

namespace Business.Interface;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<AuthResult> RegisterAsync(UserRegisterDTO registerDto);
    Task<AuthResult> AuthenticateAsync(UserLoginDTO loginDto);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
