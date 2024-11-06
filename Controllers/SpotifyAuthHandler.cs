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
using System.Security.Cryptography;

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

        private static string GenerateRandomString(int length = 64)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randomBytes = new byte[length];
            RandomNumberGenerator.Fill(randomBytes);
            return new string(randomBytes.Select(b => chars[b % chars.Length]).ToArray());
        }

        private static byte[] Sha256(string input) => System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(input));
        private static string Base64Encode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

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
            if (_clientID == "")
            {
                GetClientIDAndSecret();
            }
            string clientID = _clientID;

            string redirectURI = _redirectUri;
            string scope = "user-read-private user-read-email";
            string authUrl = $"https://accounts.spotify.com/authorize/";

            string _codeVerifier = GenerateRandomString();
            string codeChallenge = Base64Encode(Sha256(_codeVerifier));
            string url = $"{authUrl}?client_id={clientID}&response_type=code&redirect_uri={Uri.EscapeDataString(redirectURI)}&scope={Uri.EscapeDataString(scope)}&code_challenge={Uri.EscapeDataString(codeChallenge)}&code_challenge_method=S256";

            // start http server to receive authorization code
            string authCode = "";
            LocalHttpServer server = new LocalHttpServer(redirectURI);
            OpenUrlInBrowser(url);
            authCode = await server.StartListeningAsync();

            return (authCode, _codeVerifier);
        }

        public static UserAccessToken UserAccessToken { get; set; } = new UserAccessToken();
        public static async Task RequestUserAccessToken()
        {
            if (_clientID == "")
            {
                GetClientIDAndSecret();
            }

            string clientID = _clientID;
            string url = "https://accounts.spotify.com/api/token";
            string redirectUri = _redirectUri;

            var (authCode, codeVerifier) = await RequestUserAuthCode();

            using (var client = new HttpClient())
            {
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

                    var responseBody = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    var tokenData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseBody);

                    if (tokenData != null && tokenData.TryGetValue("access_token", out JsonElement accessTokenElement))
                    {
                        UserAccessToken.access_token = accessTokenElement.GetString() ?? string.Empty;
                    }
                    else
                    {
                        throw new Exception("Access token not found in response");
                    }

                    if (tokenData != null && tokenData.TryGetValue("expires_in", out JsonElement expiresInElement))
                    {
                        int expiresIn = (int?)expiresInElement.GetInt32() ?? 0;
                        UserAccessToken.SetExpiryTime(expiresIn);
                    }
                    else
                    {
                        throw new Exception("Expiry not found in response");
                    }

                    if (tokenData != null && tokenData.TryGetValue("refresh_token", out JsonElement refreshTokenElement))
                    {
                        UserAccessToken.refresh_token = refreshTokenElement.GetString() ?? string.Empty;
                    }
                    else
                    {
                        throw new Exception("Refresh token not found in response");
                    }
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
