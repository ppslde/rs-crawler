using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RadioStation.Crawler.Database;
using RadioStation.Crawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RadioStation.Crawler.MetaTagger {
  public class MusicBrainzMetaTagger : IMetaTagger {

    private readonly IServiceProvider _serviceProvider;

    private IEnumerable<Mapping> _artistMappings;
    private IEnumerable<Mapping> _songMappings;
    private IEnumerable<Mapping> _replaceMappings;

    public MusicBrainzMetaTagger(IServiceProvider provider) {
      _serviceProvider = provider;


    }

    public async Task TagSongsAsync(CancellationToken ct) {

      using var scope = _serviceProvider.CreateScope();
      var db = scope.ServiceProvider.GetRequiredService<CrawlerDbContext>();

      _artistMappings = await db.Mappings.Where(m => m.Type == "Artist").ToListAsync(ct);
      _replaceMappings = await db.Mappings.Where(m => m.Type == "Replace").ToListAsync(ct);
      _songMappings = await db.Mappings.Where(m => m.Type == "Song").ToListAsync(ct);

      foreach (var played in await db.Plays.Where(p => p.TrackId == null && p.LastTagged == null).ToListAsync(ct)) {
        try {

          //var worktitle = (await db.Mappings.SingleOrDefaultAsync(m => m.Text.ToLower() == played.CrawledTrack.ToLower() && m.Type == "Song", ct))?.Replacement ?? played.CrawledTrack;
          var workartist = _artistMappings.SingleOrDefault(m => m.Text.ToLower() == played.CrawledArtist.ToLower())?.Replacement ?? played.CrawledArtist;

          var song = await db.Songs.SingleOrDefaultAsync(s => s.Title.ToLower() == played.CrawledTrack.ToLower() && s.Artist.Title.ToLower() == workartist.ToLower(), ct);
          if (song == null) {
            var result = await SearchSongAsync(played.CrawledTrack, workartist, true, ct);
            if (result != (null, null)) {
              song = result.song;
              var artist = await db.Artists.SingleOrDefaultAsync(a => a.MusicBrainzId == Guid.Parse(result.artistId), ct);
              if (artist == null) {
                artist = await LoadArtistAsync(result.artistId, ct);
              }
              song.Artist = artist;
            } else {
              played.LastTaggedComment = "No suitable song artist combination found";
            }
          }
          played.Track = song;
        } catch (Exception ex) {
          played.LastTaggedComment = $"EXCEPTION: {ex.Message}";
        }
        played.LastTagged = DateTime.Now;
        db.Plays.Update(played);
        await db.SaveChangesAsync(ct);
        await Task.Delay(700);
      }
    }

    private async Task<XDocument> RequestData(string url, CancellationToken ct) {
      var retriecount = 0;
      using var http = new HttpClient();
      http.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36");
      var response = await http.GetAsync(url);
      while (!response.IsSuccessStatusCode && !ct.IsCancellationRequested) {
        if (retriecount == 10) {
          throw new Exception($"Request failed {retriecount} times: [code:{response.StatusCode};url:{url}]");
        }

        retriecount++;
        await Task.Delay(300);
        response = await http.GetAsync(url, ct);
      }

      var content = await response.Content.ReadAsStringAsync();
      return XDocument.Parse(content);
    }

    private string CleanTrackTitle(string title) {
      return Regex.Replace(Regex.Replace(title, @"\(.*\)", ""), @"\s+", " ").Trim();
    }

    private async Task<(string artistId, Song song)> SearchSongAsync(string song, string artist, bool retry, CancellationToken ct) {

      var worksong = song;
      foreach (var replace in _replaceMappings) {
        worksong = Regex.Replace(worksong, replace.Text, replace.Replacement).Trim();
      }

      var xdoc = await RequestData($"https://musicbrainz.org/ws/2/recording?query={worksong} AND artist:'{artist}'", ct);


      var nsmgr = new XmlNamespaceManager(new NameTable());
      nsmgr.AddNamespace("t", xdoc.Root.Name.Namespace.NamespaceName);
      nsmgr.AddNamespace("n", xdoc.Root.GetNamespaceOfPrefix("ns2").NamespaceName);

      //var xpath2candidates = "./t:recording-list/t:recording[number(@n:score) > 90]";
      //var xcandidates = xdoc.Root.XPathSelectElements(xpath2candidates, nsmgr);

      var xpath2artist = "./t:recording-list/t:recording[number(@n:score) > 90]/t:artist-credit/t:name-credit/t:artist";
      var artists = ((IEnumerable<object>)xdoc.Root.XPathSelectElements(xpath2artist, nsmgr)).Select(o => o).OfType<XElement>().Select(x => (x.Attribute("id").Value, x.XPathSelectElement("./t:name", nsmgr).Value)).Distinct();

      if (!artists.Any(a => a.Item2?.ToLower() == artist.ToLower())) {
        if (retry) {
          return await SearchSongAsync(CleanTrackTitle(worksong), artist, false, ct);
        }
        return (null, null);
      }

      var xpath2recording = "./t:recording-list/t:recording[number(@n:score) > 90]/@n:score";
      var recordscores = ((IEnumerable<object>)xdoc.Root.XPathEvaluate(xpath2recording, nsmgr)).Select(o => o).OfType<XAttribute>().Select(a => double.Parse(a.Value));

      var xpath2releasedate = "./t:recording-list/t:recording[number(@n:score) > 90]/t:release-list/t:release/t:date";
      var releaseyears = xdoc.Root.XPathSelectElements(xpath2releasedate, nsmgr).Select(x => x.Value.Split("-")[0]).OfType<string>().OrderBy(s => s).Where(s => !string.IsNullOrEmpty(s));

      var xpath2track = "./t:recording-list/t:recording[number(@n:score) > 90]/t:release-list/t:release/t:medium-list/t:medium/t:track-list/t:track";
      var xtracks = xdoc.Root.XPathSelectElements(xpath2track, nsmgr);

      var timespans = xtracks.Select(x => x.XPathSelectElement("./t:length", nsmgr)?.Value).OfType<string>()
                        .Select(s => TimeSpan.FromMilliseconds(double.Parse(s)));

      var titles = xtracks.Select(x => x.XPathSelectElement("./t:title", nsmgr)?.Value).OfType<string>();

      var sb = new StringBuilder();
      if (artists.Count() > 1) {
        sb.AppendLine($"{artists.Count()} artist(s) found [<{string.Join("> - <", artists.Select(a => $"{a.Item1}-{a.Item2}"))}>];");
      }
      if (titles.Distinct().Count() > 1) {
        sb.AppendLine($"{titles.Distinct().Count()} title(s) found [<{string.Join("> - <", titles.Distinct())}>];");
      }
      if (recordscores.Average() < 100) {
        sb.AppendLine($"{recordscores.Count()} recording scrore(s) found [<{ string.Join("> - <", recordscores)}>];");
      }
      if (!timespans.Any()) {
        sb.AppendLine($"{xtracks.Count()} track(s) found, but no recording length entry found;");
      }
      if (!releaseyears.Any()) {
        sb.AppendLine($"{xtracks.Count()} track(s) found, but no recording release year entry found;");
      }

      return (artists.First().Item1, new Song {
        Title = song, // titles.GroupBy(t => t).OrderByDescending(g => g.Count()).First().First(),
        FoundTitles = string.Join(",", titles.GroupBy(t => t).OrderByDescending(g => g.Count()).Select(t => t.Key)),
        FoundTracks = xtracks.Count(),
        FirstRelease = releaseyears.Any() ? releaseyears.First() : string.Empty,
        Duration = timespans.Any() ? timespans.GroupBy(t => t).OrderByDescending(g => g.Count()).First().Key : TimeSpan.Zero,
        DurationMin = timespans.Any() ? timespans.Min() : TimeSpan.Zero,
        DurationMax = timespans.Any() ? timespans.Max() : TimeSpan.Zero,
        DurationAvg = timespans.Any() ? TimeSpan.FromSeconds(timespans.Average(t => t.TotalSeconds)) : TimeSpan.Zero,
        TaggingHint = sb.ToString(),
        TaggingScore = recordscores.Average()
      });
    }

    private async Task<Artist> LoadArtistAsync(string artistId, CancellationToken ct) {
      var xdoc = await RequestData($"https://musicbrainz.org/ws/2/artist/{artistId}?inc=genres%20tags%20aliases", ct);

      var nsmgr = new XmlNamespaceManager(new NameTable());
      nsmgr.AddNamespace("t", xdoc.Root.Name.Namespace.NamespaceName);

      var xa = xdoc.Root.XPathSelectElement("./t:artist", nsmgr); //.Elements("artist").FirstOrDefault();


      var xtg = xa.XPathSelectElements("t:genre-list/t:genre", nsmgr)?.OrderByDescending(x => x.Attribute("count").Value);
      var w = xtg.Select(x => x.XPathSelectElement("t:name", nsmgr).Value);
      var z = w.FirstOrDefault();

      try {
        return new Artist {
          MusicBrainzId = Guid.Parse(xa.Attribute("id").Value),
          Title = xa.XPathSelectElement("t:name", nsmgr)?.Value,
          Country = xa.XPathSelectElement("t:area/t:name", nsmgr)?.Value, //xa.Element("area")?.Element("name")?.Value,
          City = xa.XPathSelectElement("t:begin-area/t:name", nsmgr)?.Value, //xa.Element("begin-area")?.Element("name")?.Value,
          Established = xa.XPathSelectElement("t:life-span/t:begin", nsmgr)?.Value?.Split("-")[0], // xa.Element("life-span")?.Element("begin")?.Value,
          SplitUp = xa.XPathSelectElement("t:life-span/t:end", nsmgr)?.Value?.Split("-")[0], //xa.Element("life-span")?.Element("end")?.Value,
          TopGenre = xa.XPathSelectElements("t:genre-list/t:genre", nsmgr).OrderByDescending(x => x.Attribute("count").Value).Select(x => x.XPathSelectElement("t:name", nsmgr).Value).FirstOrDefault() ?? "",
          Tags = GetArtistRatedListItems(xa, nsmgr, "t:tag-list/t:tag"),
          Genres = GetArtistRatedListItems(xa, nsmgr, "t:genre-list/t:genre"),
          Alias = GetArtistListProperties(xa, nsmgr, "t:alias-list/t:alias", "sort-name")
        };
      } catch (Exception ex) {
        throw new Exception("Error while parsing artist from xelement", ex);
      }
    }

    private string GetArtistListProperties(XElement xe, XmlNamespaceManager ns, string itemxpath, string sortattribute) {
      return string.Join(",", xe.XPathSelectElements(itemxpath, ns)
                                .OrderByDescending(x => x.Attribute(sortattribute).Value)
                                .Select(x => x.Value));
    }

    private string GetArtistRatedListItems(XElement xe, XmlNamespaceManager ns, string itemxpath) {
      return string.Join(",", xe.XPathSelectElements(itemxpath, ns)
                                .OrderByDescending(x => x.Attribute("count").Value)
                                .Select(x => $"{x.XPathSelectElement("t:name", ns).Value} ({x.Attribute("count").Value})"));
    }
  }
}
