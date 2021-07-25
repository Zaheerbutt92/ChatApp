using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.ViewModels;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberViewModel>> GetMembersAsync();
        Task<MemberViewModel> GetMemberByUsernameAsync(string username);
        Task<MemberViewModel> GetMemberByIdAsync(int Id);
    }
}