﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace WebViewNavigateExtensions.Extensions
{
    public static class WebViewExtensions
    {
        public static Task<WebViewNavigationResult> NavigateToStringAsync(
            this WebView webView,
            string text,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return NavigateInternal(webView, () => webView.NavigateToString(text), cancellationToken);
        }

        public static Task<WebViewNavigationResult> NavigateAsync(
            this WebView webView,
            HttpRequestMessage request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return NavigateInternal(webView, () => webView.NavigateWithHttpRequestMessage(request), cancellationToken);
        }

        public static Task<WebViewNavigationResult> NavigateAsync(
            this WebView webView,
            Uri uri,
            HttpMethod method = null,
            IHttpContent content = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpMethod = method ?? HttpMethod.Get;
            var requestMessage = new HttpRequestMessage(httpMethod, uri)
            {
                Content = content
            };

            return webView.NavigateAsync(requestMessage, cancellationToken);
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

        private static async Task<WebViewNavigationResult> NavigateInternal(
            WebView webView,
            Action navigationAction,
            CancellationToken cancellationToken)
        {
            TypedEventHandler<WebView, WebViewNavigationCompletedEventArgs> completedHandler = null;
            WebViewNavigationFailedEventHandler failedHandler = null;
            TypedEventHandler<WebView, WebViewUnsupportedUriSchemeIdentifiedEventArgs> unsupportedUriHandler = null;
            var tcs = new TaskCompletionSource<WebViewNavigationResult>();

            Action unhookEvents = () =>
            {
                webView.NavigationFailed -= failedHandler;
                webView.NavigationCompleted -= completedHandler;
                webView.UnsupportedUriSchemeIdentified -= unsupportedUriHandler;
            };

            completedHandler = (sender, args) =>
            {
                unhookEvents();
                var status = args.IsSuccess ? 200 : (int)args.WebErrorStatus;
                tcs.TrySetResult(new WebViewNavigationResult(args.Uri, status));
            };

            failedHandler = (sender, args) =>
            {
                unhookEvents();
                tcs.TrySetResult(new WebViewNavigationResult(args.Uri, (int)args.WebErrorStatus));
            };

            unsupportedUriHandler = (sender, args) =>
            {
                unhookEvents();
                args.Handled = true;
                tcs.TrySetException(new UnsupportedUriSchemeException(args.Uri));
            };

            Action cancellationAction = null;
            cancellationAction = () =>
            {
                unhookEvents();
                webView.Stop();
            };

            try
            {
                using (var registration = cancellationToken.Register(cancellationAction))
                {
                    webView.NavigationCompleted += completedHandler;
                    webView.NavigationFailed += failedHandler;
                    webView.UnsupportedUriSchemeIdentified += unsupportedUriHandler;

                    navigationAction();

                    var result = await tcs.Task;
                    return result;
                }
            }
            finally
            {
                unhookEvents();
            }
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

    public class UnsupportedUriSchemeException : Exception
    {
        public Uri Uri { get; }

        public UnsupportedUriSchemeException(Uri uri)
        {
            Uri = uri;
        }
    }
}
