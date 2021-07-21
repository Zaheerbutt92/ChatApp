using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using API.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberViewModel> GetMemberByIdAsync(int Id)
        {
            return await _context.Users
                    .Where(x=>x.Id == Id)
                    .ProjectTo<MemberViewModel>(_mapper.ConfigurationProvider) 
                    .SingleOrDefaultAsync();
        }

        public async Task<MemberViewModel> GetMemberByUsernameAsync(string username)
        {
            return await _context.Users
                    .Where(x=>x.UserName == username)
                    .ProjectTo<MemberViewModel>(_mapper.ConfigurationProvider) 
                    .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberViewModel>> GetMembersAsync()
        {
             return await _context.Users
                    .ProjectTo<MemberViewModel>(_mapper.ConfigurationProvider) 
                    .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p=>p.Photos)
            .SingleOrDefaultAsync(x=>x.UserName == username.ToLower());
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p=>p.Photos)
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;            
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}