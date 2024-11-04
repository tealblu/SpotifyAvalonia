using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAvalonia.Controllers
{
    // Class to interact with Spotify API
    internal class SpotifyAPIHandler
    {
        private string baseUrl = "https://api.spotify.com/v1/";

        private HTTPHandler http;

        private static string _token = "";

        public static string Token
        {
            get
            {
                return _token;
            }
            set
            {
                _token = value;
            }
        }

        public SpotifyAPIHandler() 
        {
            http = new HTTPHandler(baseUrl);
        }

        public SpotifyAPIHandler(string token)
        {
            http = new HTTPHandler(baseUrl, _token);
        }
    }
}
