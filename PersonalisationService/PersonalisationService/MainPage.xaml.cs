using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Cimbalino.Toolkit.Services;

namespace PersonalisationService
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var personalisation = new PersonalizationService();
            if (personalisation.IsSupported)
            {
                await personalisation.SetWallpaperImageAsync("ms-appx:///Assets/Wallpaper.jpg", true);
            }
        }
    }
}
