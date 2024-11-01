using DatingApp.Helper;
using DatingApp.Models;

namespace DatingApp.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<PagedList<User>>GetUsers(UserParams userParams);
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
        Task<Like> GetLike(int userId, int recipientId);
        Task<Message>GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParam);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);


    }
}
