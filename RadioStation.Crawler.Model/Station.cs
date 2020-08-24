using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioStation.Crawler.Model {
  public class Station : Entity {
    public string Name { get; set; }
    public string Title { get; set; }
    public string PlaylistUrl { get; set; }
    public ICollection<Play> Plays { get; set; }
  }
}
