using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId,likedUserId);
        }

        public async Task<PagedList<LikeViewModel>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }

            if(likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeViewModel
            {
                Username = user.UserName,
                Age = user.DateOfBirth.CalculateAge(),
                City = user.City,
                Id = user.Id,
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(p =>p.IsMain).Url
            });

            return await PagedList<LikeViewModel>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
           return await _context.Users
                    .Include(x=>x.LikedUsers)
                    .FirstOrDefaultAsync(x=>x.Id == userId);
        }
    }
}