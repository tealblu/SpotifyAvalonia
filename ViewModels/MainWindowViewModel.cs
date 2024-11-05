using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyAvalonia.Controllers;
using SpotifyAvalonia.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SpotifyAvalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
#pragma warning disable CA1822 // Mark members as static
        [ObservableProperty]
        private string mainWindowHeader = "SpotifyAvalonia";

        [ObservableProperty]
        private string messageBox = "";

        public void ButtonCommand()
        {
            MessageBox = "loading...";

            Task.Run(async () =>
            {
                List<Artist> artists = await SpotifyAPIHandler.SearchForArtist("Driftless Pony Club");

                string artistName = artists[0].Name;

                MessageBox = artistName;
            });
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
