using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadioStation.Crawler.Database;
using RadioStation.Crawler.ViewModels;

namespace RadioStation.Crawler.Controllers {
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class MetricsController : ControllerBase {

    private readonly CrawlerDbContext _db;
    public MetricsController(CrawlerDbContext db) {
      _db = db;
    }

    [HttpGet("full")]
    public async Task<IActionResult> FullMetrics() {

      var d = new {
        PlaysCount = await _db.Plays.CountAsync(),
        TaggedCount = await _db.Plays.CountAsync(p => p.TrackId != null),
        ExceptionsCount = await _db.Plays.CountAsync(p => p.TrackId == null && p.LastTagged != null && p.LastTaggedComment.StartsWith("EXCEPTION")),
        UntagableCount = await _db.Plays.CountAsync(p => p.TrackId == null && p.LastTagged != null && p.LastTaggedComment.StartsWith("No")),
        NeverTaggedCount = await _db.Plays.CountAsync(p => p.TrackId == null && p.LastTagged == null),
        CrawlStatus = await _db.Plays.GroupBy(p => new { p.StationId, p.Started.Date })
                         .OrderBy(g => g.Key.StationId)
                         .ThenBy(g => g.Key.Date)
                         .Select(g => new {
                           g.Key.StationId,
                           g.Key.Date,
                           FirstStart = g.Min(p => p.Started),
                           LastStart = g.Max(p => p.Started),
                           Count = g.Count()
                         }).AsNoTracking().ToListAsync()
      };

      return Ok(new ResponseModel {
        Message = "",
        Data = d
      });
    }

    [HttpGet("untagged")]
    public async Task<IActionResult> UntaggedMetrics() {

      var d = await _db.Plays.Where(p => p.LastTagged != null && p.TrackId == null)
                 .GroupBy(p => new { p.CrawledArtist, p.CrawledTrack })
                 .OrderBy(g => g.Key.CrawledArtist)
                 .Select(g => new {
                   artist = g.Key.CrawledArtist,
                   track = g.Key.CrawledTrack,
                   count = g.Count()
                 }).AsNoTracking().ToListAsync();

      return Ok(new ResponseModel {
        Message = "",
        Data = d
      });
    }
  }
}