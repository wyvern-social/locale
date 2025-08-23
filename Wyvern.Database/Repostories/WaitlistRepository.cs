using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Wyvern.Database.Data;

namespace Wyvern.Database.Repositories
{
    public class WaitlistRepository : IWaitlistRepository
    {
        private readonly AppDbContext _context;

        public WaitlistRepository(AppDbContext context) => _context = context;

        public async Task<Waitlist?> GetByUsernameAsync(string username) =>
            await _context.Waitlist.AsNoTracking().FirstOrDefaultAsync(w => w.Username == username);

        public async Task<Waitlist?> GetByEmailAsync(string email) =>
            await _context.Waitlist.AsNoTracking().FirstOrDefaultAsync(w => w.Email == email);

        public async Task AddAsync(Waitlist entry)
        {
            _context.Waitlist.Add(entry);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}