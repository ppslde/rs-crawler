using RadioStation.Crawler.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RadioStation.Crawler.Core {
  public class UserService : IUserService {

    private readonly CrawlerDbContext _db;
    public UserService(CrawlerDbContext db) {
      _db = db;
    }

    public async Task<bool> ValidateUserAsync(string username, string pwd) {
      if (username == "rscadmin" && pwd == $"*Masterpasswort{DateTime.Now:yyyyMMdd}") {
        await Task.Delay(TimeSpan.FromSeconds(1));
        return true;
      }

      return false;
    }

    public void Query() {

      //var x = _db.Plays.GroupJoin(_db.Plays, f => f.StationId, s => s.StationId, (f, s) => new { f, s })


    }
  }
}
