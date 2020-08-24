using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace RadioStation.Crawler.ConfigurationExtensions {
  public static class RepositoryServices {
    [Obsolete("Not used...", true)]
    public static IServiceCollection AddRepositories(this IServiceCollection services) {
      //services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));
      return services;
    }
  }
}
