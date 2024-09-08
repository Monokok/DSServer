//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;


//namespace DS.Data
//{
//    //Сервис генерации токена для аутентифицированного пользователя
//    public class JwtService
//    {
//        private readonly string _secret;
//        private readonly string _issuer;
//        private readonly string _audience;
//        private readonly int _expiryMinutes;

//        public JwtService(IConfiguration config)
//        {
//            _secret = config["Jwt:Key"];
//            _issuer = config["Jwt:Issuer"];
//            _audience = config["Jwt:Audience"];
//            _expiryMinutes = int.Parse(config["Jwt:ExpiryMinutes"]);
//        }

//        public string GenerateToken(string userId, IList<string> roles)
//        {
//            var claims = new List<Claim>
//        {
//            new Claim(JwtRegisteredClaimNames.Sub, userId),
//            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
//        };

//            // Добавляем роли как claims
//            foreach (var role in roles)
//            {
//                claims.Add(new Claim(ClaimTypes.Role, role));
//            }

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: _issuer,
//                audience: _audience,
//                claims: claims,
//                expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
//                signingCredentials: creds);

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }

//}
