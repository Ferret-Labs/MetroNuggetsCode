using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WebViewNavigateExtensions.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WebViewNavigateExtensions
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

        private async void NavigateTo_OnClick(object sender, RoutedEventArgs e)
        {
            var result = await MainWebView.NavigateToStringAsync("http://microsoft.com");

            var dialog = new MessageDialog($"Navigation was successful? {result.IsSuccess}");
            await dialog.ShowAsync();
        }
    }
}
