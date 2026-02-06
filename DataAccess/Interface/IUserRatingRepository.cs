using DataAccess.Models;

namespace DataAccess.Interface
{
    public interface IUserRatingRepository
    {
        Task<List<UserRating>> GetByRatedUserAsync(int ratedUserId);
    }
}
