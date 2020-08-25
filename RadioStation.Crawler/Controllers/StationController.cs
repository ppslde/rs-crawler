using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadioStation.Crawler.Database;
using RadioStation.Crawler.Model;
using RadioStation.Crawler.ViewModels;

namespace RadioStation.Crawler.Controllers {
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class StationController : ControllerBase {

    private readonly CrawlerDbContext _db;
    public StationController(CrawlerDbContext dbctx) {
      _db = dbctx;
    }

    [HttpGet]
    public async Task<IActionResult> GetStationsAsync() {
      try {
        return Ok(new ResponseModel { Data = await _db.Stations.AsNoTracking().ToArrayAsync() });
      } catch (Exception ex) {
        return StatusCode(500, new ResponseModel { Message = "Problems while getting stations" });
      }
    }

    [HttpPost]
    public async Task<IActionResult> AddStation([FromBody]Station station) {

      if (station == null) {
        return BadRequest(new ResponseModel{ Message = "Missing station object" });
      }

      if (await _db.Stations.AnyAsync(s => s.PlaylistUrl.ToLower() == station.PlaylistUrl.ToLower())) {
        return BadRequest(new ResponseModel { Message = "PlaylistUrl already crawled" });
      }

      try {
        await _db.Stations.AddAsync(station);
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Message = "Station added", Data = station });
      } catch (Exception) {

        return StatusCode(500, new ResponseModel { Message = "Problems while adding new station" });
      }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStation([FromBody]Station station) {

      if (station == null) {
        return BadRequest(new ResponseModel { Message = "Missing station object" });
      }

      try {
        //_db.Stations.Append(s);
        _db.Stations.Update(station);
        await _db.SaveChangesAsync();
        return Ok(new ResponseModel { Message = "Station updated", Data = station });
      } catch (Exception) {
        return StatusCode(500, new ResponseModel { Message = "Problems while updating station" });
      }
    }

  }
}