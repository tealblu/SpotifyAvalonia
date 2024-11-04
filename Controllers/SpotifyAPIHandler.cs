using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotifyAvalonia.Controllers
{ 
    // Class to interact with Spotify API
    internal static class SpotifyAPIHandler
    {
        public static async Task<string> GetNewAccessToken()
        {
            string clientID = "";
            string clientSecret = "";

            string jsonpath = "C:\\Projects\\SpotifyAvalonia\\appsettings.json";
            if (File.Exists(jsonpath))
            {
                string json = File.ReadAllText(jsonpath);
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                clientID = config["ClientID"];
                clientSecret = config["ClientSecret"];
            }

            string url = "https://accounts.spotify.com/api/token";

            using (HttpClient client = new HttpClient())
            {
                // Prepare body with client credentials
                string body = $"grant_type=client_credentials&client_id={clientID}&client_secret={clientSecret}";
                var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

                // Send POST request
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    return responseBody;
                }
            }

            return "";
        }
    }

    internal class SpotifyAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
