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
    
        public static async Task<string> SendRequest(string url)
        {
            if (SpotifyAuthHandler.AccessToken == null)
            {
                await SpotifyAuthHandler.GetNewAccessToken();
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SpotifyAuthHandler.AccessToken!);
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
            if (SpotifyAuthHandler.AccessToken == null)
            {
                await SpotifyAuthHandler.GetNewAccessToken();
            }

            string responseString = "";
            string url = "https://api.spotify.com/v1/artists/" + artistID;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SpotifyAuthHandler.AccessToken!);
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
            if (SpotifyAuthHandler.AccessToken == null)
            {
                await SpotifyAuthHandler.GetNewAccessToken();
            }

            string responseString = "";
            string url = "https://api.spotify.com/v1/search?q=" + artistName + "&type=artist";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SpotifyAuthHandler.AccessToken!);
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
            if (SpotifyAuthHandler.AccessToken == null)
            {
                await SpotifyAuthHandler.GetNewAccessToken();
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
            if (SpotifyAuthHandler.AccessToken == null)
            {
                await SpotifyAuthHandler.GetNewAccessToken();
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
            if (SpotifyAuthHandler.AccessToken == null)
            {
                await SpotifyAuthHandler.GetNewAccessToken();
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

        public static async Task<List<Album>> SearchForAlbum(string albumName)
        {
            if (SpotifyAuthHandler.AccessToken == null)
            {
                await SpotifyAuthHandler.GetNewAccessToken();
            }

            string url = "https://api.spotify.com/v1/search?q=" + albumName + "&type=album";
            string responseString = await SendRequest(url);

            if (responseString != "")
            {
                JsonDocument doc = JsonDocument.Parse(responseString);
                JsonElement root = doc.RootElement;
                List<Album> albums = root.GetProperty("albums").GetProperty("items").EnumerateArray().Select(x => new Album(x.ToString())).ToList();
                
                return albums;
            }

            return new List<Album>();
        }
        #endregion
    }
}
