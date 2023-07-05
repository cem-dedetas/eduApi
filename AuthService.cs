using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WebApplication1.Models;

namespace EduAPI
{
	public class AuthService
	{
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public void CreatePasswordHash(string password, out byte[] passHash, out byte[] passSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passSalt = hmac.Key;
                passHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passHash, byte[] passSalt)
        {
            using (var hmac = new HMACSHA512(passSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passHash);
            }
        }

        public string CreateToken(User _account)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("_id", _account.id.ToString()),
                new Claim("username", _account.username),
                new Claim("email", _account.email),
                new Claim("idNumber", _account.id_number)
            };
            



            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtSettings:SecretKey").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.Now.AddMonths(3),
                    signingCredentials: creds,
                    audience:"aud",
                    issuer:"edubackendapi"
                 

                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task<User> getUserFromToken(DataContext db, string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtSettings:SecretKey").Value));
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = "edubackendapi",
                ValidateAudience = true,
                ValidAudience = "aud",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            var claims = handler.ValidateToken(token, validations, out var tokenSecure);
            
            var user = await db.Users.FirstOrDefaultAsync(x => x.id == int.Parse(claims.Claims.First().Value));
            
            return user;

        }
    }
}

