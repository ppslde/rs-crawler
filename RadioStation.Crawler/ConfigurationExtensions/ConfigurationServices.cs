using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace RadioStation.Crawler.ConfigurationExtensions {
  public interface ISecurityConfig {
    string Secret { get; set; }
    int ExpireDays { get; set; }
  }
  public class SecurityConfig : ISecurityConfig {
    public string Secret { get; set; }
    public int ExpireDays { get; set; }
  }

  public static class ConfigurationServices {
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration config) {
      services.Configure<SecurityConfig>(config.GetSection("SecuritySettings"));
      services.TryAddSingleton<ISecurityConfig>(sp => sp.GetRequiredService<IOptions<SecurityConfig>>().Value);
      return services;
    }
  }
}
