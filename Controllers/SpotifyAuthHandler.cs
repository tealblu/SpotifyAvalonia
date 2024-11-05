using SpotifyAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace SpotifyAvalonia.Controllers
{
    internal class SpotifyAuthHandler
    {
        public static string? AccessToken { get; set; } = null;

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
                }
                else
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
                    var token = JsonSerializer.Deserialize<AccessToken>(responseString);
                    SpotifyAuthHandler.AccessToken = token?.access_token;
                }
            }
        }
    }
}
