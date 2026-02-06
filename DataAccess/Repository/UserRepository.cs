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

    public async Task<List<User>> GetAllAsync(
        string? search = null,
        decimal ratingAvg = 0,
        string? role = null,
        string? sortBy = "email",
        string? sortOrder = "asc")
    {
        var query = _context.Users.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(u =>
                (u.Name != null && u.Name.ToLower().Contains(searchLower)) ||
                (u.Email != null && u.Email.ToLower().Contains(searchLower)));
        }

        // Apply rating filter
        if (ratingAvg > 0)
        {
            query = query.Where(u => u.RatingAvg >= ratingAvg);
        }

        // Apply role filter
        if (!string.IsNullOrWhiteSpace(role) && role != "all")
        {
            query = query.Where(u => u.Role == role);
        }

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "name" => sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(u => u.Name)
                : query.OrderBy(u => u.Name),
            "email" => sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),
            "ratingavg" => sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(u => u.RatingAvg)
                : query.OrderBy(u => u.RatingAvg),
            _ => sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email)
        };

        return await query.ToListAsync();
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
