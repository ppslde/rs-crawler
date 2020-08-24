using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RadioStation.Crawler.Database;
using RadioStation.Crawler.ViewModels;

namespace RadioStation.Crawler.Controllers {
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class CrawlingDashboardController : ControllerBase {

    private readonly CrawlerDbContext _db;
    public CrawlingDashboardController(CrawlerDbContext db) {
      _db = db;
    }

    [HttpGet("nevercrawled/{command?}")]
    public IActionResult GetNeverCrawledPlays(string command) {

      if (command == "items") {
        var plays = _db.Plays.Where(p => p.LastTagged == null && p.TrackId == null)
        .Select(p => new { p.Id, p.StationId, p.CrawledArtist, p.CrawledTrack });
        return Ok(new ResponseModel { Message = "", Data = plays });
      }
      if (command == "count") {
        var playscount = _db.Plays.Where(p => p.LastTagged == null && p.TrackId == null).Count();
        return Ok(new ResponseModel { Message = "", Data = playscount });
      }

      return BadRequest(new ResponseModel { Message = $"No valid request url parameter provided: {command}" });
    }


  }
}