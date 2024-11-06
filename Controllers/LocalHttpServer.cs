using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAvalonia.Controllers
{
    internal class LocalHttpServer
    {
        private readonly HttpListener _listener;
        private readonly string _redirectUrl;

        public LocalHttpServer(string redirectUrl)
        {
            _redirectUrl = redirectUrl;
            _listener = new HttpListener();
            _listener.Prefixes.Add(_redirectUrl);
        }

        public async Task<string> StartListeningAsync()
        {
            _listener.Start();
            Console.WriteLine("Listening for OAuth callback...");

            var context = await _listener.GetContextAsync(); // Waits for the redirect request
            string? authCode = context.Request.QueryString["code"]; // Gets the authorization code

            // Respond to the user's browser to show a success message
            string responseString = "<html><body>Authorization successful! You can close this window.</body></html>";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();

            _listener.Stop(); // Stop the listener after receiving the code

            if (authCode == null)
            {
                throw new Exception("Authorization code not found.");
            }
            else
            {
                return authCode;
            }
        }
    }

}
