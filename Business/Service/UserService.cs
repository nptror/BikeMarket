using Business.Interface;
using Business.Models;
using DataAccess.Interface;
using DataAccess.Models;
using DTO.User;
using Microsoft.AspNetCore.Identity;

namespace Business.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public Task<List<User>> GetAllAsync()
    {
        return _userRepository.GetAllAsync();
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return _userRepository.GetByIdAsync(id);
    }

    public async Task<AuthResult> RegisterAsync(UserRegisterDTO registerDto)
    {
        if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
        {
            return new AuthResult { Success = false, ErrorMessage = "Email ?ã ???c s? d?ng" };
        }

        var user = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            Phone = registerDto.Phone,
            Role = "buyer",
            EmailVerified = true,
            Status = "active",
            RatingAvg = 0.00m,
            CreatedAt = DateTime.Now
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

        await _userRepository.AddAsync(user);

        return new AuthResult { Success = true, User = user };
    }

    public async Task<AuthResult> AuthenticateAsync(UserLoginDTO loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return new AuthResult { Success = false, ErrorMessage = "Email ho?c m?t kh?u không ?úng" };
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return new AuthResult { Success = false, ErrorMessage = "Email ho?c m?t kh?u không ?úng" };
        }

        if (user.Status != "active")
        {
            return new AuthResult { Success = false, ErrorMessage = "Tài kho?n ?ã b? khóa" };
        }

        return new AuthResult { Success = true, User = user };
    }

    public Task UpdateAsync(User user)
    {
        return _userRepository.UpdateAsync(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user != null)
        {
            await _userRepository.DeleteAsync(user);
        }
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _userRepository.ExistsAsync(id);
    }
}
