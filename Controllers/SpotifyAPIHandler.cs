using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotifyAvalonia.Controllers
{
    internal class SpotifyAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    internal static class SpotifyAPIHandler
    {
        private static string? AccessToken { get; set; }

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
    }
}
