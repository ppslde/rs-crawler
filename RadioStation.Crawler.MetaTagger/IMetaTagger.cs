using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RadioStation.Crawler.MetaTagger {
  public interface IMetaTagger {
    Task<(string url, IEnumerable<object> songs)> QuerySongAndArtistSingle(string artist, string song, CancellationToken ct);
    Task TagSongsAsync(CancellationToken ct);
  }
}