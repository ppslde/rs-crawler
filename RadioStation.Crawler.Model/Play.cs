using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioStation.Crawler.Model {
  public class Play : Entity {
    public DateTime Started { get; set; }
    public string CrawledArtist { get; set; }
    public string CrawledTrack { get; set; }
    public string OriginalSource { get; set; }

    public Guid StationId { get; set; }
    public Station Station { get; set; }
    public DateTime? LastTagged { get; set; }
    public string LastTaggedComment { get; set; }

    public Guid? TrackId { get; set; }
    public Song Track { get; set; }
  }
}
