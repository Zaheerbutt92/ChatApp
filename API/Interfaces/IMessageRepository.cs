using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.ViewModels;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int Id);
        Task<PagedList<MessageViewModel>> GetMessagesForUser(MessageParams messageParams); 
        Task<IEnumerable<MessageViewModel>> GetMessageThread(string currentUsername, string RecipientUsername);
        Task<bool> SaveAllAsync();
    }
}