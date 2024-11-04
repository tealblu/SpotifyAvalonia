using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAvalonia.Controllers
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    internal class HTTPHandler
    {
        private readonly HttpClient _client;

        // Constructor
        public HTTPHandler(string baseUrl, string? authToken = null)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };

            if (!string.IsNullOrEmpty(authToken))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }
        }

        // GET request
        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            try
            {
                return await _client.GetAsync(endpoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET request failed: {ex.Message}");
                throw;
            }
        }

        // POST request with optional JSON content
        public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content = null)
        {
            try
            {
                return await _client.PostAsync(endpoint, content ?? new StringContent(""));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST request failed: {ex.Message}");
                throw;
            }
        }

        // PUT request
        public async Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content)
        {
            try
            {
                return await _client.PutAsync(endpoint, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PUT request failed: {ex.Message}");
                throw;
            }
        }

        // DELETE request
        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            try
            {
                return await _client.DeleteAsync(endpoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DELETE request failed: {ex.Message}");
                throw;
            }
        }

        // Method to update the authorization token
        public void UpdateAuthToken(string newToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
        }

        // Dispose of HttpClient when done
        public void Dispose()
        {
            _client.Dispose();
        }
    }

}
