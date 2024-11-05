using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyAvalonia.Controllers;
using SpotifyAvalonia.Models;
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
                Artist artist = await SpotifyAPIHandler.GetArtist("5mGnbFnoUYEUNE5EKnQY7R");

                MessageBox = artist.Name;
            });
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
