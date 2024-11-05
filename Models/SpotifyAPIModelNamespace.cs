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
            Popularity = root.GetProperty("popularity").GetInt32();
            Genres = root.GetProperty("genres").EnumerateArray().Select(x => x.GetString()).ToList();
        }
    }
}
