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
using System.Text.RegularExpressions;
using Windows.UI;

using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using nsoftware.IPWorksSSL;
using ShareAll.Common;
using Windows.ApplicationModel.Resources;
using LinkedInRTLibrary;
using Newtonsoft.Json;
namespace ShareAll
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShareIt : ShareAll.Common.LayoutAwarePage
    {
        #region Objects
        //dynamic parameters = new ExpandoObject();
        ShareOperation shareOperation;
        private string sharedDataTitle;
        private string sharedDataDescription;
        private string shareQuickLinkId;
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

        #region Email
        string entryptConfigEmail = "";
        string entryptConfigPwd = "";
        string serverType = "";
        string serverDomain = "";
        Htmlmailers htmlmailer = new Htmlmailers();
        public const string MatchEmailPattern = @"\w+([-+.]\w+)*@(yahoo|gmail|hotmail|msn|live)\.com";
        Utilities u = new Utilities();
        bool checkValidEmail = true;
        #endregion

        #region WordPress
        string _wppermissions = "publish_stream";
        public const string _consumerKey = "1434";
        public const string _consumerSecret = "NE6igTB5q6xDCM72ksBReGY4tqJdNOMSDb68RPno18pm4L8o36t6ba9ZxkziiyVV";
        public const string _callbackUrl = "http://www.daksatech.com";
        public string wpcode = "";
        OAuthUtil oAuthUtil = new OAuthUtil();
        #endregion

        #endregion

        public ShareIt()
        {
            this.InitializeComponent();
            //TwitterRt = new TwitterRt("EhY3grAWGzIxF0YXPr70Yw", "uwXl6wNaLowV9No68bNHtQNbMhSpSSgXCXEk4P0g", "http://www.daksatech.com");
            TwitterRt = new TwitterRt("OGAYZv3HDfykHsQw5dtng", "80cs06pnqehabIhv6PY1Vy1RuZX26G5twxJn6Oti0yU", "http://www.daksatech.com");
            context = new OAuthContext("kvpcelszx805", "yZyeKbgdvNTYsk8T", "https://api.linkedin.com/uas/oauth/requestToken", "https://api.linkedin.com/uas/oauth/authorize", "https://api.linkedin.com/uas/oauth/accessToken", "http://www.daksatech.com/");
            htmlmailer.OnSSLServerAuthentication += htmlmailer_OnSSLServerAuthentication;

        }

        void htmlmailer_OnSSLServerAuthentication(object sender, HtmlmailersSSLServerAuthenticationEventArgs e)
        {
            e.Accept = true;
            
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
            // ApplicationData.Current.RoamingSettings.Values.Clear();
            // It is recommended to only retrieve the ShareOperation object in the activation handler, return as
            // quickly as possible, and retrieve all data from the share target asynchronously.

            this.shareOperation = (ShareOperation)e.Parameter;

            var unused = Task.Factory.StartNew(async () =>
            {
                // Retrieve the data package properties.
                this.sharedDataTitle = this.shareOperation.Data.Properties.Title;
                this.sharedDataDescription = this.shareOperation.Data.Properties.Description;
                this.shareQuickLinkId = this.shareOperation.QuickLinkId;

                if (this.shareOperation.Data.Contains(StandardDataFormats.Uri))
                {
                    // The GetUriAsync(), GetTextAsync(), GetStorageItemsAsync(), etc. APIs will throw if there was an error retrieving the data from the source app.
                    // In this sample, we just display the error. It is recommended that a share target app handles these in a way appropriate for that particular app.
                    try
                    {
                        this.sharedUri = await this.shareOperation.Data.GetUriAsync();
                    }
                    catch (Exception ex)
                    {
                        Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                        dialog.ShowAsync();
                    }
                }
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
                        WPTitle.Text = this.sharedDataTitle;
                        EmailSubject.Text = this.sharedDataTitle;
                    }

                    if (this.sharedText != null)
                    {
                        FBMessage.Text = this.sharedText;
                        WPMessage.Text = this.sharedText;

                        LinkedInMessage.Text = this.sharedText;
                        sText = this.sharedText;
                        //TweetMessage.Text = this.sharedText;
                        EmailMessage.Text = this.sharedText;
                    }
                    else if (this.sharedDataDescription != null)
                    {
                        FBMessage.Text = this.sharedDataDescription.ToString();
                        WPMessage.Text = this.sharedDataDescription.ToString();
                        LinkedInMessage.Text = this.sharedDataDescription.ToString();
                        sText = this.sharedDataDescription.ToString();
                        //TweetMessage.Text = this.sharedDataDescription.ToString();
                        EmailMessage.Text = this.sharedDataDescription.ToString();
                    }

                    //if (this.sharedText != null)
                    //{
                    //    sText = this.sharedText;
                    //}
                    //else if (this.sharedDataDescription != null)
                    //{
                    //    sText = this.sharedDataDescription.ToString();
                    //}

                    //TweetMessage.Text = ExtractStringUsingRegularExpression(sText);

                    string extractedLinkFromTwitterText = ExtractLinkFromText(this.sText);
                    string extractedLink = ExtractLinkFromText(this.sharedDataDescription.ToString());

                    if (this.sharedUri != null)
                    {
                        if (extractedLinkFromTwitterText == "")
                        {
                            TweetMessage.Text = sText + " " + this.sharedUri.ToString();
                        }
                        else
                        {
                            if (this.sharedDataTitle != null & this.sharedDataTitle != "" & sText == extractedLinkFromTwitterText)
                            {
                                TweetMessage.Text = this.sharedDataTitle + " " + sText;
                            }
                            else
                            {
                                TweetMessage.Text = sText;
                            }
                        }
                    }
                    else
                    {
                        if (extractedLink != "")
                        {
                            TweetMessage.Text = sText + " " + extractedLink;
                        }
                        else
                        {
                            TweetMessage.Text = sText;
                        }
                        
                    }
                    
                    EnableTwitterorNot(TweetMessage.Text.Trim());
                    
                    
                    if (this.sharedUri != null)
                    {
                        FBLinktoShare.Text = this.sharedUri.ToString();
                    }
                    else
                    {
                        //string extractedLink = ExtractLinkFromText(this.sharedDataDescription.ToString());
                        if (extractedLink != "")
                        {
                            FBLinktoShare.Text = extractedLink;
                        }
                    }
                    #endregion



                });
            });
            //sText = @"This is a test message with a link to https://dev.twitter.com/docs/tco-link-wrapper/faq And the message continues and is very long. https://www.google.com/ .";
            //sText = @"This is a test message with a link to https://dev.twitter.com/docs/tco-link-wrapper/faq And the message continues and is very long. And the message continues and is very long. And the message continues and is very long.";
            //TweetMessage.Text = ExtractStringUsingRegularExpression(sText);
            //ApplicationData.Current.RoamingSettings.Values["isEmailConfigure"] = 0;
            EnableConfigurationOptions();

            //TweetMessage.Text = "Metro style apps are full screen apps tailored to your users' needs, tailored to the device they run on, tailored for touch interaction, and tailored to the Windows user interface. Windows helps you interact with your users, and your users interact with your app.";
            //EnableTwitterorNot(TweetMessage.Text.Trim());

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
                    listURL.Add(match.Groups[0].Index + "#" + match.Groups[0].Value);
                }

                if (listURL.Count > 0)
                {
                    for (int i = 0; i < listURL.Count; i++)
                    {
                        sText = sText.Replace(listURL[i].Split('#')[1].ToString(), "");
                    }

                    if (sText.Length > 140 - (listURL.Count * 20))
                    {
                        sText = sText.Substring(0, 140 - (listURL.Count * 20) - 3);
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
            if (ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isFBConfigure"].ToString() == "1")
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

            if (ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"].ToString() == "1")
            {
                if (TweetMessage.Text.Length <= 140)
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

            if (ApplicationData.Current.RoamingSettings.Values["isEmailConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isEmailConfigure"].ToString() == "1")
            {
                btnEmailConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                chkEmail.IsEnabled = false;
                //if (chkEmail.IsChecked == false)
                //{
                //    chkEmail.IsChecked = true;
                //}
            }
            else
            {
                chkEmail.IsEnabled = false;
                if (chkEmail.IsChecked == true)
                {
                    chkEmail.IsChecked = false;
                }
                btnEmailConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            if (ApplicationData.Current.RoamingSettings.Values["isLinkedInConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isLinkedInConfigure"].ToString() == "1")
            {
                btnLinkedInConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                chkLinkedin.IsEnabled = true;
                if (chkLinkedin.IsChecked == false)
                {
                    chkLinkedin.IsChecked = true;
                }

            }
            else
            {
                chkLinkedin.IsEnabled = false;
                if (chkLinkedin.IsChecked == true)
                {
                    chkLinkedin.IsChecked = false;
                }
                btnLinkedInConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            if (ApplicationData.Current.RoamingSettings.Values["isWordPressConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isWordPressConfigure"].ToString() == "1")
            {
                btnWPConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                chkWordPress.IsEnabled = true;
                if (chkWordPress.IsChecked == false)
                {
                    chkWordPress.IsChecked = true;
                }
            }
            else
            {
                btnWPConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                chkWordPress.IsEnabled = false;
                if (chkWordPress.IsChecked == true)
                {
                    chkWordPress.IsChecked = false;
                }
            }
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

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            
            fbPostMessage.Text = "";
            TwitterPostMessage.Text = "";
            if (FBLinktoShare.Text == loader.GetString("Link"))
            {
                FBLinktoShare.Text = "";
            }
            
            #region Facebook
            if (chkFacebook.IsChecked == true)
            {
				try
                {
                    shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    string _lastMessageId;
                    dynamic parameters = new ExpandoObject();
                    parameters.message = FBMessage.Text;
                    //parameters.title = FBTitle.Text;
                    parameters.name = FBTitle.Text;

                    string fbLink = ExtractLinkFromText(FBLinktoShare.Text);
                    //if (FBLinktoShare.Text != "") & FBLinktoShare.Text.Substring(0, 4) == "http")
                    if (fbLink != "")
                    {
                        parameters.link = FBLinktoShare.Text.ToString();
                    }
                    parameters.access_token = ApplicationData.Current.RoamingSettings.Values["FBAccessToken"].ToString();

                    dynamic result = await _fb.PostTaskAsync("me/feed", parameters);
                    
                    if (result.id != null)
                    {
                        _lastMessageId = result.id;
                        //await _fb.DeleteTaskAsync(_lastMessageId);
                        //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Posted to your Facebook wall.");
                        //await dialog.ShowAsync();
                        //fbPostMessage.Text = loader.GetString("Posted to Facebook.");
                        fbPostMessage.Text = loader.GetString("Posted to Facebook");
                        fbPostMessage.Foreground = new SolidColorBrush(Colors.Green);
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else
                    {
                        //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Post to wall failed. Please try again.");
                        //await dialog.ShowAsync();
                        //fbPostMessage.Text = loader.GetString("Post to Facebook failed. Please try again.");
                        fbPostMessage.Text = loader.GetString("Post to Facebook failed_Please try again");
                        fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }

                }
                catch (FacebookOAuthException foauthex)
                {
                    //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(foauthex.Message.ToString());
                    //dialog.ShowAsync();
                    if (foauthex.ErrorCode == 506)
                    {
                        //fbPostMessage.Text = loader.GetString("Post to Facebook failed. Message already exists on Facebook.");
                        fbPostMessage.Text = loader.GetString("Post to Facebook failed_Message already exists on Facebook");
                        fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else if (foauthex.ErrorCode >= 100 & foauthex.ErrorCode < 200)
                    {
                        //fbPostMessage.Text = loader.GetString("Post to Facebook failed. Please click on Settings icon and authorize the app before trying again.");
                        fbPostMessage.Text = loader.GetString("Facebook_Authorize_Error");
                        fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] = 0;
                        btnFBConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else
                    {
                        if (foauthex.Message.IndexOf("Error validating access token") != 0 || foauthex.Message.IndexOf("Invalid OAuth access token") != 0 || foauthex.Message.IndexOf("An active access token must be used") != 0)
                        {
                            //fbPostMessage.Text = loader.GetString("Post to Facebook failed. Please click on Settings icon and authorize the app before trying again.");
                            fbPostMessage.Text = loader.GetString("Facebook_Authorize_Error");
                            fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                            ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] = 0;
                            btnFBConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                            shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        }
                        else
                        {
                            //fbPostMessage.Text = loader.GetString("Post to Facebook failed. ") + " " + foauthex.Message.ToString();
                            fbPostMessage.Text = loader.GetString("Post to Facebook failed") + " " + foauthex.Message.ToString();
                            fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                            shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        }
                    }
                }
                catch (FacebookApiLimitException flimitex)
                {
                    //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(flimitex.Message.ToString());
                    //dialog.ShowAsync();
                    //fbPostMessage.Text = loader.GetString("Post to Facebook failed. ") + " " + flimitex.Message.ToString();
                    fbPostMessage.Text = loader.GetString("Post to Facebook failed") + " " + flimitex.Message.ToString();
                    fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                catch (FacebookApiException fapiex)
                {
                    //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(fapiex.Message.ToString());
                    //dialog.ShowAsync();
                    //fbPostMessage.Text = loader.GetString("Post to Facebook failed. ") + " " + fapiex.Message.ToString();
                    fbPostMessage.Text = loader.GetString("Post to Facebook failed") + " " + fapiex.Message.ToString();
                    fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                    //dialog.ShowAsync();
                    //fbPostMessage.Text = loader.GetString("Post to Facebook failed. ") + " " + ex.Message.ToString();
                    fbPostMessage.Text = loader.GetString("Post to Facebook failed") + " " + ex.Message.ToString();
                    fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

            }
            #endregion
            //This is a test message with a link to https://dev.twitter.com/docs/tco-link-wrapper/faq And the message continues and is very long.  And the message continues and is ve.
            #region Twitter
            if (chkTweet.IsChecked == true)
            {
                try
                {
                    shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    tweeetSuccess = await TwitterRt.UpdateStatus(TweetMessage.Text);
                    
                    if (tweeetSuccess)
                    {
                        //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Posted to Twitter.");
                        //await dialog.ShowAsync();
                        //TwitterPostMessage.Text = loader.GetString("Posted to Twitter.");
                        TwitterPostMessage.Text = loader.GetString("Posted to Twitter");
                        TwitterPostMessage.Foreground = new SolidColorBrush(Colors.Green);
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else
                    {
                        Regex twitterStatusRe = new Regex(@"(\()([0-9]{3,3})(\))");
                        MatchCollection statusCodeCollection = twitterStatusRe.Matches(TwitterRt.Status.ToString());
                        string twitterErrorMessage = "";
                        if (statusCodeCollection.Count > 0)
                        {
                            switch (statusCodeCollection[0].Groups[2].ToString())
                            {
                                // twitter response codes here: https://dev.twitter.com/docs/error-codes-responses
                                case "400":
                                case "401":
                                    ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] = 0;
                                    EnableTwitterOptions();
                                    //twitterErrorMessage = loader.GetString("Could not post to Twitter because of authentication issue. Please click Settings icon and authorize app.");
                                    twitterErrorMessage = loader.GetString("Twitter_Error_401");
                                    break;
                                case "403":
                                    //twitterErrorMessage = loader.GetString("The post was rejected by Twitter.");
                                    twitterErrorMessage = loader.GetString("The post was rejected by Twitter");
                                    break;
                                case "404":
                                    twitterErrorMessage = "";
                                    break;
                                case "406":
                                    twitterErrorMessage = "";
                                    break;
                                case "420":
                                    twitterErrorMessage = "";
                                    break;
                                case "422":
                                    //twitterErrorMessage = loader.GetString("Could not post image to Twitter. Try again later.");
                                    twitterErrorMessage = loader.GetString("Twitter_Error_422");
                                    break;
                                case "429":
                                    //twitterErrorMessage = loader.GetString("Could not post to Twitter. Try again later.");
                                    twitterErrorMessage = loader.GetString("Could not post to Twitter_Try again later");
                                    break;
                                case "500":
                                case "502":
                                case "503":
                                case "504":
                                    //twitterErrorMessage = loader.GetString("Could not post to Twitter. It seems to be overloaded. Try again later.");
                                    twitterErrorMessage = loader.GetString("Twitter_Error_500To504");
                                    break;
                                default:
                                    twitterErrorMessage = "";
                                    break;

                            }
                        }

                        TwitterPostMessage.Text = twitterErrorMessage;
                        TwitterPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    }
                }
                
                catch (Exception ex)
                {
                    if (ex.Message.ToString() == "Access is denied. (Exception from HRESULT: 0x80070005 (E_ACCESSDENIED))" || TwitterRt.Status ==  "Access is denied. (Exception from HRESULT: 0x80070005 (E_ACCESSDENIED))")
                    {
                        ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] = 0;
                        EnableTwitterOptions();

                        //TwitterPostMessage.Text = loader.GetString("Could not post to Twitter. Please click on Settings icon and authorize the app.");
                        TwitterPostMessage.Text = loader.GetString("Could not post to Twitter_Please click on Settings icon and authorize the app");
                        TwitterPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else
                    {
                        //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                        //dialog.ShowAsync();
                        //TwitterPostMessage.Text = loader.GetString("Could not post to Twitter. ") + " " + ex.Message.ToString();
                        TwitterPostMessage.Text = loader.GetString("Could not post to Twitter") + " " + ex.Message.ToString();
                        TwitterPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                }
                
            }
            #endregion

            #region Email
            //if (chkEmail.IsChecked == true)
            //{
            //    var mailto = new Uri("mailto:?to=" + EmailTo.Text + "&subject=" + EmailSubject.Text + "&body=" + EmailMessage.Text);
            //    await Windows.System.Launcher.LaunchUriAsync(mailto);
            //}
            if (chkEmail.IsChecked == true)
            {
                shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                string mailType = "";
                string mailServer = "";
                string mailFromAddress = "";
                string mailFromAddressPwd = "";
                string mailToEmails = "";
                string[] mailToEmailsArray = null;
                 mailType = ApplicationData.Current.RoamingSettings.Values["EmailServerType"].ToString();
                 mailServer = ApplicationData.Current.RoamingSettings.Values["EmailServerDomain"].ToString();
                 mailFromAddress =  AES_Decrypt(ApplicationData.Current.RoamingSettings.Values["EmailConfigureEmail"].ToString(),"daksatech");
                 mailFromAddressPwd = AES_Decrypt(ApplicationData.Current.RoamingSettings.Values["EmailConfigurePwd"].ToString(), "daksatech");

                #region Encrypting Sending Email Addreses

                if (EmailTo.Text.Contains(","))
                {
                    mailToEmailsArray = EmailTo.Text.Split(',');
                }
                else if (EmailTo.Text.Contains(" "))
                {
                    mailToEmailsArray = EmailTo.Text.Split(' ');
                }
                else if (EmailTo.Text.Contains(";"))
                {
                    mailToEmailsArray = EmailTo.Text.Split(';');
                }
                
                if (mailToEmailsArray != null)
                {
                    mailToEmails = createCSVEmailList(mailToEmailsArray, ",");
                }
                else
                {
                    mailToEmails = EmailTo.Text;
                }

                


                #endregion

                #region Sending Mail
                string returnString = "";
                try
                {
                    // sending test mail with serverType, ServerDomain,  toaddress and password, subject, message
                    //(mailType,mailServer,mailFromAddress,mailToEmails,mailFromAddressPwd, EmailSubject.Text, EmailMessage.Text)
                    htmlmailer.RuntimeLicense = "31534E3952413153554252414E5852465350454E434552544553540000000000000000000000000038344E394A483841000052424B414A3739394D3354440000";
                    htmlmailer.MailServer = mailServer;
                    

                    if (mailType != "Yahoo")
                    {
                        htmlmailer.SSLStartMode = HtmlmailersSSLStartModes.sslExplicit;
                        htmlmailer.MailPort = 587;
                    }
                    else
                    {
                        htmlmailer.MailPort = 465;
                    }
                    htmlmailer.User = mailFromAddress;
                    htmlmailer.Password = mailFromAddressPwd;
                    htmlmailer.From = mailFromAddress;
                    htmlmailer.SendTo = mailToEmails;
                    htmlmailer.Subject =  EmailSubject.Text;
                    htmlmailer.MessageHTML = EmailMessage.Text;
                    htmlmailer.Config("Hello=Metro"); //This can not be automatically determined in WinRT due to access restrictions.

                    string message = "";
                    try
                    {
                        await htmlmailer.SendAsync();
                        message = "Success";
                    }
                    catch (Exception ex)
                    {
                        message = "Error: " + ex.Message;
                    }

                    returnString = message;
                    if (returnString == "Success")
                    {
                        // wrap the below around if else statements; only enable if email test was successful
                        //EmailPostMessage.Text = loader.GetString("Mail was sent successfully.");
                        EmailPostMessage.Text = loader.GetString("Mail was sent successfully");
                        EmailPostMessage.Foreground = new SolidColorBrush(Colors.Green);
                        //EmailTo.Text = loader.GetString("To Emails (separate with comma, space or semi colon)");
                        EmailTo.Text = loader.GetString("To Emails (separate with comma, space or semi colon)");
                        EmailSubject.Text = "";
                        EmailMessage.Text = "";
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else
                    {
                        //EmailPostMessage.Text = loader.GetString("Unable to send mail. Check your 'To Email' address or 'Email settings'");
                        EmailPostMessage.Text = loader.GetString("Email_Failure");
                        EmailPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                }
                catch (Exception ex)
                {
                    EmailPostMessage.Text = ex.Message.ToString();
                    EmailPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

                #endregion
                
            }
            #endregion

            #region LinkedIn

            if (chkLinkedin.IsChecked == true)
            {
                try
                {
                    shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    Uri protectedResourceUri = new Uri("http://api.linkedin.com/v1/people/~/current-status");

                    string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                    xml += "<current-status>" + LinkedInMessage.Text + "</current-status>";

                    
                    client = new Client(context);                   
                    client.RequestToken = LinkedInRTLibrary.Credentials.TokenContainer.Parse("oauth_token=" + ApplicationData.Current.RoamingSettings.Values["LinkedInRequestToken"].ToString() + "&oauth_token_secret=" + ApplicationData.Current.RoamingSettings.Values["LinkedInRequestTokenSecret"].ToString());
                    client.AccessToken = LinkedInRTLibrary.Credentials.TokenContainer.Parse("oauth_token=" + ApplicationData.Current.RoamingSettings.Values["LinkedInOauthToken"].ToString() + "&oauth_token_secret=" + ApplicationData.Current.RoamingSettings.Values["LinkedInOauthTokenSecret"].ToString());
                    
                    String postResponse = await client.MakeRequest("PUT")
                                      .WithData(xml)
                                      .ForResource(ApplicationData.Current.RoamingSettings.Values["LinkedInOauthToken"].ToString(), protectedResourceUri)
                                      .Sign(ApplicationData.Current.RoamingSettings.Values["LinkedInOauthTokenSecret"].ToString())
                                      .ExecuteRequest();

                    if (postResponse == "")
                    {
                        LinkedInPostMessage.Text = loader.GetString("LinkedIn_Success");
                        LinkedInPostMessage.Foreground = new SolidColorBrush(Colors.Green);
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else
                    {
                        ApplicationData.Current.RoamingSettings.Values["isLinkedInConfigure"] = 0;
                        LinkedInPostMessage.Text = loader.GetString("LinkedIn_Failure");
                        LinkedInPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                }

                catch (Exception ex)
                {
                    ApplicationData.Current.RoamingSettings.Values["isLinkedInConfigure"] = 0;
                    LinkedInPostMessage.Text = loader.GetString("LinkedIn_Failure");
                    LinkedInPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

            }
            #endregion

            #region WordPress
            if (chkWordPress.IsChecked == true)
            {
                try
                {
                    shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    Post p = new Post();
                    p.title = WPTitle.Text;
                    p.description = WPMessage.Text + " " + DateTime.Now;
                    p.format = "standard";
                    p.categories = new string[] { "Uncategorized" };
                    p.tags = new string[] { "test" };

                    //var pp = new Post()
                    //{
                    //    title = "I Need a Good REST",
                    //    description = "That's probably the corniest blog title ever created...",
                    //    format = "standard",
                    //};

                    //var data = "title:" + p.title + ", content: " + p.description + ", format: " + p.format + ", tags:test, categories:Uncategorized";

                    //string sigBaseStringParamspost = "oauth_signature_method=" + "HMAC-SHA1";
                    //sigBaseStringParamspost += "&" + "oauth_timestamp=" + timeStamp;
                    //sigBaseStringParamspost += "&" + "title=" + p.title;
                    //sigBaseStringParamspost += "&" + "content=" + p.content;
                    //sigBaseStringParamspost += "&" + "format=" + p.format;
                    //sigBaseStringParamspost += "&" + "tags=tests";
                    //sigBaseStringParamspost += "&" + "categories=API";
                    //sigBaseStringParamspost += "&" + "Bearer=" + accesstoken;
                    //sigBaseStringParamspost += "&" + "oauth_version=1.0";


                    //metadata mdata = new metadata();
                    //mdata.tmp[0].key = "add";
                    //mdata.tmp[0].operation = "geo_address";
                    //mdata.tmp[0].value = "San Francisco, CA";

                    //string sigBaseStringParamspost = "title=" + p.title;
                    //sigBaseStringParamspost += "&" + "content=" + p.description;
                    //sigBaseStringParamspost += "&" + "format=" + p.format;
                    //sigBaseStringParamspost += "&" + "tags=" + p.tags;
                    //sigBaseStringParamspost += "&" + "categories=" + p.categories;
                    ////////sigBaseStringParamspost += "&" + "SITE_ID=" + blogid;
                    //////sigBaseStringParamspost += "&" + "ignore_errors=true";
                    //sigBaseStringParamspost += "&" + "Authorization: Bearer " + ApplicationData.Current.RoamingSettings.Values["WordPressAccessToken"].ToString();

                    //string sigBaseStringParamspost = "title=" + p.title;
                    //sigBaseStringParamspost += "&" + "description=" + p.content;
                    //sigBaseStringParamspost += "&" + "Authorization: Bearer " + accesstoken;

                    //string sigBaseStringParamspost = "Authorization: Bearer " + accesstoken;

                    string sigBaseStringParamspost = "Authorization: Bearer " + ApplicationData.Current.RoamingSettings.Values["WordPressAccessToken"].ToString();
                    //sigBaseStringParamspost += "&" + "client_id=1434";
                    //sigBaseStringParamspost += "&" + "oauth_signature_method=" + "HMAC-SHA1";
                    //sigBaseStringParamspost += "&" + "oauth_timestamp=" + timeStamp;
                    //sigBaseStringParamspost += "&" + "oauth_version=1.0";
                    sigBaseStringParamspost += "&" + "content=" + p;
                    //////string sigBaseStringParamspost = "data=" + pp;
                    //sigBaseStringParamspost += "&" + "Bearer " + accesstoken;

                    string sigBaseStringpost = "POST&";
                    sigBaseStringpost += Uri.EscapeDataString("https://public-api.wordpress.com/rest/v1/sites/" + ApplicationData.Current.RoamingSettings.Values["WordPressBlogId"].ToString() + "/posts/new/") + "&" + Uri.EscapeDataString(sigBaseStringParamspost);
                    //sigBaseStringpost += Uri.EscapeDataString("https://public-api.wordpress.com/rest/v1/sites/" + blogid + "/posts/new/" + "&" + sigBaseStringParamspost);
                    //sigBaseStringpost += Uri.EscapeDataString("https://public-api.wordpress.com/rest/v1/sites/" + blogurl + "/posts/new/?pretty=true") + "&" + Uri.EscapeDataString(sigBaseStringParamspost);

                    string signaturepost = oAuthUtil.GetSignature(sigBaseStringpost, _consumerSecret, null);

                    var postresponseText = await oAuthUtil.PostData("https://public-api.wordpress.com/rest/v1/sites/" + ApplicationData.Current.RoamingSettings.Values["WordPressBlogId"].ToString() + "/posts/new/", sigBaseStringParamspost + "&oauth_signature=" + Uri.EscapeDataString(signaturepost));
                    //var postresponseText = await oAuthUtil.PostData("https://public-api.wordpress.com/rest/v1/sites/" + blogurl + "/posts/new/?pretty=true", sigBaseStringParamspost + "&oauth_signature=" + Uri.EscapeDataString(signaturepost));

                    //{"error":"unauthorized","message":"User cannot publish posts"}

                    postresponseText = postresponseText.Replace(@"\", "");

                    if (!string.IsNullOrEmpty(postresponseText))
                    {
                        //return contents from json serialize object
                        var WPErrorItems = JsonConvert.DeserializeObject<WPError>(postresponseText);

                        if (WPErrorItems != null)
                        {
                            #region Error

                            if (WPErrorItems.error == null)
                            {
                                WPPostMessage.Text = "Posted to WordPress.";
                                WPPostMessage.Foreground = new SolidColorBrush(Colors.Green);
                                shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            }
                            else
                            {
                                ApplicationData.Current.RoamingSettings.Values["isWordPressConfigure"] = 0;
                                WPPostMessage.Text = WPErrorItems.message.ToString() + ".  Try again later.";                                
                                //WPPostMessage.Text = "Could not post to WordPress. Try again later.";
                                WPPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                                shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            }

                            #endregion
                        }
                    }                    
                }

                catch (Exception ex)
                {
                    ApplicationData.Current.RoamingSettings.Values["isWordPressConfigure"] = 0;
                    WPPostMessage.Text = "Could not post to WordPress. Try again later.";
                    WPPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    shareProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }

            }
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

        #region Facebook
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
            {
                stackFacebook.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imgFB.Source = new BitmapImage(new Uri("ms-appx:///Assets/ExpandBox.png", UriKind.Absolute));

            }
            else
            {
                stackFacebook.Visibility = Windows.UI.Xaml.Visibility.Visible;
                imgFB.Source = new BitmapImage(new Uri("ms-appx:///Assets/CollapseBox.png", UriKind.Absolute));
            }
        }

        private void FBLinktoShare_GotFocus_1(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            if (FBLinktoShare.Text == loader.GetString("Link"))
            {
                FBLinktoShare.Text = "";
            }

        }

        #endregion

        #region Twitter

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

        private void TweetMessage_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            EnableTwitterorNot(TweetMessage.Text.Trim());
        }

        private void btnTweet_Click(object sender, RoutedEventArgs e)
        {
            if (stackTweet.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                stackTweet.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imgTwitter.Source = new BitmapImage(new Uri("ms-appx:///Assets/ExpandBox.png", UriKind.Absolute));

            }
            else
            {
                stackTweet.Visibility = Windows.UI.Xaml.Visibility.Visible;
                imgTwitter.Source = new BitmapImage(new Uri("ms-appx:///Assets/CollapseBox.png", UriKind.Absolute));
            }
        }
        private void EnableTwitterOptions()
        {
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
        }

        public static string ReplaceFirstOccurrance(string original, string oldValue, string newValue)
        {
            if (String.IsNullOrEmpty(original))
                return String.Empty;
            if (String.IsNullOrEmpty(oldValue))
                return original;
            if (String.IsNullOrEmpty(newValue))
                newValue = String.Empty;
            int loc = original.IndexOf(oldValue);
            return original.Remove(loc, oldValue.Length).Insert(loc, newValue);
        }

        private string ExtractLinkFromText(string sText)
        {

            string extractedURL = "";
            Regex linkRegex = new Regex(@"(http|ftp|https)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection linkCollection = linkRegex.Matches(sText);
            if (linkCollection.Count > 0)
            {
                extractedURL = linkCollection[0].ToString();

            }

            return extractedURL;

        }

        private void EnableTwitterorNot(string sText)
        {
            List<string> listURL = new List<string>();
            //Regex linkRegex = new Regex(@"(http|ftp|https)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

            string reURLs = @"(?xi)
\b
(                       # Capture 1: entire matched URL
  (?:
    https?://               # http or https protocol
    |                       #   or
    www\d{0,3}[.]           # 'www.', 'www1.', 'www2.' … 'www999.'
    |
    [a-z0-9\-]{3,}\.
    |                           #   or
    [a-z0-9.\-]+[.][a-z]{2,4}/  # looks like domain name followed by a slash
  )
  (?:                       # One or more:
    [^\s()<>]+                  # Run of non-space, non-()<>
    |                           #   or
    \(([^\s()<>]+|(\([^\s()<>]+\)))*\)  # balanced parens, up to 2 levels
  )+
  (?:                       # End with:
    \(([^\s()<>]+|(\([^\s()<>]+\)))*\)  # balanced parens, up to 2 levels
    |                               #   or
    [^\s`!()\[\]{};:'"".,<>?«»“”‘’]        # not a space or one of these punct chars
  )
)";

            string[] gTLD = { "aero", "asia", "biz", "cat", "com", "coop", "edu", "gov", "info", "int", "jobs", "mil", "mobi", "museum", "name", "net", "org", "pro", "tel", "travel", "xxx" };
            string[] ccTLD = { "ac", "ad", "ae", "af", "ag", "ai", "al", "am", "an", "ao", "aq", "ar", "as", "at", "au", "aw", "ax", "az", "ba", "bb", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bm", "bn", "bo", "br", "bs", "bt", "bv", "bw", "by", "bz", "ca", "cc", "cd", "cf", "cg", "ch", "ci", "ck", "cl", "cm", "cn", "co", "cr", "cs", "cu", "cv", "cx", "cy", "cz", "dd", "de", "dj", "dk", "dm", "do", "dz", "ec", "ee", "eg", "eh", "er", "es", "et", "eu", "fi", "fj", "fk", "fm", "fo", "fr", "ga", "gb", "gd", "ge", "gf", "gg", "gh", "gi", "gl", "gm", "gn", "gp", "gq", "gr", "gs", "gt", "gu", "gw", "gy", "hk", "hm", "hn", "hr", "ht", "hu", "id", "ie", "il", "im", "in", "io", "iq", "ir", "is", "it", "je", "jm", "jo", "jp", "ke", "kg", "kh", "ki", "km", "kn", "kp", "kr", "kw", "ky", "kz", "la", "lb", "lc", "li", "lk", "lr", "ls", "lt", "lu", "lv", "ly", "ma", "mc", "md", "me", "mg", "mh", "mk", "ml", "mm", "mn", "mo", "mp", "mq", "mr", "ms", "mt", "mu", "mv", "mw", "mx", "my", "mz", "na", "nc", "ne", "nf", "ng", "ni", "nl", "no", "np", "nr", "nu", "nz", "om", "pa", "pe", "pf", "pg", "ph", "pk", "pl", "pm", "pn", "pr", "ps", "pt", "pw", "py", "qa", "re", "ro", "rs", "ru", "rw", "sa", "sb", "sc", "sd", "se", "sg", "sh", "si", "sj", "sk", "sl", "sm", "sn", "so", "sr", "ss", "st", "su", "sv", "sy", "sz", "tc", "td", "tf", "tg", "th", "tj", "tk", "tl", "tm", "tn", "to", "tp", "tr", "tt", "tv", "tw", "tz", "ua", "ug", "uk", "us", "uy", "uz", "va", "vc", "ve", "vg", "vi", "vn", "vu", "wf", "ws", "ye", "yt", "za", "zm", "zw" };


            Regex linkRegex = new Regex(reURLs, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection linkCollection = linkRegex.Matches(sText);




            foreach (Match match in linkCollection)
            {
                if (match.Groups[0].Value.Split(':').Count() == 1)
                {
                    string topDomain = match.Groups[0].Value.Split('.').Last().ToString();
                    if (gTLD.Contains(topDomain) | ccTLD.Contains(topDomain))
                    {
                        listURL.Add(match.Groups[0].Value);
                    }
                }
                else
                {
                    listURL.Add(match.Groups[0].Value);
                }
            }

            if (listURL.Count > 0)
            {
                for (int i = 0; i < listURL.Count; i++)
                {
                    sText = ReplaceFirstOccurrance(sText, listURL[i].ToString(), "01234567890123456789");
                }
            }

            int diff;

            diff = 140 - Convert.ToInt32(sText.Length);



            if (diff < 0)
            {
                txtCount.Text = diff.ToString();
                txtCount.Foreground = new SolidColorBrush(Colors.Red);
                btnTweetConfigure.IsEnabled = false;
                chkTweet.IsEnabled = false;
                chkTweet.IsChecked = false;
            }
            else
            {
                txtCount.Text = diff.ToString();
                txtCount.Foreground = new SolidColorBrush(Colors.Black);
                btnTweetConfigure.IsEnabled = true;
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
            }
        }
        #endregion

        #region Email
        private void btnEmailConfigure_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            #region Commented
            /*
            if (txtPassword.Password.ToString() == txtVerifyPassword.Password.ToString())
            {
                ComboBoxItem cbi = (ComboBoxItem)domainPicker.SelectedItem;
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
                        EmailConfigMessage.Text = "Please enter the Email which exists in the domain you selected.";
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
                        EmailConfigMessage.Text = "Please enter the Email which exists in the domain you selected.";
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
                    // sending test mail with serverType, ServerDomain,  toaddress and password, subject, message
                    //(serverType,ServerDomain,entryptConfigEmail,entryptConfigEmail,entryptConfigPwd,"Test Email","This is a test email from Share All.")
                    returnString = u.SendEmail(htmlmailer, serverDomain, serverType, txtEmail.Text, txtPassword.Password, txtEmail.Text, txtEmail.Text, "Test Email", "This is a test email from Share All.");

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
                        EmailConfigMessage.Text = "Email settings saved.";
                        EnableConfigurationOptions();
                        myPopup.IsOpen = false;

                    }
                    else
                    {
                        EmailConfigMessage.Text = "Unable to send Email now.";
                    }
                }
                catch (Exception ex)
                {
                    EmailConfigMessage.Text = ex.Message.ToString();
                }

                #endregion

            }
            else
            {
                EmailConfigMessage.Text = "Password and Veriy Password should be same.";
            }
            */
            #endregion

            ComboBoxItem cbi = (ComboBoxItem)domainPicker.SelectedItem;


            if (cbi == null)
            {
                //EmailConfigMessage.Text = loader.GetString("Please select the Domain");
                EmailConfigMessage.Text = loader.GetString("Please select the Domain");
            }
            else if (txtEmail.Text == "")
            {
                //EmailConfigMessage.Text = loader.GetString("Please enter Email to use for sending");
                EmailConfigMessage.Text = loader.GetString("Please enter Email to use for sending");
            }
            else if (!u.IsEmail(txtEmail.Text))
            {
                //EmailConfigMessage.Text = loader.GetString("Please enter valid Email");
                EmailConfigMessage.Text = loader.GetString("Please enter valid Email");
            }
            else if (txtPassword.Password.ToString() != txtVerifyPassword.Password.ToString())
            {

                //EmailConfigMessage.Text = loader.GetString("Password and Verify Password should be same");
                EmailConfigMessage.Text = loader.GetString("Password and Verify Password should be same");
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
                        //EmailConfigMessage.Text = loader.GetString("For Hotmail, valid emails are hotmail.com, msn.com & live.com");
                        EmailConfigMessage.Text = loader.GetString("Hotmail_Domain_Failure");
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
                        EnableConfigurationOptions();

                    }
                    else
                    {
                        //EmailConfigMessage.Text = loader.GetString("Unable to send mail now.");
                        EmailConfigMessage.Text = loader.GetString("Unable to send mail now");
                    }
                }
                catch (Exception ex)
                {
                    EmailConfigMessage.Text = ex.Message.ToString();
                }

                #endregion
            }
        }

        private string createCSVEmailList(string[] mailToEmailsArray, string delimiter)
        {
            string returnString = "";

            for (int i = 0; i < mailToEmailsArray.Length; i++)
            {
                returnString += mailToEmailsArray[i].ToString().Trim();

                if (i != (mailToEmailsArray.Length - 1))
                    returnString += delimiter;

            }

            return returnString;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = false;
        }

        public string AES_Encrypt(string input, string pass)
        {
            SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            CryptographicKey AES;
            HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash Hash_AES = HAP.CreateHash();

            string encrypted = "";
            try
            {
                byte[] hash = new byte[32];
                Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(pass)));
                byte[] temp;
                CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out temp);

                Array.Copy(temp, 0, hash, 0, 16);
                Array.Copy(temp, 0, hash, 15, 16);

                AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

                IBuffer Buffer = CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(input));
                encrypted = CryptographicBuffer.EncodeToBase64String(CryptographicEngine.Encrypt(AES, Buffer, null));

                return encrypted;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string AES_Decrypt(string input, string pass)
        {
            SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
            CryptographicKey AES;
            HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash Hash_AES = HAP.CreateHash();

            string decrypted = "";
            try
            {
                byte[] hash = new byte[32];
                Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(pass)));
                byte[] temp;
                CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out temp);

                Array.Copy(temp, 0, hash, 0, 16);
                Array.Copy(temp, 0, hash, 15, 16);

                AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

                IBuffer Buffer = CryptographicBuffer.DecodeFromBase64String(input);
                byte[] Decrypted;
                CryptographicBuffer.CopyToByteArray(CryptographicEngine.Decrypt(AES, Buffer, null), out Decrypted);
                decrypted = System.Text.Encoding.UTF8.GetString(Decrypted, 0, Decrypted.Length);

                return decrypted;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void EmailTo_GotFocus_1(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            //if (EmailTo.Text == loader.GetString("To Emails (separate with comma, space or semi colon)"))
            //{
            //    EmailTo.Text = "";
            //}
            if (EmailTo.Text == loader.GetString("To Emails (separate with comma, space or semi colon)"))
            {
                EmailTo.Text = "";
            }

        }

        private void EmailTo_LostFocus_1(object sender, RoutedEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            //if (EmailTo.Text == "")
            //{
            //    EmailTo.Text = loader.GetString("To Emails (separate with comma, space or semi colon)");
            //}
            if (EmailTo.Text == "")
            {
                EmailTo.Text = loader.GetString("To Emails (separate with comma, space or semi colon)");
            }
        }

        private void btnEmail_Click_1(object sender, RoutedEventArgs e)
        {
            if (stackEmail.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                stackEmail.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imgEmail.Source = new BitmapImage(new Uri("ms-appx:///Assets/ExpandBox.png", UriKind.Absolute));

            }
            else
            {
                stackEmail.Visibility = Windows.UI.Xaml.Visibility.Visible;
                imgEmail.Source = new BitmapImage(new Uri("ms-appx:///Assets/CollapseBox.png", UriKind.Absolute));
            }
        }

        private void EmailTo_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (checkValidEmail)
            {
                if (EmailTo.Text.Length > 6 & EmailTo.Text.IndexOf(".") > 1 & EmailTo.Text.IndexOf("@") > 0)
                {
                    if (u.IsValidEmailTo(EmailTo.Text))
                    {
                        chkEmail.IsEnabled = true;
                        checkValidEmail = false;
                    }
                }
            }
        }
        #endregion

        #region LinkedIn
        private async void btnLinkedInConfigure_Click(object sender, RoutedEventArgs e)
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
                EnableConfigurationOptions();
            }
            else
            {
                ApplicationData.Current.RoamingSettings.Values["isLinkedInConfigure"] = "0";
                EnableConfigurationOptions();
            }
        }

        private void btnLinkedIn_Click(object sender, RoutedEventArgs e)
        {
            if (stackLinkedIn.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                stackLinkedIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imgLinkedIn.Source = new BitmapImage(new Uri("ms-appx:///Assets/ExpandBox.png", UriKind.Absolute));

            }
            else
            {
                stackLinkedIn.Visibility = Windows.UI.Xaml.Visibility.Visible;
                imgLinkedIn.Source = new BitmapImage(new Uri("ms-appx:///Assets/CollapseBox.png", UriKind.Absolute));
            }
        }

        #endregion

        #region WordPress
        private async void btnWPConfigure_Click(object sender, RoutedEventArgs e)
        {
            System.Uri StartUri = new Uri("https://public-api.wordpress.com/oauth2/authorize?client_id=" + _consumerKey + "&redirect_uri=" + _callbackUrl + "&response_type=code&scope=" + _wppermissions);
            System.Uri EndUri = new Uri("http://www.daksatech.com");

            WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                    WebAuthenticationOptions.None,
                                                    StartUri,
                                                    EndUri);
            if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                String[] keyValPairs = WebAuthenticationResult.ResponseData.ToString().Split('?');

                for (int i = 0; i < keyValPairs.Length; i++)
                {
                    String[] splits = keyValPairs[i].Split('=');
                    switch (splits[0])
                    {
                        case "code":
                            wpcode = splits[1].Split('&')[0].ToString();
                            break;
                    }
                }

                string nonce = oAuthUtil.GetNonce();
                string timeStamp = oAuthUtil.GetTimeStamp();

                string sigBaseStringParams = "client_id=" + _consumerKey;
                sigBaseStringParams += "&" + "redirect_uri=" + _callbackUrl;
                sigBaseStringParams += "&" + "grant_type=authorization_code";
                sigBaseStringParams += "&" + "oauth_signature_method=" + "HMAC-SHA1";
                sigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
                sigBaseStringParams += "&" + "code=" + wpcode;
                sigBaseStringParams += "&" + "oauth_version=1.0";

                string sigBaseString = "POST&";
                sigBaseString += Uri.EscapeDataString("https://public-api.wordpress.com/oauth2/token") + "&" + Uri.EscapeDataString(sigBaseStringParams);

                string signature = oAuthUtil.GetSignature(sigBaseString, _consumerSecret, null);

                var responseText = await oAuthUtil.PostData("https://public-api.wordpress.com/oauth2/token", sigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(signature));

                responseText = responseText.Replace(@"\", "");

                //return contents from json serialize object
                var WPAccessTokenItems = JsonConvert.DeserializeObject<WPAccessToken>(responseText);

                if (WPAccessTokenItems != null)
                {
                    #region Getting Auth Token

                    ApplicationData.Current.RoamingSettings.Values["isWordPressConfigure"] = "1";
                    ApplicationData.Current.RoamingSettings.Values["WordPressAccessToken"] = WPAccessTokenItems.access_token;
                    ApplicationData.Current.RoamingSettings.Values["WordPressTokenType"] = WPAccessTokenItems.token_type;
                    ApplicationData.Current.RoamingSettings.Values["WordPressBlogId"] = WPAccessTokenItems.blog_id;
                    ApplicationData.Current.RoamingSettings.Values["WordPressBlogURL"] = WPAccessTokenItems.blog_url;
                    ApplicationData.Current.RoamingSettings.Values["WordPressScope"] = WPAccessTokenItems.scope;

                    EnableConfigurationOptions();
                    #endregion
                }
            }
        }

        private void btnWordPress_Click(object sender, RoutedEventArgs e)
        {
            if (stackWordPress.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                stackWordPress.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imgWP.Source = new BitmapImage(new Uri("ms-appx:///Assets/ExpandBox.png", UriKind.Absolute));

            }
            else
            {
                stackWordPress.Visibility = Windows.UI.Xaml.Visibility.Visible;
                imgWP.Source = new BitmapImage(new Uri("ms-appx:///Assets/CollapseBox.png", UriKind.Absolute));
            }
        }
        #endregion
    }
}
