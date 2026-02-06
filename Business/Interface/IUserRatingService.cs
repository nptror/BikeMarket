using DataAccess.Models;

namespace Business.Interface
{
    public interface IUserRatingService
    {
        Task<List<UserRating>> GetByRatedUserAsync(int ratedUserId);
    }
}
