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
                List<Track> tracks = await SpotifyAPIHandler.SearchForTrack("Calypso Spiderbait");

                Track track = tracks[0];

                MessageBox = track.Name;
            });
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
