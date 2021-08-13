using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.ViewModels;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeViewModel>> GetUserLikes(LikesParams likesParams);
    }
}