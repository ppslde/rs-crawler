using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RadioStation.Crawler.Authentication;

namespace RadioStation.Crawler.ConfigurationExtensions {
  public static class AuthenticationServices {
    public static IServiceCollection AddBearerAuthConfiguration(this IServiceCollection services, IConfiguration config) {

      services.TryAddScoped<IAuthService, AuthService>();

      var key = Encoding.ASCII.GetBytes(config.GetSection("SecuritySettings:Secret").Value);

      services.AddAuthentication(x => {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(x => {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false
        };
      });
      return services;
    }
  }
}
