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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Web.Http;

namespace SpotifyAvalonia.Controllers
{
    internal static class SpotifyAuthHandler
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
        private static string GenerateRandomString(int length = 16) => string.Concat(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length).Select(s => s[new Random().Next(s.Length)]));
        private static string Sha256(string input) => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(input)));
        private static string Base64Encode(string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

        // code from https://stackoverflow.com/a/43232486
        private static void OpenUrlInBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public static void RequestUserAuthorization()
        {
            string clientID = _clientID;
            if (clientID == "")
            {
                GetClientIDAndSecret();
                clientID = _clientID;
            }

            string redirectURI = $"http://localhost:5000/callback/";
            string scope = "user-read-private user-read-email";
            string authUrl = $"https://accounts.spotify.com/authorize/";

            string codeVerifier = GenerateRandomString();
            string codeChallenge = Base64Encode(Sha256(codeVerifier));

            // TODO save the code verifier here?

            

            LocalHttpServer server = new LocalHttpServer(redirectURI);
            string url = $"{authUrl}?client_id={clientID}&response_type=code&redirect_uri={Uri.EscapeDataString(redirectURI)}&scope={Uri.EscapeDataString(scope)}&code_challenge={Uri.EscapeDataString(codeChallenge)}&code_challenge_method=S256";
            OpenUrlInBrowser(url);

            // start http server to receive authorization code
            string OAuthCode = "";
            Task.Run(async () =>
            {
                OAuthCode = await server.StartListeningAsync();
            });
        }
        #endregion
    }
}
