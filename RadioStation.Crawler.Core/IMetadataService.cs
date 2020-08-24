using RadioStation.Crawler.Model;
using System;
using System.Threading.Tasks;

namespace RadioStation.Crawler.Core {
  [Obsolete("NOT USED", true)]
  public interface IMetadataService {
    Task<(Artist Artist, Song Song)> QueryArtistSongAsync(string artist, string title);
    Task<Artist> QueryArtistAsync(string artistname);
    Task<Song> QuerySongAsync(Artist artist, string songtitle);
    Task<Song> QuerySongDeepAsync(Artist artist, string songtitle, string originaltitle);
  }
}
