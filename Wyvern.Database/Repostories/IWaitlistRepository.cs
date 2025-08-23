using System.Threading.Tasks;
using Wyvern.Database.Data;

namespace Wyvern.Database.Repositories
{
    public interface IWaitlistRepository
    {
        Task<Waitlist?> GetByUsernameAsync(string username);
        Task<Waitlist?> GetByEmailAsync(string email);
        Task AddAsync(Waitlist entry);
        Task SaveChangesAsync();
    }
}