using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ModularArchitecture.Identity.Core.Result;

namespace ModularArchitecture.Identity.Core
{
    public interface ITwoFactorRegistrationService
    {
        Task<IGenerateRegistrationCodeResult> GenerateRegistrationCodeAsync(string cell);
        Task<IIdentityResult> VerifyRegistrationCodeAsync(IVerifyRegistrationCodeModel model, bool preventRemoveCode = false);
    }

    public class TwoFactorRegistrationService : ITwoFactorRegistrationService
    {
        private readonly DbSet<CellToken> _tokens;
        private readonly IdentityContext _context;
        private readonly IStringLocalizer _localizer;

        public TwoFactorRegistrationService(IdentityContext context, IStringLocalizer localizer)
        {
            _context = context;
            _localizer = localizer;
            _tokens = context.Set<CellToken>();
        }

        public async Task<IGenerateRegistrationCodeResult> GenerateRegistrationCodeAsync(string cell)
        {
            if (string.IsNullOrEmpty(cell))
            {
                return new GenerateRegistrationCodeResult { Message = _localizer["Cell number should be sent."] };
            }
            Random random = new Random();
            object syncLock = new object();
            int code;
            lock (syncLock)
            {
                // synchronize
                code = random.Next(1000, 9999);
            }

            var token = new CellToken(cell, code);
            var existingToken = _tokens.FirstOrDefault(r => r.Cell == token.Cell);

            if (existingToken != null)
            {
                _tokens.Remove(existingToken);
            }

            await _tokens.AddAsync(token);

            await _context.SaveChangesAsync();
            return new GenerateRegistrationCodeResult { Success = true, Code = code, Cell = cell };
        }

        public async Task<IIdentityResult> VerifyRegistrationCodeAsync(IVerifyRegistrationCodeModel model, bool preventRemoveCode = false)
        {
            var cellToken = _tokens.SingleOrDefault(r => r.Cell == model.Cell || r.Hashed == model.Cell);

            if (cellToken == null || cellToken.Code.ToString() != model.Code)
                return new IdentityResult { Message = _localizer["code is invalid"] };

            if (!preventRemoveCode)
                _tokens.Remove(cellToken);

            await _context.SaveChangesAsync();
            return new IdentityResult { Success = true };
        }
    }
}
