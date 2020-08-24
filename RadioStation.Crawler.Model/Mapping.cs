using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RadioStation.Crawler.Model {

  public enum MappingType {
    Artist,
    Song
  }

  public class Mapping : Entity {
    public string Type { get; set; }
    public string Text { get; set; }
    public string Replacement { get; set; }
  }
}
