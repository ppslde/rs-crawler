using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RadioStation.Crawler.Database;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RadioStation.Crawler.Core {
  [Obsolete("NOT USED", true)]
  public class MetaDataTagger : IMetaDataTagger {

    private readonly IServiceProvider _serviceProvider;
    public MetaDataTagger(IServiceProvider provider) {
      _serviceProvider = provider;
    }

    public async Task TagAllTracks(CancellationToken ct) {
      try {
        await UpdateMetaData(0, ct);
      } catch (Exception ex) {
        throw new Exception($"{nameof(TagAllTracks)} with errors:", ex);
      }
    }

    public async Task TagTracksBalanced(int count, CancellationToken ct) {
      try {
        await UpdateMetaData(count, ct);
      } catch (Exception ex) {
        throw new Exception($"{nameof(TagTracksBalanced)} with errors:", ex);
      }
    }

    private string CleanTrackTitle(string title) {
      return Regex.Replace(Regex.Replace(title, @"\(.*\)", ""), @"\s+", " ").Trim();
    }

    private async Task UpdateMetaData(int count, CancellationToken ct) {

      using var scope = _serviceProvider.CreateScope();
      var db = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();
      //var songs = scope.ServiceProvider.GetRequiredService<IRepository<Song>>();
      //var artits = scope.ServiceProvider.GetRequiredService<IRepository<Artist>>();
      //var plays = scope.ServiceProvider.GetRequiredService<IRepository<Play>>();
      var metaSrv = scope.ServiceProvider.GetRequiredService<IMetadataService>();

      var playeditems = await db.Plays.Where(p => p.TrackId == null && p.LastTagged == null).ToListAsync(ct);
      foreach (var played in count == 0 ? playeditems : playeditems.Take(count)) {
        played.LastTagged = DateTime.Now;

        var tracktitle = CleanTrackTitle(played.CrawledTrack);
        var track = await db.Songs.SingleOrDefaultAsync(s => s.Title.ToLower() == tracktitle.ToLower() && s.Artist.Title.ToLower() == played.CrawledArtist.ToLower(), ct);
        if (track == null) {
          var artist = await db.Artists.SingleOrDefaultAsync(a => a.Title.ToLower() == played.CrawledArtist.ToLower(), ct);
          if (artist == null) {
            var artsong = await metaSrv.QueryArtistSongAsync(played.CrawledArtist, tracktitle);
            if (artsong.Song == null) {
              artsong.Song = await metaSrv.QuerySongDeepAsync(artsong.Artist, tracktitle, played.CrawledTrack);
              if (artsong.Song != null) {
                artsong.Song.Artist = artsong.Artist;
                played.Track = artsong.Song;
              }
            } else {
              artsong.Song.Artist = artsong.Artist;
              played.Track = artsong.Song;
            }
          } else {
            played.Track = await metaSrv.QuerySongAsync(artist, tracktitle);
            if (played.Track == null) {
              played.Track = await metaSrv.QuerySongDeepAsync(artist, tracktitle, played.CrawledTrack);
              if (played.Track != null) {
                played.Track.ArtistId = artist.Id;
              }
            } else {
              played.Track.ArtistId = artist.Id;
            }
          }
        } else {
          played.TrackId = track.Id;
        }
        db.Plays.Update(played);

      }
      await db.SaveChangesAsync(ct);
    }
  }
}