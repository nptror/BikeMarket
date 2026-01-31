using DataAccess.Interface;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class UserRepository : IUserRepository
{
    private readonly VehicleMarketContext _context;

    public UserRepository(VehicleMarketContext context)
    {
        _context = context;
    }

    public Task<List<User>> GetAllAsync()
    {
        return _context.Users.ToListAsync();
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Users.AnyAsync(e => e.Id == id);
    }
}
