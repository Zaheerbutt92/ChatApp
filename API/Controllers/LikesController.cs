using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserID = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserID);
            
            if(likedUser is null) return NotFound();

            if(sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userliked = await _likesRepository.GetUserLike(sourceUserID,likedUser.Id);

            if(userliked is not null) return BadRequest("You already liked this user.");

            userliked = new UserLike 
            {
                SourceUserId = sourceUserID,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userliked);

            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeViewModel>>> GetUserLike([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            
            var users = await _likesRepository.GetUserLikes(likesParams);
            
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}