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

        #region Authorization Code with PKCE -> Access Token
        private static string _redirectUri = $"http://localhost:5000/callback/";

        private static string GenerateRandomString(int length = 64) => string.Concat(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length).Select(s => s[new Random().Next(s.Length)]));
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

        private static async Task<(string, string)> RequestUserAuthCode()
        {
            string clientID = _clientID;
            if (clientID == "")
            {
                GetClientIDAndSecret();
                clientID = _clientID;
            }

            string redirectURI = _redirectUri;
            string scope = "user-read-private user-read-email";
            string authUrl = $"https://accounts.spotify.com/authorize/";

            string _codeVerifier = GenerateRandomString();
            Debug.WriteLine(_codeVerifier);
            string codeChallenge = Base64Encode(Sha256(_codeVerifier));
            string url = $"{authUrl}?client_id={clientID}&response_type=code&redirect_uri={Uri.EscapeDataString(redirectURI)}&scope={Uri.EscapeDataString(scope)}&code_challenge={Uri.EscapeDataString(codeChallenge)}&code_challenge_method=S256";

            // start http server to receive authorization code
            string authCode = "";
            LocalHttpServer server = new LocalHttpServer(redirectURI);
            OpenUrlInBrowser(url);
            authCode = await server.StartListeningAsync();

            return (authCode, _codeVerifier);
        }

        public static async Task<string> RequestUserAccessToken()
        {
            string clientID = _clientID;
            string redirectUri = _redirectUri;

            var (_authCode, _codeVerifier) = await RequestUserAuthCode();
            string authCode = _authCode;
            string codeVerifier = _codeVerifier;

            if (string.IsNullOrEmpty(clientID))
            {
                GetClientIDAndSecret();
                clientID = _clientID;
            }

            string url = "https://accounts.spotify.com/api/token"; // Updated endpoint

            using (var client = new HttpClient())
            {
                // Add the Content-Type header
                //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");

                // Prepare form data
                Debug.WriteLine(codeVerifier);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientID),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", authCode),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("code_verifier", codeVerifier)
                });

                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    // Read and print the response body, even if it’s a bad request
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Response Body: {responseBody}");

                    response.EnsureSuccessStatusCode();

                    // Read and parse the response
                    //var responseBody = await response.Content.ReadAsStringAsync();
                    var tokenData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);

                    // Extract the access_token
                    if (tokenData != null && tokenData.TryGetValue("access_token", out JsonElement accessTokenElement))
                    {
                        return accessTokenElement.GetString() ?? string.Empty;
                    }

                    throw new Exception("Access token not found in response");
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception($"Failed to get access token: {ex.Message}", ex);
                }
            }
        }


        #endregion
    }
}
