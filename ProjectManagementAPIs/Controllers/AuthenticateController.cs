using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementAPIs.Data;
using ProjectManagementAPIs.Dto;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagementAPIs.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticateController(DataContext context, IConfiguration configuration, IRefreshTokenGenerator refreshTokenGenerator, IPasswordHasher passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _refreshTokenGenerator = refreshTokenGenerator;
            _passwordHasher = passwordHasher;
        }
        [AllowAnonymous]
        [NonAction]
        public async Task<TokenResponse> TokenAuthenticate(string memberUserEmail, Claim[] claims)
        {
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])), SecurityAlgorithms.HmacSha256)
                );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponse() { AccessToken = accessToken, RefreshToken = await _refreshTokenGenerator.GenerateToken(memberUserEmail) };
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginModelDto loginModel)
        {
            var user = await _context.MemberUsers.FirstOrDefaultAsync(x => x.MemberUserEmail == loginModel.MemberUserEmail);

            if (user == null || !_passwordHasher.Verify((user.passwordSalt + ':' + user.passwordHash), loginModel.MemberUserPassword))
            {
                return Unauthorized();
            }

            // Generate Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                new Claim(ClaimTypes.Name, user.MemberUserEmail),
                new Claim(ClaimTypes.Role, user.RoleId),
                new Claim(ClaimTypes.NameIdentifier, user.MemberUserId),
                new Claim(ClaimTypes.Email, user.Firstname),
                new Claim(ClaimTypes.GivenName, user.Lastname)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDesc);
            string finalToken = tokenHandler.WriteToken(token);

            var response = new TokenResponse()
            {
                AccessToken = finalToken,
                RefreshToken = await _refreshTokenGenerator.GenerateToken(loginModel.MemberUserEmail)
            };

            return Ok(response);
        }




        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenResponse tokenResponse)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(tokenResponse.AccessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false, //
                ValidateAudience = false, //
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),

            }, out securityToken);

            var token = securityToken as JwtSecurityToken;
            if (token != null && !token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                return Unauthorized();
            }
            var memberUserEmail = principal.Identity?.Name;
            var user = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.MemberUserEmail == memberUserEmail && x.RefreshToken == tokenResponse.RefreshToken);
            if (user == null)
            {
                return Unauthorized();
            }

            var response = TokenAuthenticate(memberUserEmail, principal.Claims.ToArray()).Result;

            return Ok(response);


        }
    }
}
