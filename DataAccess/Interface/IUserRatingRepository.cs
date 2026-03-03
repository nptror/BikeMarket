using DataAccess.Models;

namespace DataAccess.Interface
{
    public interface IUserRatingRepository
    {
        Task<List<UserRating>> GetByRatedUserAsync(int ratedUserId);
        Task<UserRating?> GetByOrderAndRaterAsync(int orderId, int raterId);
        Task AddAsync(UserRating rating);
    }
}
