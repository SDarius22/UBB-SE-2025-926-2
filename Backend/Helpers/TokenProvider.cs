namespace Backend.Helpers
{

    using System;
    using System.Text;
    using System.Security.Claims;
    using System.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using Backend.Configs;
    using Backend.Models;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;

    internal sealed class TokenProvider
    {
        public string Create(UserModel user)
        {
            string secretKey = ApplicationConfiguration.GetInstance().JwtSecret;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                ]),
                Expires = DateTime.UtcNow.AddMinutes(ApplicationConfiguration.GetInstance().TokenExpirationMinutes),
                SigningCredentials = credentials,
                //Issuer
                //Audience
            };

            var handler = new JsonWebTokenHandler();

            var token = handler.CreateToken(tokenDescriptor);

            return token;
        }
    }
}
