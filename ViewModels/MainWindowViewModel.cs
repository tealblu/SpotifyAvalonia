﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyAvalonia.Controllers;
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
        private string authStatus = "Not logged in";

        public void LoginCommand()
        {
            AuthStatus = "Logging in...";

            SpotifyAPIHandler.GetNewAccessToken();

            return;
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
