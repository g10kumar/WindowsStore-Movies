using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238


using Windows.Security.Authentication.Web;
using Facebook;
using Windows.Storage;
using Windows.UI.ApplicationSettings;

using System.Dynamic;
using System.Threading.Tasks;
using TwitterRtLibrary;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

using Windows.UI.Core;
using Windows.UI.ViewManagement;
using nsoftware.IPWorksSSL;
using ShareAll.Common;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Resources;
using LinkedInRTLibrary;

namespace ShareAll
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : ShareAll.Common.LayoutAwarePage
    {
        private DataTransferManager dataTransferManager;

        //string _facebookAppId = "160672697306624"; // You must set your own AppId here
        string _facebookAppId = "521064607905686"; // You must set your own AppId here

        string _permissions = "user_about_me,read_stream,publish_stream"; // Set your permissions here
        FacebookClient _fb = new FacebookClient();
        

        #region Email
        string entryptConfigEmail = "";
        string entryptConfigPwd = "";
        string serverType = "";
        string serverDomain = "";
        Htmlmailers htmlmailer = new Htmlmailers();
        Utilities u = new Utilities();

        //        public const string MatchEmailPattern =
        //            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        //     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
        //				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        //     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
        //				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        //     + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public const string MatchEmailPattern = @"\w+([-+.]\w+)*@(yahoo|gmail|hotmail|msn|live)\.com";

        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            //TwitterRt = new TwitterRt("EhY3grAWGzIxF0YXPr70Yw", "uwXl6wNaLowV9No68bNHtQNbMhSpSSgXCXEk4P0g", "http://www.daksatech.com");
            TwitterRt = new TwitterRt("OGAYZv3HDfykHsQw5dtng", "80cs06pnqehabIhv6PY1Vy1RuZX26G5twxJn6Oti0yU", "http://www.daksatech.com");
            htmlmailer.OnSSLServerAuthentication += htmlmailer_OnSSLServerAuthentication;
            context = new OAuthContext("kvpcelszx805", "yZyeKbgdvNTYsk8T", "https://api.linkedin.com/uas/oauth/requestToken", "https://api.linkedin.com/uas/oauth/authorize", "https://api.linkedin.com/uas/oauth/accessToken", "http://www.daksatech.com/");
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        void htmlmailer_OnSSLServerAuthentication(object sender, HtmlmailersSSLServerAuthenticationEventArgs e)
        {
            e.Accept = true;

        }

        void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            string visualState = DetermineVisualState(ApplicationView.Value);

            if (visualState == "Snapped")
            {
                stackPanelFull.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                stackPanelSnapped.Visibility = Windows.UI.Xaml.Visibility.Visible;
                backButton.Style = App.Current.Resources["SnappedBackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["SnappedPageHeaderTextStyle"] as Style;
            }
            else
            {
                stackPanelFull.Visibility = Windows.UI.Xaml.Visibility.Visible;
                stackPanelSnapped.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                backButton.Style = App.Current.Resources["BackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["PageHeaderTextStyle"] as Style;
            }

            PopupLoaded();
        }

        protected string DetermineVisualState(ApplicationViewState viewState)
        {
            if (viewState == ApplicationViewState.Filled || viewState == ApplicationViewState.FullScreenLandscape)
            {
                // Allow pages to request that the Filled state be used only for landscape layouts narrower
                // than 1366 virtual pixels
                var windowWidth = Window.Current.Bounds.Width;
                viewState = windowWidth >= 1366 ? ApplicationViewState.FullScreenLandscape : ApplicationViewState.Filled;
            }
            return viewState.ToString();
        }

        public TwitterRt TwitterRt { get; private set; }
        public OAuthContext context { get; set; }
        public Client client { get; private set; }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                // Register the current page as a share source.
                this.dataTransferManager = DataTransferManager.GetForCurrentView();
                this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
            }
            catch (Exception ex)
            {
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                // Unregister the current page as a share source.
                this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
            }
            catch (Exception ex)
            {
            }
        }
        // When share is invoked (by the user or programatically) the event handler we registered will be called to populate the datapackage with the
        // data to be shared.
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            // Call the scenario specific function to populate the datapackage with the data to be shared.
            if (GetShareContent(e.Request))
            {
                // Out of the datapackage properties, the title is required. If the scenario completed successfully, we need
                // to make sure the title is valid since the sample scenario gets the title from the user.
                if (String.IsNullOrEmpty(e.Request.Data.Properties.Title))
                {
                    //e.Request.FailWithDisplayText(MainPage.MissingTitleError);
                }
            }
        }


        private bool GetShareContent(DataRequest request)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            bool succeeded = false;
            DataPackage requestData = request.Data;
            //requestData.Properties.Title = loader.GetString("Your link title goes here.");
            //requestData.Properties.Description = loader.GetString("This is the description of the message."); // The description is optional.
            //requestData.SetText(loader.GetString("This is the text to share."));
            requestData.Properties.Title = loader.GetString("Your link title goes here");
            requestData.Properties.Description = loader.GetString("This is the description of the message"); // The description is optional.
            requestData.SetText(loader.GetString("This is the text to share"));
            //requestData.SetUri("");

            succeeded = true;
            return succeeded;
        }
        private async void btnConfigureFacebook_Click(object sender, RoutedEventArgs e)
        {
            var redirectUrl = "https://www.facebook.com/connect/login_success.html";
            try
            {
                //fb.AppId = facebookAppId;
                var loginUrl = _fb.GetLoginUrl(new
                {
                    client_id = _facebookAppId,
                    redirect_uri = redirectUrl,
                    scope = _permissions,
                    display = "popup",
                    response_type = "token"
                });

                var endUri = new Uri(redirectUrl);

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        loginUrl,
                                                        endUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var callbackUri = new Uri(WebAuthenticationResult.ResponseData.ToString());
                    var facebookOAuthResult = _fb.ParseOAuthCallbackUrl(callbackUri);
                    var accessToken = facebookOAuthResult.AccessToken;
                    if (String.IsNullOrEmpty(accessToken))
                    {
                        // User is not logged in, they may have canceled the login
                    }
                    else
                    {
                        // User is logged in and token was returned                        
                        LoginSucceded(accessToken);
                    }

                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    throw new InvalidOperationException("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                }
                else
                {
                    // The user canceled the authentication
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void LoginSucceded(string accessToken)
        {
            dynamic parameters = new ExpandoObject();
            parameters.access_token = accessToken;
            parameters.fields = "id";
            ApplicationData.Current.RoamingSettings.Values["FBAccessToken"] = accessToken;
            dynamic result = await _fb.GetTaskAsync("me", parameters);
            parameters = new ExpandoObject();
            parameters.id = result.id;
            parameters.access_token = accessToken;
            ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] = 1;
            //sessionData.isFBConfigure = true;
            var frame = new Frame();
            frame.Navigate(typeof(MainPage));

            Window.Current.Content = frame;
            Window.Current.Activate();

        }

        private async void btnConfigureTwitter_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(ShareIt));
            await TwitterRt.GainAccessToTwitter();
            if (TwitterRt.Status == "Access granted")
            {
                //sessionData.isTweetConfigure = true;
                ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] = "1";
                ApplicationData.Current.RoamingSettings.Values["TweetOauthToken"] = TwitterRt.OauthToken;
                ApplicationData.Current.RoamingSettings.Values["TweetOauthTokenSecret"] = TwitterRt.OauthTokenSecret;
                ApplicationData.Current.RoamingSettings.Values["TweetUserID"] = TwitterRt.UserID;
                var frame = new Frame();
                frame.Navigate(typeof(MainPage));

                Window.Current.Content = frame;
                Window.Current.Activate();
            }
        }

        private async void btnConfigureLinkedIn_Click(object sender, RoutedEventArgs e)
        {
            String verificationCode = "";
            client = new Client(context);
            String requestTokenResponse = await client.MakeRequest("GET")
                    .ForRequestToken()
                    .Sign()
                    .ExecuteRequest();

            client.RequestToken = LinkedInRTLibrary.Credentials.TokenContainer.Parse(requestTokenResponse);
            Uri authorizationUri = client.GetAuthorizationUri();

            //Authorize the temporary token using the authorizationUri
            //One option is to use the supplied WebAuthenticationBroker

            WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, authorizationUri, client.Context.CallbackUri);

            //The verification code could be returned in the response
            //i.e. Parse it out of WebAuthenticationResult.ResponseData.ToString();
            //Or it could be displayed to the user and they will have to enter it into your application manually


            string[] VerificationCodePairs = WebAuthenticationResult.ResponseData.ToString().Split('&');

            for (int j = 0; j < VerificationCodePairs.Length; j++)
            {
                String[] VerificationTokensSplits = VerificationCodePairs[j].Split('=');
                switch (VerificationTokensSplits[0])
                {
                    case "oauth_verifier":
                        verificationCode = VerificationTokensSplits[1];
                        break;
                }
            }

            String accessTokenResponse = await client.MakeRequest("GET")
                    .ForAccessToken(client.RequestToken.Token, verificationCode)
                    .Sign(client.RequestToken.Secret)
                    .ExecuteRequest();

            client.AccessToken = LinkedInRTLibrary.Credentials.TokenContainer.Parse(accessTokenResponse);

            if (client.AccessToken != null)
            {
                ApplicationData.Current.RoamingSettings.Values["isLinkedInConfigure"] = "1";
                ApplicationData.Current.RoamingSettings.Values["LinkedInRequestToken"] = client.RequestToken.Token;
                ApplicationData.Current.RoamingSettings.Values["LinkedInRequestTokenSecret"] = client.RequestToken.Secret;
                ApplicationData.Current.RoamingSettings.Values["LinkedInVerificationCode"] = verificationCode;
                ApplicationData.Current.RoamingSettings.Values["LinkedInOauthToken"] = client.AccessToken.Token;
                ApplicationData.Current.RoamingSettings.Values["LinkedInOauthTokenSecret"] = client.AccessToken.Secret;
                var frame = new Frame();
                frame.Navigate(typeof(MainPage));

                Window.Current.Content = frame;
                Window.Current.Activate();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // If the user clicks the share button, invoke the share flow programatically.
            DataTransferManager.ShowShareUI();
            //this.Frame.Navigate(typeof(ShareIt));            
        }

        private void btnConfigureEmail_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = true;
        }


        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        public static bool IsEmail(string email)
        {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            ComboBoxItem cbi = (ComboBoxItem)domainPicker.SelectedItem;


            if (cbi == null)
            {
                //EmailConfigMessage.Text = loader.GetString("Please select the Domain.");
                EmailConfigMessage.Text = loader.GetString("Please select the Domain");
            }
            else if (txtEmail.Text == "")
            {
                //EmailConfigMessage.Text = loader.GetString("Please enter the Email.");
                EmailConfigMessage.Text = loader.GetString("Please enter the Email");
            }
            else if (!u.IsEmail(txtEmail.Text))
            {
                //EmailConfigMessage.Text = loader.GetString("Please enter the valid Email.");
                EmailConfigMessage.Text = loader.GetString("Please enter the valid Email");
            }
            else if (txtPassword.Password.ToString() != txtVerifyPassword.Password.ToString())
            {

                //EmailConfigMessage.Text = loader.GetString("Password and Veriy Password should be same.");
                EmailConfigMessage.Text = loader.GetString("Password and Veriy Password should be same");
            }
            else
            {
                serverType = cbi.Content.ToString();

                #region Selecting Domain

                if (serverType == "Hotmail")
                {
                    // test@hotmail.com, test@msn.com, test@live.com

                    if (txtEmail.Text.Split('@')[1].Split('.')[0].ToString() == "hotmail" ||
                        txtEmail.Text.Split('@')[1].Split('.')[0].ToString() == "msn" ||
                        txtEmail.Text.Split('@')[1].Split('.')[0].ToString() == "live")
                    {
                        EmailConfigMessage.Text = "";
                        serverDomain = "smtp.live.com";
                    }
                    else
                    {
                        //EmailConfigMessage.Text = loader.GetString("Please enter the Email which exists in the domain you selected.");
                        EmailConfigMessage.Text = loader.GetString("Please enter the Email which exists in the domain you selected");
                        return;
                    }
                }
                else if (serverType == "Yahoo")
                {
                    if (txtEmail.Text.Split('@')[1].Split('.')[0].ToString() == "yahoo")
                    {
                        EmailConfigMessage.Text = "";
                        serverDomain = "smtp.mail.yahoo.com";
                    }
                    else
                    {
                        //EmailConfigMessage.Text = loader.GetString("Please enter the Email which exists in the domain you selected.");
                        EmailConfigMessage.Text = loader.GetString("Please enter the Email which exists in the domain you selected");
                        return;
                    }
                }
                else if (serverType == "Google")
                {
                    serverDomain = "smtp.gmail.com";
                }
                #endregion

                #region Sending Test Mail
                string returnString = "";
                try
                {
                    //returnString = u.SendEmail(htmlmailer, serverDomain, serverType, txtEmail.Text, txtPassword.Password, txtEmail.Text, txtEmail.Text, "Test Email", "This is a test email from Share All.");
                    returnString = u.SendEmail(htmlmailer, serverDomain, serverType, txtEmail.Text, txtPassword.Password, txtEmail.Text, txtEmail.Text, loader.GetString("Test Email"), loader.GetString("This is a test email from Share All"));

                    if (returnString == "Success")
                    {
                        #region Encrypting Configuration Email and Password

                        entryptConfigEmail = u.AES_Encrypt(txtEmail.Text, "daksatech");
                        entryptConfigPwd = u.AES_Encrypt(txtPassword.Password.ToString(), "daksatech");

                        #endregion

                        // wrap the below around if else statements; only enable if email test was successful
                        ApplicationData.Current.RoamingSettings.Values["EmailServerType"] = serverType;
                        ApplicationData.Current.RoamingSettings.Values["EmailServerDomain"] = serverDomain;
                        ApplicationData.Current.RoamingSettings.Values["EmailConfigureEmail"] = entryptConfigEmail;
                        ApplicationData.Current.RoamingSettings.Values["EmailConfigurePwd"] = entryptConfigPwd;
                        ApplicationData.Current.RoamingSettings.Values["isEmailConfigure"] = 1;
                        //EmailConfigMessage.Text = loader.GetString("Email settings saved.");
                        EmailConfigMessage.Text = loader.GetString("Email settings saved");
                        myPopup.IsOpen = false;
                        var frame = new Frame();
                        frame.Navigate(typeof(MainPage));

                        Window.Current.Content = frame;
                        Window.Current.Activate();

                    }
                    else
                    {
                        //EmailConfigMessage.Text = loader.GetString("Unable to send Email now.");
                        EmailConfigMessage.Text = loader.GetString("Unable to send Email now");
                    }
                }
                catch (Exception ex)
                {
                    EmailConfigMessage.Text = ex.Message.ToString();
                }

                #endregion
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = false;
        }

        private void myPopup_Loaded_1(object sender, RoutedEventArgs e)
        {
            PopupLoaded();
        }

        private void PopupLoaded()
        {
            myPopup.HorizontalOffset = (Window.Current.Bounds.Width - myPopup.ActualWidth) / 2;
            myPopup.VerticalOffset = (Window.Current.Bounds.Height - mainGrid.ActualHeight) / 2;
        }
        //private void btnConfigureEmail_Click(object sender, RoutedEventArgs e)
        //{
        //   Frame.Navigate(typeof(ShareIt));

        //    //OnShareTargetActivated(ShareTargetActivatedEventArgs);
        //}

        //protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        //{
        //    var rootFrame = new Frame();
        //    rootFrame.Navigate(typeof(MainPage), args.ShareOperation);
        //    Window.Current.Content = rootFrame;
        //    Window.Current.Activate();
        //}

    }
}
