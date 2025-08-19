using Wyvern.Database.Data;

namespace Wyvern.Database.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task SaveChangesAsync();
    }
}
