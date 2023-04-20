using Microsoft.EntityFrameworkCore;

namespace ModularArchitecture.Identity.Core
{
    public class RefreshTokenStore
    {
        private readonly DbSet<RefreshToken> _tokens;
        private readonly IdentityContext _context;

        public RefreshTokenStore(IdentityContext context)
        {
            _context = context;
            _tokens = context.Set<RefreshToken>();
        }

        public async Task<bool> AddRefreshTokenAsync(RefreshToken token)
        {
            //var existingToken = _tokens.SingleOrDefault(r => r.Subject == token.Subject && r.ClientId == token.ClientId);

            //if (existingToken != null)
            //{
            //    _tokens.Remove(existingToken);
            //}

            await _tokens.AddAsync(token);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshTokenAsync(string refreshTokenId)
        {
            var refreshToken = await _tokens.FindAsync(refreshTokenId);

            if (refreshToken == null)
                return false;

            _tokens.Remove(refreshToken);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _tokens.Remove(refreshToken);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshTokenAsync(string refreshTokenId)
        {
            var refreshToken = await _tokens.FindAsync(refreshTokenId);
            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _tokens.ToList();
        }

        public async Task RemoveOldTokensAsync(string username)
        {
            _tokens.RemoveRange(_tokens.Where(x => x.Subject == username && x.ExpiresUtc < DateTime.UtcNow));
            await _context.SaveChangesAsync();
        }
    }
}