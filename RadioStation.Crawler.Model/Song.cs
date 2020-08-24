using System;
using System.Collections.Generic;

namespace RadioStation.Crawler.Model {
  public class Song : Entity {
    public string Title { get; set; }
    public int FoundTracks { get; set; }
    public string FirstRelease { get; set; }
    public TimeSpan Duration { get; set; }
    public TimeSpan DurationMax { get; set; }
    public TimeSpan DurationMin { get; set; }
    public TimeSpan DurationAvg { get; set; }

    public Guid ArtistId { get; set; }
    public Artist Artist { get; set; }

    public double? TaggingScore { get; set; }
    public string TaggingHint { get; set; }

    public ICollection<Play> Plays { get; set; }
    public string FoundTitles { get; set; }
  }
}
