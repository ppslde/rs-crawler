using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RadioStation.Crawler.MetaTagger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RadioStation.Crawler.HostedServices {
  public class TrackTaggingService : BackgroundService {

    private readonly IServiceProvider _serviceProvider;
    public TrackTaggingService(IServiceProvider provider) {
      _serviceProvider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      while (!stoppingToken.IsCancellationRequested) {
        await RunTaggerAsync(100, stoppingToken);
        await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
      }
    }

    private async Task RunTaggerAsync(int count, CancellationToken ct) {
      using var scope = _serviceProvider.CreateScope();
      await scope.ServiceProvider.GetRequiredService<IMetaTagger>().TagSongsAsync(ct);
    }
  }
}
