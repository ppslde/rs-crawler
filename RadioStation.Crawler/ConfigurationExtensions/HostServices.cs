using Microsoft.Extensions.DependencyInjection;
using RadioStation.Crawler.HostedServices;

namespace RadioStation.Crawler.ConfigurationExtensions {
  public static class HostServices {

    public static IServiceCollection AddHostedServices(this IServiceCollection services) {
      services.AddHostedService<PlaylistCrawlingService>();
      services.AddHostedService<TrackTaggingService>();
      return services;
    }
  }
}
