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

        public SpotifyAPIHandler() 
        {
            http = new HTTPHandler(baseUrl);
        }
    }
}
