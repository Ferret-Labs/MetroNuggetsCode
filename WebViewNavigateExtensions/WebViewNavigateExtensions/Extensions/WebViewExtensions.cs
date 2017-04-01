using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace WebViewNavigateExtensions.Extensions
{
    public static class WebViewExtensions
    {
        public static Task<WebViewNavigationResult> NavigateAsync(this WebView webView, Uri uri, HttpMethod method = null)
        {
            var httpMethod = method ?? HttpMethod.Get;

            TypedEventHandler<WebView, WebViewNavigationCompletedEventArgs> completedHandler = null;
            WebViewNavigationFailedEventHandler failedHandler = null;
            var tcs = new TaskCompletionSource<WebViewNavigationResult>();

            completedHandler = (sender, args) =>
            {
                webView.NavigationCompleted -= completedHandler;
                var status = args.IsSuccess ? 200 : (int) args.WebErrorStatus;
                tcs.SetResult(new WebViewNavigationResult(args.Uri, status));
            };

            failedHandler = (sender, args) =>
            {
                webView.NavigationFailed -= failedHandler;
                tcs.SetResult(new WebViewNavigationResult(args.Uri, (int)args.WebErrorStatus));
            };

            webView.NavigationCompleted += completedHandler;
            webView.NavigationFailed += failedHandler;

            var requestMessage = new HttpRequestMessage(httpMethod, uri);

            webView.NavigateWithHttpRequestMessage(requestMessage);

            return tcs.Task;
        }

        public static Task<WebViewNavigationResult> NavigateAsync(this WebView webView, string url, HttpMethod method = null)
        {
            return webView.NavigateAsync(new Uri(url), method);
        }
    }

    public class WebViewNavigationResult
    {
        public Uri Uri { get; }

        public int StatusCode { get; }

        public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

        public WebViewNavigationResult(Uri uri, int statusCode)
        {
            Uri = uri;
            StatusCode = statusCode;
        }
    }
}
