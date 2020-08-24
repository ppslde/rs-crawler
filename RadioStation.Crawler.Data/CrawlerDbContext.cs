using Microsoft.EntityFrameworkCore;
using RadioStation.Crawler.Model;
using System;

namespace RadioStation.Crawler.Database {
  public class CrawlerDbContext : DbContext {

    public CrawlerDbContext(DbContextOptions<CrawlerDbContext> options) : base(options) { }

    public DbSet<Artist> Artists { get; set; }
    public DbSet<Play> Plays { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Mapping> Mappings { get; set; }
  }
}
