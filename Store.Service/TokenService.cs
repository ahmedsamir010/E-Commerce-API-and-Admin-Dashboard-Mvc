using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Store.Core.Entities.Identity;
using Store.Core.Services;
using Store.Core.Entities;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace Store.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TokenService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this.config = config;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> CreateTokenAsync(AppUser appUser, UserManager<AppUser> userManager)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, appUser.Email),
                new Claim("firstName", appUser.FirstName),
                new Claim("lastName", appUser.LastName)
            };
            var userRoles = await userManager.GetRolesAsync(appUser);
            foreach (var role in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(double.Parse(config["Jwt:DurationInDays"])),
                Issuer = config["Jwt:Issuer"],
                Audience = config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encrypterToken = tokenHandler.WriteToken(token);



           return encrypterToken;
        }

   
   
    }
}
