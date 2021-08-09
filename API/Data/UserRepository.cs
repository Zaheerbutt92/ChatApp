using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
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

        public async Task<PagedList<MemberViewModel>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge -1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <=maxDob);

            query = userParams.OrderBy switch {
                "created" => query.OrderByDescending(u =>u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberViewModel>.CreateAsync(query.ProjectTo<MemberViewModel>(_mapper
            .ConfigurationProvider).AsNoTracking(),
            userParams.PageNumber, userParams.PageSize);
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