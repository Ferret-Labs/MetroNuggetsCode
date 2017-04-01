using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace WebViewNavigateExtensions.Extensions
{
    public static class WebViewExtensions
    {
        public static Task<WebViewNavigationResult> NavigateAsync(
            this WebView webView, 
            Uri uri, 
            HttpMethod method = null,
            IHttpContent content = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpMethod = method ?? HttpMethod.Get;
            
            TypedEventHandler<WebView, WebViewNavigationCompletedEventArgs> completedHandler = null;
            WebViewNavigationFailedEventHandler failedHandler = null;
            var tcs = new TaskCompletionSource<WebViewNavigationResult>(cancellationToken);

            completedHandler = (sender, args) =>
            {
                webView.NavigationFailed -= failedHandler;
                webView.NavigationCompleted -= completedHandler;
                var status = args.IsSuccess ? 200 : (int) args.WebErrorStatus;
                if (!tcs.Task.IsCanceled)
                {
                    tcs.SetResult(new WebViewNavigationResult(args.Uri, status));
                }
            };

            failedHandler = (sender, args) =>
            {
                webView.NavigationFailed -= failedHandler;
                webView.NavigationCompleted -= completedHandler;
                if (!tcs.Task.IsCanceled)
                {
                    tcs.SetResult(new WebViewNavigationResult(args.Uri, (int)args.WebErrorStatus));
                }
            };

            Action cancellationAction = null;
            cancellationAction = () =>
            {
                webView.NavigationFailed -= failedHandler;
                webView.NavigationCompleted -= completedHandler;
                webView.Stop();
            };

            using (var registration = cancellationToken.Register(cancellationAction))
            {
                webView.NavigationCompleted += completedHandler;
                webView.NavigationFailed += failedHandler;

                var requestMessage = new HttpRequestMessage(httpMethod, uri);

                webView.NavigateWithHttpRequestMessage(requestMessage);

                return tcs.Task;
            }
        }

        public static Task<WebViewNavigationResult> NavigateAsync(
            this WebView webView, 
            string url, 
            HttpMethod method = null, 
            IHttpContent content = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return webView.NavigateAsync(new Uri(url), method, content, cancellationToken);
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
