using DatingApp.Models;

namespace DatingApp.Interface
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> LogIn(string userName, string password);
        Task<bool> UserExists (string userName);
    }
}
