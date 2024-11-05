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
        #region Client ID and Secret
        private static string _clientID = "";
        private static string _clientSecret = "";

        private static void GetClientIDAndSecret()
        {
            string jsonpath = "C:\\Projects\\SpotifyAvalonia\\appsettings.json";
            if (File.Exists(jsonpath))
            {
                string json = File.ReadAllText(jsonpath);
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (config != null)
                {
                    _clientID = config["ClientID"];
                    _clientSecret = config["ClientSecret"];
                }
                else
                {
                    throw new Exception("Unable to read appsettings.json");
                }
            }
        }
        #endregion

        #region Access Token
        public static string? AccessToken { get; set; } = null;

        public static async Task GetNewAccessToken()
        {
            GetClientIDAndSecret();

            string clientID = _clientID;
            string clientSecret = _clientSecret;

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
        #endregion

        #region Authorization Code with PKCE
        private static string _codeVerifier = GenerateRandomString();
        private static void GenerateNewCodeVerifier() => _codeVerifier = GenerateRandomString();

        private static string GenerateRandomString(int length = 16) => string.Concat(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length).Select(s => s[new Random().Next(s.Length)]));
        private static string Sha256(string input) => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(input)));
        private static string Base64Encode(string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        public static string GenerateCodeChallenge(string codeVerifier) => Base64Encode(Sha256(codeVerifier));

        public static void RequestUserAuthorization()
        {
            string clientID = 
        }
        #endregion
    }
}
