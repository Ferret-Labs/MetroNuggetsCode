using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using WebViewNavigateExtensions.Extensions;

namespace WebViewNavigateExtensions.Behaviors
{
    public class WebViewBehavior : Behavior<WebView>
    {
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
            nameof(IsLoading), typeof(bool), typeof(WebViewBehavior), new PropertyMetadata(default(bool)));

        public bool IsLoading
        {
            get { return (bool) GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register(
            nameof(Url), typeof(string), typeof(WebViewBehavior), new PropertyMetadata(default(string), (o, args) => (o as WebViewBehavior)?.LoadUrl()));

        public string Url
        {
            get { return (string) GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register(
            nameof(Html), typeof(string), typeof(WebViewBehavior), new PropertyMetadata(default(string), (o, args) => (o as WebViewBehavior)?.LoadHtml()));

        public string Html
        {
            get { return (string) GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        private async void LoadUrl()
        {
            IsLoading = true;

            if (!string.IsNullOrEmpty(Url))
            {
                await AssociatedObject.NavigateAsync(new Uri(Url));
            }

            IsLoading = false;
        }

        private async void LoadHtml()
        {
            IsLoading = true;
            
            await AssociatedObject.NavigateToStringAsync(Html);

            IsLoading = false;
        }
    }
}
