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

        public Task<UserRating?> GetByOrderAndRaterAsync(int orderId, int raterId)
        {
            return _userRatingRepository.GetByOrderAndRaterAsync(orderId, raterId);
        }

        public Task CreateAsync(UserRating rating)
        {
            return _userRatingRepository.AddAsync(rating);
        }
    }
}
