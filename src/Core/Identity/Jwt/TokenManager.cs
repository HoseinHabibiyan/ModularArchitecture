using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ModularArchitecture.Identity.Core
{
    public class TokenManager
    {
        public string GenerateJwtToken(ClaimsIdentity claimsIdentity, string clientId, string clientSecret, in int clientTokenLifeTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(clientSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Subject,
                Expires = DateTime.UtcNow.AddMinutes(clientTokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = clientId,
                //Claims = claimsIdentity.Claims.ToDictionary(x => x.Type, x => (object)x.Value),
                IssuedAt = DateTime.UtcNow
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(ClaimsIdentity claimsIdentity, string id, string clientId,
            int refreshTokenLifeTime)
        {
            var token = new RefreshToken
            {
                Id = RandomTokenString(id),
                ClientId = clientId,
                Subject = claimsIdentity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };
            return token;
        }

        public string RandomTokenString(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

    }
}
