using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace RadioStation.Crawler.Model {
  public class Artist : Entity {
    public string Title { get; set; }
    public Guid MusicBrainzId { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Established { get; set; }
    public string SplitUp { get; set; }
    public string TopGenre { get; set; }
    public string Alias { get; set; }
    public string Genres { get; set; }
    public string Tags { get; set; }

    public ICollection<Song> Songs { get; set; }
  }
}
