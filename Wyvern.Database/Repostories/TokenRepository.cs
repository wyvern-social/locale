using Microsoft.EntityFrameworkCore;
using Wyvern.Database.Data;

namespace Wyvern.Database.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly AppDbContext _context;

        public TokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Token> CreateAsync(Token token)
        {
            _context.Tokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<Token?> GetByTokenAsync(string token) =>
            await _context.Tokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenValue == token && !t.Used && t.ExpiresAt > DateTime.UtcNow);

        public async Task<IEnumerable<Token>> GetByUserIdAsync(string userId) =>
            await _context.Tokens
                .Where(t => t.UserId == userId && !t.Used && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

        public async Task MarkAsUsedAsync(Token token)
        {
            token.Used = true;
            _context.Tokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
