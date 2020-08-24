using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using RadioStation.Crawler.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RadioStation.Crawler.Core {
  [Obsolete("NOT USED", true)]
  public class MusicBrainzService : IMetadataService {

    private readonly Query _query = new Query("TestApp", "0.1", "nocontact@mailtomail.org");

    public async Task<(Artist Artist, Song Song)> QueryArtistSongAsync(string artistname, string songtitle) {
      var artist = await QueryArtistAsync(artistname);
      return (artist, await QuerySongAsync(artist, songtitle));
    }

    public async Task<Artist> QueryArtistAsync(string artistname) {
      var queryartist = artistname.ToLower().Replace(" ", " AND ");
      var selectedartist = (await _query.FindArtistsAsync($"{queryartist}", 3, simple: true)).Results/*.Where(r => r.Score == 100)*/.FirstOrDefault();

      if (selectedartist == null) {
        return null;
      }

      var lookartist = await _query.LookupArtistAsync(selectedartist.Item.Id, Include.Aliases | Include.Genres | Include.Tags);

      return new Artist {
        Title = artistname,
        MusicBrainzId = lookartist.Id,
        //Score = selectedartist.Score,
        Country = lookartist.Country,
        City = lookartist.BeginArea?.Name,
        Alias = string.Join(",", lookartist.Aliases.Select(t => t.Name)),
        Tags = string.Join(",", lookartist.Tags.OrderBy(t => t.VoteCount).Select(t => $"{t.Name}[{t.VoteCount}]")),
        Genres = string.Join(",", lookartist.Genres.OrderBy(t => t.VoteCount).Select(t => $"{t.Name}[{t.VoteCount}]"))
      };
    }

    public async Task<Song> QuerySongAsync(Artist artist, string songtitle) {

      if (artist == null) {
        return null;
      }

      var releases = await _query.BrowseArtistReleasesAsync(artist.MusicBrainzId, limit: 50, inc: Include.Recordings);
      var tracks = releases.Results.SelectMany(r => r.Media?.SelectMany(m => m.Tracks ?? null)).Where(t => t.Title.ToLower() == songtitle.ToLower());

      foreach (var r in releases.Results) {
        Debug.WriteLine($"{r.Title} - {r.Media?.Count} - {r.Media.SelectMany(m => string.Join(",", m.Tracks))}");
      }

      if (!tracks.Any()) {
        return null;
      }

      return new Song {
        Title = songtitle,
        FoundTracks = tracks.Count(),
        //MusicBrainzId = tracks.GroupBy(t => t.Length).OrderByDescending(g => g.Count()).First().First().Id,
        Duration = tracks.GroupBy(t => t.Length).OrderByDescending(g => g.Count()).First().Key ?? TimeSpan.Zero,
        DurationMin = tracks.Min(t => t.Length) ?? TimeSpan.Zero,
        DurationMax = tracks.Max(t => t.Length) ?? TimeSpan.Zero,
        DurationAvg = TimeSpan.FromSeconds(tracks.Average(t => (t.Length ?? TimeSpan.Zero).TotalSeconds))
      };
    }

    public async Task<Song> QuerySongDeepAsync(Artist artist, string songtitle, string originaltitle) {

      if (artist == null) {
        return null;
      }
      var allreleases = new List<IRelease>();
      var pageidx = 0;
      var pagesize = 50;
      var releases = await _query.BrowseArtistReleasesAsync(artist.MusicBrainzId, limit: pagesize, offset: pageidx, inc: Include.Recordings);
      allreleases.AddRange(releases.Results);

      while (releases.Results.Count > 0) {
        pageidx++;
        releases = await _query.BrowseArtistReleasesAsync(artist.MusicBrainzId, limit: pagesize, offset: pageidx * pagesize, inc: Include.Recordings);
        allreleases.AddRange(releases.Results);
      }

      var tracks = allreleases.SelectMany(r => r.Media.SelectMany(m => m.Tracks)).Where(t => t.Title.ToLower() == songtitle.ToLower() || t.Title.ToLower() == originaltitle.ToLower());

      if (!tracks.Any()) {
        return null;
      }

      return new Song {
        Title = tracks.GroupBy(t => t.Length).OrderByDescending(g => g.Count()).First().First().Title,
        FoundTracks = tracks.Count(),
        //MusicBrainzId = tracks.GroupBy(t => t.Length).OrderByDescending(g => g.Count()).First().First().Id,
        Duration = tracks.GroupBy(t => t.Length).OrderByDescending(g => g.Count()).First().Key ?? TimeSpan.Zero,
        DurationMin = tracks.Min(t => t.Length) ?? TimeSpan.Zero,
        DurationMax = tracks.Max(t => t.Length) ?? TimeSpan.Zero,
        DurationAvg = TimeSpan.FromSeconds(tracks.Average(t => (t.Length ?? TimeSpan.Zero).TotalSeconds))
      };
    }
  }
}
