using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Cimbalino.Toolkit.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CimbalinoBackButton
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page2 : Page
    {
        public Page2()
        {
            this.InitializeComponent();

            App.NavigationService.BackKeyPressed += NavigationServiceOnBackKeyPressed;
        }

        private void NavigationServiceOnBackKeyPressed(object sender, NavigationServiceBackKeyPressedEventArgs e)
        {
            e.Behavior = NavigationServiceBackKeyPressedBehavior.GoBack;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            App.NavigationService.Navigate<Page3>();
        }
    }
}
