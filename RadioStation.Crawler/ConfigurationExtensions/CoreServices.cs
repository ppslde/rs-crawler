using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RadioStation.Crawler.Core;
using RadioStation.Crawler.MetaTagger;

namespace RadioStation.Crawler.ConfigurationExtensions {
  public static class CoreServices {
    public static IServiceCollection AddCoreServices(this IServiceCollection services) {
      services.TryAddScoped<IStationCrawler, RadioBoxCrawler>();
      services.TryAddScoped<IMetaTagger, MusicBrainzMetaTagger>();
      services.TryAddScoped<IUserService, UserService>();
      return services;
    }
  }
}
