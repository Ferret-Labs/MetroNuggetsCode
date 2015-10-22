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
                var provider = await WebAuthenticationCoreManager.FindAccountProviderAsync("https://www.facebook.com");
                if (provider != null)
                {
                    var response = await GetToken(provider, true);
                    if (response != null)
                    {
                        // We have the response token, so we can check if we are authenticated. 
                        await ProcessResponse(response, provider);
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
                var i = 1;
            }
        }

        private async Task ProcessResponse(WebTokenRequestResult response, WebAccountProvider provider)
        {
            switch (response.ResponseStatus)
            {
                case WebTokenRequestStatus.Success:
                    try
                    {
                        var tokenResponse = response.ResponseData[0];
                        var accessToken = tokenResponse?.Token;
                        var username = tokenResponse?.WebAccount?.UserName;

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
                case WebTokenRequestStatus.UserInteractionRequired:
                    response = await GetToken(provider, false);
                    if (response != null)
                    {
                        await ProcessResponse(response, provider);
                    }
                    break;

            }
        }

        private async Task<WebTokenRequestResult> GetToken(WebAccountProvider provider, bool isSilent)
        {
            WebTokenRequestResult result = null;

            try
            {
                var request = new WebTokenRequest(provider, "public_profile", "1043253362376121");

                // We need to add the redirect uri so that the facebook app knows it's actually us.
                // This will use the store id you assigned in your facebook developer portal
                request.Properties.Add("redirect_uri", "msft-2f5fb048-0fd0-43e4-ad74-a9fc71e4b53d:/Authorise");

                if (isSilent)
                {
                    result = await WebAuthenticationCoreManager.GetTokenSilentlyAsync(request);
                }
                else
                {
                    result = await WebAuthenticationCoreManager.RequestTokenAsync(request);
                }
            }
            catch (Exception ex)
            {
                var i = 1;
            }

            return result;
        }

        private void Fallbacks()
        {
            // Do something with the WebAuthenticationBroker
        }
    }
}
