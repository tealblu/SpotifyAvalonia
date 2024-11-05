using CommunityToolkit.Mvvm.ComponentModel;
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
        private string messageBox = "Not logged in";

        public void ButtonCommand()
        {
            MessageBox = "not implemented";
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
