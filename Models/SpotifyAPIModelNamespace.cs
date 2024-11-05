using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotifyAvalonia.Models
{
    internal class SpotifyAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    internal class Artist
    {
        public string? Name { get; set; } = "Unknown";
        public string? ID { get; set; } = "Unknown";
        public int? Popularity { get; set; } = -1;
        public List<string?> Genres { get; set; } = new List<string?>();
        public string URL => "https://open.spotify.com/artist/" + ID;

        public Artist() { }

        public Artist(string jsonString)
        {
            ParseJsonString(jsonString);
        }

        public void ParseJsonString(string jsonString)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonString);

            JsonElement root = doc.RootElement;

            Name = root.GetProperty("name").GetString();
            ID = root.GetProperty("id").GetString();

            // items below here are not in the simple artist object
            if (root.TryGetProperty("popularity", out JsonElement popularity))
            {
                Popularity = popularity.GetInt32();
            }

            if (root.TryGetProperty("genres", out JsonElement genres))
            {
                Genres = genres.EnumerateArray().Select(x => x.GetString()).ToList();
            }
        }
    }

    internal class Track
    {
        public string? Name { get; set; } = "Unknown";
        public string? ID { get; set; } = "Unknown";
        public Album? Album { get; set; }
        public List<Artist>? Artists { get; set; } = new List<Artist>();
        public int? Popularity { get; set; } = -1;
        public int? Duration { get; set; } = -1;
        public string? URL => "https://open.spotify.com/track/" + ID;

        public Track() { }

        public Track(string jsonString)
        {
            ParseJsonString(jsonString);
        }

        public void ParseJsonString(string jsonString)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonString);

            JsonElement root = doc.RootElement;

            Name = root.GetProperty("name").GetString();
            ID = root.GetProperty("id").GetString();
            // Album
            Popularity = root.GetProperty("popularity").GetInt32();
            Duration = root.GetProperty("duration_ms").GetInt32();
            var artistArray = root.GetProperty("artists").EnumerateArray();

            foreach (var artist in artistArray)
            {
                if (Artists == null)
                {
                    Artists = new List<Artist>();
                }

                Artists.Add(new Artist(artist.ToString()));
            }
        }
    }

    internal class Album
    {
        public string? Name { get; set; } = "Unknown";
        public string? ID { get; set; } = "Unknown";
        public string? URL => "https://open.spotify.com/album/" + ID;

        public Album() { }

        public Album(string jsonString)
        {
            ParseJsonString(jsonString);
        }

        public void ParseJsonString(string jsonString)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonString);

            JsonElement root = doc.RootElement;

            Name = root.GetProperty("name").GetString();
            ID = root.GetProperty("id").GetString();
        }
    }
}
