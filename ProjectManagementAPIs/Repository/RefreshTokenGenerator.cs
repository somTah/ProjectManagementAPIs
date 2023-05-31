using Microsoft.EntityFrameworkCore;
using ProjectManagementAPIs.Data;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;
using System.Security.Cryptography;

namespace ProjectManagementAPIs.Repository
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly DataContext _context;

        public RefreshTokenGenerator(DataContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateToken(string memberUserEmail)
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);

                var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.MemberUserEmail == memberUserEmail);
                if (token != null)
                {
                    token.RefreshToken = refreshToken;
                }
                else
                {
                    _context.RefreshTokens.Add(new RefreshTokenModel()
                    {
                        RefreshTokenId = Guid.NewGuid().ToString(),
                        MemberUserEmail = memberUserEmail,
                        RefreshToken = refreshToken,
                        IsActive = true
                    });
                }
                await _context.SaveChangesAsync();

                return refreshToken;
            }
        }
    }
}
