using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using backend.data;
using backend.dto.response;
using backend.entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend.service;

public class TokenService : ITokenService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public TokenService(
            UserManager<User> userManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<TokenResponse> GenerateTokens(User user)
        {
            // Generăm JWT token
            var accessToken = await GenerateAccessToken(user);
            
            // Generăm refresh token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7); // Valid 7 zile
            
            // Salvăm refresh token în baza de date
            var userRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = refreshTokenExpiry,
                IsRevoked = false
            };
            
            // Eliminăm orice token existent pentru acest utilizator
            var existingTokens = await _context.RefreshTokens
                .Where(t => t.UserId == user.Id)
                .ToListAsync();
                
            _context.RefreshTokens.RemoveRange(existingTokens);
            await _context.RefreshTokens.AddAsync(userRefreshToken);
            await _context.SaveChangesAsync();
            
            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpiryInMinutes"] ?? "60"))
            };
        }

        public async Task<TokenResponse> RefreshToken(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == refreshToken);
                
            if (storedToken == null)
                throw new SecurityTokenException("Invalid refresh token");
                
            if (storedToken.ExpiryDate < DateTime.UtcNow)
                throw new SecurityTokenException("Refresh token expired");
                
            if (storedToken.IsRevoked)
                throw new SecurityTokenException("Refresh token revoked");
                
            // Generăm un nou set de tokens
            var user = storedToken.User;
            return await GenerateTokens(user);
        }

        public async Task RevokeToken(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken);
                
            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        private async Task<string> GenerateAccessToken(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            
            // Adăugăm rolurile ca claims
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            
            // Adăugăm divizia ca claim (dacă există)
            if (user.DivisionId.HasValue)
            {
                authClaims.Add(new Claim("DivisionId", user.DivisionId.Value.ToString()));
            }
            
            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpiryInMinutes"] ?? "60")),
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }