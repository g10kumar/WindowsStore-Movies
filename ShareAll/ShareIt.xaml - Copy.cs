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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Documents;
using Facebook;
using System.Dynamic;
using TwitterRtLibrary;

using Windows.Security.Authentication.Web;
using Windows.UI.ApplicationSettings;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ShareAll
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShareIt : ShareAll.Common.LayoutAwarePage
    {
        ShareOperation shareOperation;
        private string sharedDataTitle;
        private string sharedDataDescription;
        private string sharedText;
        private Uri sharedUri;
        private const string dataFormatName = "http://schema.org/Book";
        //string _facebookAppId = "160672697306624"; // You must set your own AppId here
        string _facebookAppId = "521064607905686"; // You must set your own AppId here
        string _permissions = "user_about_me,read_stream,publish_stream"; // Set your permissions here
        FacebookClient _fb = new FacebookClient();
        string sText = "";
        string finalSharedText = "";

        #region Facebook
        bool tweeetSuccess;
        private string _userId;
        #endregion
        public ShareIt()
        {
            this.InitializeComponent();
            //TwitterRt = new TwitterRt("EhY3grAWGzIxF0YXPr70Yw", "uwXl6wNaLowV9No68bNHtQNbMhSpSSgXCXEk4P0g", "http://www.daksatech.com");
            TwitterRt = new TwitterRt("OGAYZv3HDfykHsQw5dtng", "80cs06pnqehabIhv6PY1Vy1RuZX26G5twxJn6Oti0yU", "http://www.daksatech.com");

        }

        public TwitterRt TwitterRt { get; private set; }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>       

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values.Clear();
            // It is recommended to only retrieve the ShareOperation object in the activation handler, return as
            // quickly as possible, and retrieve all data from the share target asynchronously.

            this.shareOperation = (ShareOperation)e.Parameter;

            var unused = Task.Factory.StartNew(async () =>
            {
                // Retrieve the data package properties.
                this.sharedDataTitle = this.shareOperation.Data.Properties.Title;
                this.sharedDataDescription = this.shareOperation.Data.Properties.Description;

                // Retrieve the data package content.
                if (this.shareOperation.Data.Contains(StandardDataFormats.Text))
                {
                    try
                    {
                        this.sharedText = await this.shareOperation.Data.GetTextAsync();
                    }
                    catch (Exception ex)
                    {
                        Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                        dialog.ShowAsync();
                    }
                }
                // In this sample, we just display the shared data content.

                // Get back to the UI thread using the dispatcher.
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    #region Facebook, Twitter, Email
                    if (this.sharedDataTitle != null)
                    {
                        FBTitle.Text = this.sharedDataTitle;
                        //EmailSubject.Text = this.sharedDataTitle;
                    }

                    if (this.sharedText != null)
                    {
                        FBMessage.Text = this.sharedText;
                        sText = this.sharedText;
                        //TweetMessage.Text = this.sharedText;
                        //EmailMessage.Text = this.sharedText;
                    }
                    else if (this.sharedDataDescription != null)
                    {
                        FBMessage.Text = this.sharedDataDescription.ToString();
                        sText = this.sharedText;
                        //TweetMessage.Text = this.sharedDataDescription.ToString();
                        //EmailMessage.Text = this.sharedDataDescription.ToString();
                    }

                    //if (this.sharedText != null)
                    //{
                    //    sText = this.sharedText;
                    //}
                    //else if (this.sharedDataDescription != null)
                    //{
                    //    sText = this.sharedDataDescription.ToString();
                    //}

                    TweetMessage.Text = ExtractStringUsingRegularExpression(sText);

                    if (this.sharedUri != null)
                        FBLinktoShare.Text = this.sharedUri.ToString();
                    #endregion



                });
            });
            //sText = @"This is a test message with a link to https://dev.twitter.com/docs/tco-link-wrapper/faq And the message continues and is very long. https://www.google.com/ .";
            //sText = @"This is a test message with a link to https://dev.twitter.com/docs/tco-link-wrapper/faq And the message continues and is very long. And the message continues and is very long. And the message continues and is very long.";
            //TweetMessage.Text = ExtractStringUsingRegularExpression(sText);

            EnableConfigurationOptions();
        }

        private string ExtractStringUsingRegularExpression(string sText)
        {
            #region using Regular Expression
            string returnText = "";
            string orginalText = "";
            string tmpText = "";
            orginalText = sText;

            if (sText.Length >= 140)
            {
                tmpText = sText;

                List<string> listURL = new List<string>();
                //Regex linkRegex = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&amp;=]*)?");
                //Regex linkRegex = new Regex(@"(http(s)?://)?([\w-]+\.)+[\w-]+[\w-]+[\.]+[\.com]+([./?%&=]*)?");
                Regex linkRegex = new Regex(@"(http|ftp|https)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                //Regex linkRegex = new Regex(@"(.*)(\s)(?<link>\w+:\/\/[\w@][\w.:@]+\/?[\w\.?=%&=\-@#/$,]*)(.*)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //This is a test message with a link to And the message continues and is very long. And the message continues and is v...

                MatchCollection linkCollection = linkRegex.Matches(sText);

                foreach (Match match in linkCollection)
                {
                    listURL.Add(match.Groups[0].Index+"#"+match.Groups[0].Value);
                }

                if (listURL.Count > 0)
                { 
                    for(int i=0;i<listURL.Count;i++)
                    {
                        sText = sText.Replace(listURL[i].Split('#')[1].ToString(), "");
                    }

                    if (sText.Length > 140-(listURL.Count*20))
                    {
                        sText = sText.Substring(0, 140 - (listURL.Count * 20)-3);
                        for (int i = 0; i < listURL.Count; i++)
                        {
                            sText = sText.Insert(Convert.ToInt32(listURL[i].Split('#')[0]), listURL[i].Split('#')[1].ToString());
                        }
                        sText = sText + "...";

                        //if (sText.Length > 120 && sText.Length < 135)
                        //{
                        //    sText = sText + "...";
                        //}
                        //else
                        //{
                        //    sText = sText.Replace(listURL[0].Split('#')[1].ToString(), "");
                        //    sText = sText + "...";
                        //}
                    }
                    else
                    {
                        for (int i = 0; i < listURL.Count; i++)
                        {
                            sText = sText.Insert(Convert.ToInt32(listURL[i].Split('#')[0]), listURL[i].Split('#')[1].ToString());

                            if (sText.Length > 120 && sText.Length < 135)
                            {
                                sText = sText + "...";
                                break;
                            }
                        }
                    }
                }
                
                returnText = sText;
            }
            else
            {
                returnText = orginalText;
            }
            #endregion

            return returnText;
        }

        private void EnableConfigurationOptions()
        {
            if(ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isFBConfigure"].ToString() == "1")
            //if (sessionData.isFBConfigure)
            {
                btnFBConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                chkFacebook.IsEnabled = true;
                if (chkFacebook.IsChecked == false)
                {
                    chkFacebook.IsChecked = true;
                }
            }
            else
            {
                btnFBConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                chkFacebook.IsEnabled = false;
                if (chkFacebook.IsChecked == true)
                {
                    chkFacebook.IsChecked = false;
                }
            }

            //if (sessionData.isTweetConfigure)
            if (ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"].ToString() == "1")
            {
                btnTweetConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                chkTweet.IsEnabled = true;
                if (chkTweet.IsChecked == false)
                {
                    chkTweet.IsChecked = true;
                }
            }
            else
            {
                chkTweet.IsEnabled = false;
                if (chkTweet.IsChecked == true)
                {
                    chkTweet.IsChecked = false;
                }
                btnTweetConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            //if (ApplicationData.Current.RoamingSettings.Values["isEmailConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isEmailConfigure"].ToString() == "1")
            //{
            //    btnEmailConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //    chkEmail.IsEnabled = true;
            //    if (chkEmail.IsChecked == false)
            //    {
            //        chkEmail.IsChecked = true;
            //    }
            //}
            //else
            //{
            //    chkEmail.IsEnabled = false;
            //    if (chkEmail.IsChecked == true)
            //    {
            //        chkEmail.IsChecked = false;
            //    }
            //    btnEmailConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //}
        }

        private void chkFacebook_Checked_1(object sender, RoutedEventArgs e)
        {
            if (chkFacebook.IsChecked == true)
            {
                chkFacebook.IsChecked = false;
            }
            else
            {
                chkFacebook.IsChecked = true;
            }
        }

        private void btnFacebook_Click(object sender, RoutedEventArgs e)
        {
            if (stackFacebook.Visibility == Windows.UI.Xaml.Visibility.Visible)
                stackFacebook.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            else
                stackFacebook.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void btnTweet_Click(object sender, RoutedEventArgs e)
        {
            if (stackTweet.Visibility == Windows.UI.Xaml.Visibility.Visible)
                stackTweet.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            else
                stackTweet.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        //private void btnEmail_Click(object sender, RoutedEventArgs e)
        //{
        //    if (stackEmail.Visibility == Windows.UI.Xaml.Visibility.Visible)
        //        stackEmail.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //    else
        //        stackEmail.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //}

        private async void btnShare_Click(object sender, RoutedEventArgs e)
        {


            #region Facebook
            if (chkFacebook.IsChecked == true)
            {
                dynamic parameters = new ExpandoObject();
                parameters.message = FBMessage.Text;
                parameters.title = FBTitle.Text;
                parameters.access_token = ApplicationData.Current.RoamingSettings.Values["FBAccessToken"].ToString();
                dynamic result = await _fb.PostTaskAsync("me/feed", parameters);
                
                if (result.id != null)
                {
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Posted to your Facebook wall.");
                    await dialog.ShowAsync();
                }
                else
                {
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Post to wall failed. Please try again.");
                    await dialog.ShowAsync();
                }

            }
            #endregion

            #region Twitter
            if (chkTweet.IsChecked == true)
            {
                try
                {
                tweeetSuccess = await TwitterRt.UpdateStatus(TweetMessage.Text);
                }
                catch (Exception ex)
                {
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                    dialog.ShowAsync();
                }



                if (tweeetSuccess)
                {
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Posted to Twitter.");
                    await dialog.ShowAsync();
                }
                else
                {
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Post to Twitter failed. Please try again.");
                    await dialog.ShowAsync();
                }
            }
            #endregion


            #region Email
            //if (chkEmail.IsChecked == true)
            //{
            //    var mailto = new Uri("mailto:?to=" + EmailTo.Text + "&subject=" + EmailSubject.Text + "&body=" + EmailMessage.Text);
            //    await Windows.System.Launcher.LaunchUriAsync(mailto);
            //}
            #endregion

            #region Commented
            //try
            //{
            //    #region Facebook
            //    if (chkFacebook.IsChecked == true)
            //    {
            //        dynamic parameters = new ExpandoObject();
            //        parameters.message = FBMessage.Text;
            //        parameters.title = FBTitle.Text;
            //        parameters.access_token = ApplicationData.Current.RoamingSettings.Values["FBAccessToken"].ToString();
            //        dynamic result = await _fb.PostTaskAsync("me/feed", parameters);

            //    }
            //    #endregion

            //    #region Twitter
            //    if (chkTweet.IsChecked == true)
            //    {
            //        await TwitterRt.UpdateStatus(TweetMessage.Text + "  " + DateTime.Now);
            //    }
            //    #endregion

            //    #region Email
            //    //var mailto = new Uri("mailto:?to=" + EmailTo.Text + "&subject=" + EmailSubject.Text + "&body=" + EmailMessage.Text);
            //    //await Windows.System.Launcher.LaunchUriAsync(mailto);
            //    #endregion

            //}
            //catch (FacebookApiException fbex)
            //{
            //    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(fbex.Message.ToString());
            //    dialog.ShowAsync();
            //}
            //catch (Exception ex)
            //{
            //    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
            //    dialog.ShowAsync();
            //}
            #endregion
        }

        private async void btnFBConfigure_Click(object sender, RoutedEventArgs e)
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
            ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] = "1";
            //sessionData.isFBConfigure = true;
            EnableConfigurationOptions();

        }

        private async void btnTweetConfigure_Click(object sender, RoutedEventArgs e)
        {
            await TwitterRt.GainAccessToTwitter();
            if (TwitterRt.Status == "Access granted")
            {
                //sessionData.isTweetConfigure = true;
                ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] = "1";
                ApplicationData.Current.RoamingSettings.Values["TweetOauthToken"] = TwitterRt.OauthToken;
                ApplicationData.Current.RoamingSettings.Values["TweetOauthTokenSecret"] = TwitterRt.OauthTokenSecret;
                ApplicationData.Current.RoamingSettings.Values["TweetUserID"] = TwitterRt.UserID;
                EnableConfigurationOptions();
            }
            else
            {
                //sessionData.isTweetConfigure = false;
                ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] = "0";
                EnableConfigurationOptions();
            }

        }

        //private void btnEmailConfigure_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
