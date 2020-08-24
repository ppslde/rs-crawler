using System.Threading;
using System.Threading.Tasks;

namespace RadioStation.Crawler.MetaTagger {
  public interface IMetaTagger {
    Task TagSongsAsync(CancellationToken ct);
  }
}