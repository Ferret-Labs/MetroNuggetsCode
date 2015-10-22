using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.UI.Xaml.Input;

namespace FacebookWebAccountProvider
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                // Check whether there is an app for the provider we want to use (facebook)
                var provider = await WebAuthenticationCoreManager.FindAccountProviderAsync("http://www.facebook.com");
                if (provider != null)
                {
                    var response = await GetTokenSilently(provider);
                    if (response != null)
                    {
                        // We have the response token, so we can check if we are authenticated. 
                        ProcessResponse(response);
                    }
                    else
                    {
                        // Ok, so now we need to tell Facebook to give us the token and possibly sign in
                        Fallbacks();
                    }
                }
                else
                {
                    // Use alternative methods
                    Fallbacks();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void ProcessResponse(WebTokenRequestResult response)
        {
            switch (response.ResponseStatus)
            {
                case WebTokenRequestStatus.Success:
                    try
                    {
                        var tokenResponse = response.ResponseData[0];
                        
                        // SAVE this response, it has all your authentication info
                    }
                    catch (Exception)
                    {
                    }

                    break;
                case WebTokenRequestStatus.UserCancel:
                    // Handle user cancel by resuming pre-login screen   
                    break;
                case WebTokenRequestStatus.AccountProviderNotAvailable:
                    // Fall back to WebAuthenticationBroker
                    Fallbacks();                     
                    break;
                case WebTokenRequestStatus.ProviderError:
                    break;
            }
        }

        private async Task<WebTokenRequestResult> GetTokenSilently(WebAccountProvider provider)
        {
            WebTokenRequestResult result = null;

            try
            {
                var request = new WebTokenRequest(provider, "read_stream", "<CLIENTID>");

                // We need to add the redirect uri so that the facebook app knows it's actually us.
                // This will be the redirect uri you assigned in your facebook developer portal
                request.Properties.Add("redirect_uri", "msft-<REDIRECTURL>:");

                result = await WebAuthenticationCoreManager.GetTokenSilentlyAsync(request);
            }
            catch (Exception ex)
            {
                
            }

            return result;
        }

        private void Fallbacks()
        {
            // Do something with the WebAuthenticationBroker
        }
    }
}
