using Business.Interface;
using DataAccess.Interface;
using DataAccess.Models;

namespace Business.Service
{
    public class UserRatingService : IUserRatingService
    {
        private readonly IUserRatingRepository _userRatingRepository;

        public UserRatingService(IUserRatingRepository userRatingRepository)
        {
            _userRatingRepository = userRatingRepository;
        }

        public Task<List<UserRating>> GetByRatedUserAsync(int ratedUserId)
        {
            return _userRatingRepository.GetByRatedUserAsync(ratedUserId);
        }
    }
}
