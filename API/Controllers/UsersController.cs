using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberViewModel>>> GetUsers()
        {
            var users = await  _userRepository.GetMembersAsync();
           
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberViewModel>> GetUser(int id)
        {
            return await _userRepository.GetMemberByIdAsync(id);
        }

        [HttpGet("{username}", Name ="GetUser")]
        public async Task<ActionResult<MemberViewModel>> GetUser(string username)
        {
            return await _userRepository.GetMemberByUsernameAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(UserUpdateViewModel userUpdateViewModel)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            _mapper.Map(userUpdateViewModel,user);

            _userRepository.Update(user); 

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user"); 
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoViewModel>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0){
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync()){
                return CreatedAtRoute("GetUser",new {username = user.UserName},_mapper.Map<PhotoViewModel>(photo));
            }
            
            return BadRequest("Problem adding photo.");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

            if(photo.IsMain) return BadRequest("This is already your main photo.");

            var currentMain = user.Photos.FirstOrDefault(x=>x.IsMain);
            if(currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync())
             return NoContent();

            return BadRequest("Failed to set main photo.");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("You cannot delete you main photo.");

            if(photo.PublicId != null)
            {
               var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if(await _userRepository.SaveAllAsync())
                return Ok();

            return BadRequest("Failed to delete photo.");
        }
    }
}