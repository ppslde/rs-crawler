using System.Threading.Tasks;

namespace RadioStation.Crawler.Core {
  public interface IUserService {
    Task<bool> ValidateUserAsync(string username, string pwd);
  }
}