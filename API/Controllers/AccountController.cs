using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;
using API.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserViewModel>> Register(RegisterViewModel registerVM)
        {
            if (await UserExists(registerVM.Username))
                return BadRequest("Username is not available");

            var user = _mapper.Map<AppUser>(registerVM);

            user.UserName = registerVM.Username.ToLower();

            var userResult = await _userManager.CreateAsync(user, registerVM.Password);

            if(!userResult.Succeeded) BadRequest(userResult.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user,"Member");
            if(!roleResult.Succeeded) BadRequest(roleResult.Errors);

            var singinResult = await _signInManager.CheckPasswordSignInAsync(user,registerVM.Password,false);
            if(!singinResult.Succeeded) BadRequest("Something went wrong.");
            
            return new UserViewModel{
                Username = user.UserName,
                Token =await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserViewModel>> Login(LoginViewModel loginVM)
        {
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginVM.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user,loginVM.Password,false);

            if(!result.Succeeded) Unauthorized();

            return new UserViewModel{
                Username = user.UserName,
                Token =await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}