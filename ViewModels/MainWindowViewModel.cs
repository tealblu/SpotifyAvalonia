﻿using CommunityToolkit.Mvvm.ComponentModel;
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
                List<Album> albums = await SpotifyAPIHandler.SearchForAlbum("Clancy");

                if (albums != null && albums.Count > 0)
                {
                    MessageBox = albums[0].Artists[0].Name;
                }
            });
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
