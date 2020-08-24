using System;
using System.Threading;
using System.Threading.Tasks;

namespace RadioStation.Crawler.Core {
  [Obsolete("NOT USED", true)]
  public interface IMetaDataTagger {
    Task TagAllTracks(CancellationToken ct);
    Task TagTracksBalanced(int count, CancellationToken ct);
  }
}