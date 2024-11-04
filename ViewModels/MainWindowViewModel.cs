using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyAvalonia.Controllers;
using System.Collections.ObjectModel;

namespace SpotifyAvalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
#pragma warning disable CA1822 // Mark members as static
        [ObservableProperty]
        private string mainWindowHeader = "SpotifyAvalonia";

        [ObservableProperty]
        private string authStatus = "Not logged in";

        public void LoginCommand()
        {
            AuthStatus = "Logging in...";

            AuthStatus = SpotifyAPIHandler.GetNewAccessToken().Result;
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
