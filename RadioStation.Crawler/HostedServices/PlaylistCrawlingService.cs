using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RadioStation.Crawler.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RadioStation.Crawler.HostedServices {
  public class PlaylistCrawlingService : BackgroundService {

    private readonly IServiceProvider _serviceProvider;
    public PlaylistCrawlingService(IServiceProvider provider) {
      _serviceProvider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      while (!stoppingToken.IsCancellationRequested) {
        await RunCrawler(stoppingToken);
        await Task.Delay(TimeSpan.FromSeconds((DateTime.Today.AddDays(1).AddHours(1) - DateTime.Now).TotalSeconds), stoppingToken);
      }
    }

    private async Task RunCrawler(CancellationToken ct) {
      using var scope = _serviceProvider.CreateScope();
      await scope.ServiceProvider.GetRequiredService<IStationCrawler>().ExecuteSingleRunAsync(ct);
    }
  }
}
