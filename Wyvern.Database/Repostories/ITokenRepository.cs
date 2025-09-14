using Wyvern.Database.Data;

namespace Wyvern.Database.Repositories
{
    public interface ITokenRepository
    {
        Task<Token> CreateAsync(Token token);
        Task<Token?> GetByTokenAsync(string token);
        Task<IEnumerable<Token>> GetByUserIdAsync(string userId);
        Task MarkAsUsedAsync(Token token);
        Task SaveChangesAsync();
    }
}
