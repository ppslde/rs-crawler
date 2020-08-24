using System.Threading.Tasks;

namespace RadioStation.Crawler.Authentication {
  public interface IAuthService {
    Task<string> AuthenticateAsync(string username, string password);
  }
}