using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
