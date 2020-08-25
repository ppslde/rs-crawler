using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RadioStation.Crawler.Database;
using RadioStation.Crawler.MetaTagger;
using RadioStation.Crawler.Model;
using RadioStation.Crawler.ViewModels;

namespace RadioStation.Crawler.Controllers {
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class TaggingController : ControllerBase {

    private readonly CrawlerDbContext _db;
    private readonly IMetaTagger _tagSrv;

    public TaggingController(CrawlerDbContext db, IMetaTagger tagSrv) {
      _db = db;
      _tagSrv = tagSrv;
    }

    [HttpGet("tagdata")]
    public async Task<IActionResult> GetTagData([FromQuery(Name = "artist")]string artist, [FromQuery(Name = "song")]string song) {
      var (url, content) = await _tagSrv.QuerySongAndArtistSingle(artist, song, CancellationToken.None);

      return Ok(new ResponseModel {
        Message = "",
        Data = new { url, records = content }
      });
    }

    [HttpGet("untagged")]
    public async Task<IActionResult> UntaggedSongs() {

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

    [HttpGet("mappings")]
    public async Task<IActionResult> Mappings() {

      var d = await _db.Mappings.Select(m => new {
        m.Id,
        m.Type,
        m.Text,
        m.Replacement
      }).AsNoTracking().ToListAsync();

      return Ok(new ResponseModel {
        Message = "",
        Data = d
      });
    }

    [HttpPost("mappings")]
    public async Task<IActionResult> AddStation([FromBody]Mapping map) {

      if (map == null) {
        return BadRequest(new ResponseModel { Message = "Missing mapping object" });
      }

      if (await _db.Mappings.AnyAsync(s => s.Text == map.Text)) {
        return BadRequest(new ResponseModel { Message = "Mapping already exists" });
      }

      try {
        await _db.Mappings.AddAsync(map);
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Message = "Mapping added", Data = map });
      }
      catch (Exception) {
        return StatusCode(500, new ResponseModel { Message = "Problems while adding new mapping" });
      }
    }

    [HttpPut("mappings")]
    public async Task<IActionResult> UpdateMapping([FromBody]Mapping map) {

      if (map == null) {
        return BadRequest(new ResponseModel { Message = "Missing mapping object" });
      }

      try {
        _db.Mappings.Update(map);
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Message = "Mapping updated", Data = map });
      }
      catch (Exception) {
        return StatusCode(500, new ResponseModel { Message = "Problems while updating mapping" });
      }
    }

  }
}