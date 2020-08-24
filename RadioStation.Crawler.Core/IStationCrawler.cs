using RadioStation.Crawler.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RadioStation.Crawler.Core {
  public interface IStationCrawler {
    Task ExecuteSingleRunAsync(CancellationToken ct);
  }
}