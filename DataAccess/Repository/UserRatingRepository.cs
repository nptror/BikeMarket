using DataAccess.Interface;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    public class UserRatingRepository : IUserRatingRepository
    {
        private readonly VehicleMarketContext _context;

        public UserRatingRepository(VehicleMarketContext context)
        {
            _context = context;
        }

        public Task<List<UserRating>> GetByRatedUserAsync(int ratedUserId)
        {
            return _context.UserRatings
                .Include(r => r.Rater)
                .Include(r => r.Order)
                .ThenInclude(o => o.Vehicle)
                .Where(r => r.RatedUserId == ratedUserId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public Task<UserRating?> GetByOrderAndRaterAsync(int orderId, int raterId)
        {
            return _context.UserRatings
                .FirstOrDefaultAsync(r => r.OrderId == orderId && r.RaterId == raterId);
        }

        public async Task AddAsync(UserRating rating)
        {
            _context.UserRatings.Add(rating);
            await _context.SaveChangesAsync();
        }
    }
}
