using Microsoft.IdentityModel.Tokens;
using RadioStation.Crawler.ConfigurationExtensions;
using RadioStation.Crawler.Core;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RadioStation.Crawler.Authentication {
  public class AuthService : IAuthService {

    private readonly ISecurityConfig _secureConfig;
    private readonly IUserService _userSrv;

    public AuthService(IUserService userSrv, ISecurityConfig securityConfig) {
      _userSrv = userSrv;
      _secureConfig = securityConfig;
    }

    public async Task<string> AuthenticateAsync(string username, string password) {

      // return null if user not found
      if (!(await _userSrv.ValidateUserAsync(username, password)))
        throw new Exception("User not found");

      // authentication successful so generate jwt token
      var key = Encoding.ASCII.GetBytes(_secureConfig.Secret);
      var tokenDescriptor = new SecurityTokenDescriptor {
        Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "admin") }),
        Expires = DateTime.UtcNow.AddDays(_secureConfig.ExpireDays),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }
  }
}
