using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RadioStation.Crawler.Authentication;
using RadioStation.Crawler.ViewModels;

namespace RadioStation.Crawler.Controllers {

  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase {

    private readonly IAuthService _authSrv;
    private readonly ILogger<UserController> _logger;

    public UserController(IAuthService authSrv, ILogger<UserController> logger) {
      _authSrv = authSrv;
      _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> AuthenticateAsync([FromBody]AuthModel model) {
      try {
        var token = await _authSrv.AuthenticateAsync(model.username, model.password);
        if (string.IsNullOrEmpty(token))
          return BadRequest(new { message = "Username or password is incorrect" });
        return Ok(new { token });
      } catch (Exception ex) {
        _logger.LogError("Authentication failed", ex);
        return BadRequest(new { message = "Authentication failed" }); ;
      }
    }
  }
}