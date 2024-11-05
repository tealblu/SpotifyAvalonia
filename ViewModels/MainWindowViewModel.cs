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
        private ObservableCollection<string> itemList = new ObservableCollection<string>();

        public void ButtonCommand()
        {
            Task.Run(async () =>
            {
                List<Album> albums = await SpotifyAPIHandler.SearchForAlbum("Clancy");

                if (albums != null && albums.Count > 0)
                {
                    ItemList.Clear();
                    foreach (Album album in albums)
                    {
                        ItemList.Add(album.Name);
                    }
                }
            });
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
