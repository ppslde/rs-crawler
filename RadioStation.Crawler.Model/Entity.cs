using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioStation.Crawler.Model {
  public abstract class Entity {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; }
    public DateTime Added { get; set; } = DateTime.Now;
  }
}
