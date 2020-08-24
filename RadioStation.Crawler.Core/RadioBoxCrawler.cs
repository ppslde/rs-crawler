using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AngleSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RadioStation.Crawler.Database;
using RadioStation.Crawler.Model;

namespace RadioStation.Crawler.Core {
  public class RadioBoxCrawler : IStationCrawler {

    private readonly IServiceProvider _serviceProvider;
    public RadioBoxCrawler(IServiceProvider provider) {
      _serviceProvider = provider;
    }

    public async Task ExecuteSingleRunAsync(CancellationToken ct) {

      var msgChannel = Channel.CreateUnbounded<Play>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

      _ = Task.Run(() => CrawlStationsAsync(msgChannel.Writer, ct));


      using var scope = _serviceProvider.CreateScope();
      var db = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();

      await foreach (var playedItem in msgChannel.Reader.ReadAllAsync()) {
        //await StorePlayedSong(playedItem, ct);
        await db.Plays.AddAsync(playedItem, ct);
      }

      await db.SaveChangesAsync(ct);
    }

    private async Task StorePlayedSong(Play item, CancellationToken ct) {
      using var scope = _serviceProvider.CreateScope();
      var db = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();

      try {
        await db.Plays.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
      } catch (Exception ex) {

      }
    }

    private async Task CrawlStationsAsync(ChannelWriter<Play> msgChannelWriter, CancellationToken ct) {

      using var scope = _serviceProvider.CreateScope();

      var db = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
      
      foreach (var s in await db.Stations.AsNoTracking().ToArrayAsync(ct)) {
        var browseCtx = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        var l = await db.Plays.Where(p => p.StationId.Equals(s.Id)).Select(p => p.Started).ToListAsync(ct);
        var lastcrawldate = l.DefaultIfEmpty(DateTime.MinValue.Date).Max();
        if (lastcrawldate < DateTime.Today.AddDays(-1)) {
          var x = (DateTime.Today.AddDays(-1) - lastcrawldate.Date).TotalDays;
          x = x > 6 ? 6 : x;
          for (int i = 1; i <= x; i++) {
            var targethtml = await browseCtx.OpenAsync($"{s.PlaylistUrl}/playlist/{i}?useStationLocation=1", ct);
            foreach (var p in targethtml.QuerySelectorAll(".tablelist-schedule tr")) {
              var played = new Play {
                StationId = s.Id,
                OriginalSource = p.OuterHtml,
                Started = DateTime.Today.AddDays(i * -1).Add(TimeSpan.Parse(p.QuerySelector("td:nth-child(1)").TextContent))
              };
              var artisttitle = p.QuerySelector("td:nth-child(2)").TextContent.Split(" : ", 2);
              played.CrawledArtist = artisttitle[0].Trim();
              played.CrawledTrack = artisttitle[1].Replace("NEU:", "").Trim();

              await msgChannelWriter.WriteAsync(played);
            }
          }
        }
      }
      msgChannelWriter.Complete();
    }
  }
}
