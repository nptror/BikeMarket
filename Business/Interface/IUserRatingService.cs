using DataAccess.Models;

namespace Business.Interface
{
    public interface IUserRatingService
    {
        Task<List<UserRating>> GetByRatedUserAsync(int ratedUserId);
        Task<UserRating?> GetByOrderAndRaterAsync(int orderId, int raterId);
        Task CreateAsync(UserRating rating);
    }
}
