using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SpotifyAvalonia.Models;

namespace SpotifyAvalonia.Controllers
{
    internal static class SpotifyAPIHandler
    {
        #region Utils
        private static string? AccessToken { get; set; } = null;

        public static async Task GetNewAccessToken()
        {
            string clientID = "";
            string clientSecret = "";

            string jsonpath = "C:\\Projects\\SpotifyAvalonia\\appsettings.json";
            if (File.Exists(jsonpath))
            {
                string json = File.ReadAllText(jsonpath);
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (config != null)
                {
                    clientID = config["ClientID"];
                    clientSecret = config["ClientSecret"];
                } else
                {
                    throw new Exception("Unable to read appsettings.json");
                }
            }

            string url = "https://accounts.spotify.com/api/token/";

            using (HttpClient client = new HttpClient())
            {
                var formData = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", clientID },
                    { "client_secret", clientSecret }
                };

                var content = new FormUrlEncodedContent(formData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var accessToken = JsonSerializer.Deserialize<SpotifyAccessToken>(responseString);
                    AccessToken = accessToken?.access_token;
                }
            }
        }

        public static async Task<string> SendRequest(string url)
        {
            if (AccessToken == null)
            {
                await GetNewAccessToken();
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken!);
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
            }

            return "";
        }
        #endregion

        #region Artists
        public static async Task<Artist> GetArtist(string artistID)
        {
            if (AccessToken == null)
            {
                await GetNewAccessToken();
            }

            string responseString = "";
            string url = "https://api.spotify.com/v1/artists/" + artistID;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken!);
                var response = await client.GetAsync(url);
                responseString = await response.Content.ReadAsStringAsync();
            }

            if (responseString != null)
            {
                return new Artist(responseString);
            } 
            else
            {
                return new Artist();
            }
        }

        public static async Task<List<Artist>> SearchForArtist(string artistName)
        {
            if (AccessToken == null)
            {
                await GetNewAccessToken();
            }

            string responseString = "";
            string url = "https://api.spotify.com/v1/search?q=" + artistName + "&type=artist";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken!);
                var response = await client.GetAsync(url);
                responseString = await response.Content.ReadAsStringAsync();
            }

            if (responseString != null)
            {
                JsonDocument doc = JsonDocument.Parse(responseString);
                JsonElement root = doc.RootElement;
                List<Artist> artists = root.GetProperty("artists").GetProperty("items").EnumerateArray().Select(x => new Artist(x.ToString())).ToList();

                return artists;
            }

            return new List<Artist>();
        }
        #endregion

        #region Tracks
        public static async Task<Track> GetTrack(string trackID)
        {
            if (AccessToken == null)
            {
                await GetNewAccessToken();
            }

            string url = "https://api.spotify.com/v1/tracks/" + trackID;
            string responseString = await SendRequest(url);

            if (responseString != "")
            {
                return new Track(responseString);
            }
            else
            {
                return new Track();
            }
        }

        public static async Task<List<Track>> SearchForTrack(string trackName)
        {
            if (AccessToken == null)
            {
                await GetNewAccessToken();
            }

            string url = "https://api.spotify.com/v1/search?q=" + trackName + "&type=track";
            string responseString = await SendRequest(url);

            if (responseString != "")
            {
                JsonDocument doc = JsonDocument.Parse(responseString);
                JsonElement root = doc.RootElement;
                List<Track> tracks = root.GetProperty("tracks").GetProperty("items").EnumerateArray().Select(x => new Track(x.ToString())).ToList();
            
                return tracks;
            }

            return new List<Track>();
        }
        #endregion

        #region Albums
        public static async Task<Album> GetAlbum(string albumID)
        {
            if (AccessToken == null)
            {
                await GetNewAccessToken();
            }

            string url = "https://api.spotify.com/v1/albums/" + albumID;
            string responseString = await SendRequest(url);

            if (responseString != "")
            {
                return new Album(responseString);
            }
            else
            {
                return new Album();
            }
        }
        #endregion
    }
}
