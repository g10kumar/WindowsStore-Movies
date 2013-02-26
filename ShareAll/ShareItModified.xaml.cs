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
using Windows.UI.Xaml.Media.Imaging;
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

namespace ShareAll
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShareItModified : Page
    {
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
        Utilities u = new Utilities();
        #endregion

        public ShareItModified()
        {
            this.InitializeComponent();
            TwitterRt = new TwitterRt("OGAYZv3HDfykHsQw5dtng", "80cs06pnqehabIhv6PY1Vy1RuZX26G5twxJn6Oti0yU", "http://www.daksatech.com");
            htmlmailer.OnSSLServerAuthentication += htmlmailer_OnSSLServerAuthentication;
        }

        void htmlmailer_OnSSLServerAuthentication(object sender, HtmlmailersSSLServerAuthenticationEventArgs e)
        {
            e.Accept = true;

        }

        public TwitterRt TwitterRt { get; private set; }
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
                        EmailSubject.Text = this.sharedDataTitle;
                    }

                    if (this.sharedText != null)
                    {
                        FBMessage.Text = this.sharedText;
                        sText = this.sharedText;
                        //TweetMessage.Text = this.sharedText;
                        EmailMessage.Text = this.sharedText;
                    }
                    else if (this.sharedDataDescription != null)
                    {
                        FBMessage.Text = this.sharedDataDescription.ToString();
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

                    string extractedLinkFromTwitterText = u.ExtractLinkFromText(this.sText);
                    string extractedLink = u.ExtractLinkFromText(this.sharedDataDescription.ToString());

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

                    //EnableTwitterorNot(TweetMessage.Text.Trim());


                    if (this.sharedUri != null)
                    {
                        FBLinktoShare.Text = this.sharedUri.ToString();
                    }
                    else
                    {
                        //string extractedLink = u.ExtractLinkFromText(this.sharedDataDescription.ToString());
                        if (extractedLink != "")
                        {
                            FBLinktoShare.Text = extractedLink;
                        }
                    }
                    #endregion



                });
            });
            EnableConfigurationOptions();
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

        private void EnableConfigurationOptions()
        {
            if (ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isFBConfigure"].ToString() == "1")
            {
                btnFBConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnFacebook.Margin = new Thickness(395, -30, 0, 0);
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
                    btnTweet.Margin = new Thickness(395, -30, 0, 0);
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
                btnEmail.Margin = new Thickness(395, -30, 0, 0);
                chkEmail.IsEnabled = true;
                if (chkEmail.IsChecked == false)
                {
                    chkEmail.IsChecked = true;
                }
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
            if (FBLinktoShare.Text == "Link")
            {
                FBLinktoShare.Text = "";
            }

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
            EnableConfigurationOptions();

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

        private async void btnTweetConfigure_Click(object sender, RoutedEventArgs e)
        {
            await TwitterRt.GainAccessToTwitter();
            if (TwitterRt.Status == "Access granted")
            {
                ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] = "1";
                ApplicationData.Current.RoamingSettings.Values["TweetOauthToken"] = TwitterRt.OauthToken;
                ApplicationData.Current.RoamingSettings.Values["TweetOauthTokenSecret"] = TwitterRt.OauthTokenSecret;
                ApplicationData.Current.RoamingSettings.Values["TweetUserID"] = TwitterRt.UserID;
                EnableConfigurationOptions();
            }
            else
            {
                ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] = "0";
                EnableConfigurationOptions();
            }
        }

        private void TweetMessage_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            EnableTwitterorNot(TweetMessage.Text.Trim());
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

        private void EmailTo_GotFocus_1(object sender, RoutedEventArgs e)
        {
            if (EmailTo.Text == "To Emails (separate with comma, space or semi colon)")
            {
                EmailTo.Text = "";
            }

        }

        private void EmailTo_LostFocus_1(object sender, RoutedEventArgs e)
        {
            if (EmailTo.Text == "")
            {
                EmailTo.Text = "To Emails (separate with comma, space or semi colon)";
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

        private void btnEmailConfigure_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
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
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = false;
        }

        //public string AES_Encrypt(string input, string pass)
        //{
        //    SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
        //    CryptographicKey AES;
        //    HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
        //    CryptographicHash Hash_AES = HAP.CreateHash();

        //    string encrypted = "";
        //    try
        //    {
        //        byte[] hash = new byte[32];
        //        Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(pass)));
        //        byte[] temp;
        //        CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out temp);

        //        Array.Copy(temp, 0, hash, 0, 16);
        //        Array.Copy(temp, 0, hash, 15, 16);

        //        AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

        //        IBuffer Buffer = CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(input));
        //        encrypted = CryptographicBuffer.EncodeToBase64String(CryptographicEngine.Encrypt(AES, Buffer, null));

        //        return encrypted;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public string AES_Decrypt(string input, string pass)
        //{
        //    SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
        //    CryptographicKey AES;
        //    HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
        //    CryptographicHash Hash_AES = HAP.CreateHash();

        //    string decrypted = "";
        //    try
        //    {
        //        byte[] hash = new byte[32];
        //        Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(pass)));
        //        byte[] temp;
        //        CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out temp);

        //        Array.Copy(temp, 0, hash, 0, 16);
        //        Array.Copy(temp, 0, hash, 15, 16);

        //        AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

        //        IBuffer Buffer = CryptographicBuffer.DecodeFromBase64String(input);
        //        byte[] Decrypted;
        //        CryptographicBuffer.CopyToByteArray(CryptographicEngine.Decrypt(AES, Buffer, null), out Decrypted);
        //        decrypted = System.Text.Encoding.UTF8.GetString(Decrypted, 0, Decrypted.Length);

        //        return decrypted;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        private async void btnShare_Click(object sender, RoutedEventArgs e)
        {
            fbPostMessage.Text = "";
            TwitterPostMessage.Text = "";
            if (FBLinktoShare.Text == "Link")
            {
                FBLinktoShare.Text = "";
            }

            #region Facebook
            if (chkFacebook.IsChecked == true)
            {
                try
                {
                    string _lastMessageId;
                    dynamic parameters = new ExpandoObject();
                    parameters.message = FBMessage.Text;
                    parameters.name = FBTitle.Text;

                    string fbLink = u.ExtractLinkFromText(FBLinktoShare.Text);
                    if (fbLink != "")
                    {
                        parameters.link = FBLinktoShare.Text.ToString();
                    }
                    parameters.access_token = ApplicationData.Current.RoamingSettings.Values["FBAccessToken"].ToString();

                    dynamic result = await _fb.PostTaskAsync("me/feed", parameters);

                    if (result.id != null)
                    {
                        _lastMessageId = result.id;
                        fbPostMessage.Text = "Posted to your Facebook wall.";
                        fbPostMessage.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        fbPostMessage.Text = "Post to Facebook failed. Please try again.";
                        fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    }

                }
                catch (FacebookOAuthException foauthex)
                {
                    if (foauthex.ErrorCode == 506)
                    {
                        fbPostMessage.Text = "Post to Facebook failed. Message already exists on Facebook.";
                        fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (foauthex.ErrorCode >= 100 & foauthex.ErrorCode < 200)
                    {
                        fbPostMessage.Text = "Post to Facebook failed. Please click on Settings icon and authorize the app before trying again.";
                        fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] = 0;
                        btnFBConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else
                    {
                        if (foauthex.Message.IndexOf("Error validating access token") != 0 || foauthex.Message.IndexOf("Invalid OAuth access token") != 0 || foauthex.Message.IndexOf("An active access token must be used") != 0)
                        {
                            fbPostMessage.Text = "Post to Facebook failed. Please click on Settings icon and authorize the app before trying again.";
                            fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                            ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] = 0;
                            btnFBConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        }
                        else
                        {
                            fbPostMessage.Text = "Post to Facebook failed. " + foauthex.Message.ToString();
                            fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        }
                    }
                }
                catch (FacebookApiLimitException flimitex)
                {
                    fbPostMessage.Text = "Post to Facebook failed. " + flimitex.Message.ToString();
                    fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                }
                catch (FacebookApiException fapiex)
                {
                    fbPostMessage.Text = "Post to Facebook failed. " + fapiex.Message.ToString();
                    fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                }
                catch (Exception ex)
                {
                    fbPostMessage.Text = "Post to Facebook failed. " + ex.Message.ToString();
                    fbPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                }

            }
            #endregion
            
            #region Twitter
            if (chkTweet.IsChecked == true)
            {
                try
                {
                    tweeetSuccess = await TwitterRt.UpdateStatus(TweetMessage.Text);

                    if (tweeetSuccess)
                    {
                        TwitterPostMessage.Text = "Posted to Twitter.";
                        TwitterPostMessage.Foreground = new SolidColorBrush(Colors.Green);
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
                                    twitterErrorMessage = "Could not post to Twitter because of authentication issue. Please click Settings icon and authorize app.";
                                    break;
                                case "403":
                                    twitterErrorMessage = "The post was rejected by Twitter.";
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
                                    twitterErrorMessage = "Could not post image to Twitter. Try again later.";
                                    break;
                                case "429":
                                    twitterErrorMessage = "Could not post to Twitter. Try again later.";
                                    break;
                                case "500":
                                case "502":
                                case "503":
                                case "504":
                                    twitterErrorMessage = "Could not post to Twitter. It seems to be overloaded. Try again later.";
                                    break;
                                default:
                                    twitterErrorMessage = "";
                                    break;

                            }
                        }

                        TwitterPostMessage.Text = twitterErrorMessage;
                        TwitterPostMessage.Foreground = new SolidColorBrush(Colors.Red);

                    }
                }

                catch (Exception ex)
                {
                    if (ex.Message.ToString() == "Access is denied. (Exception from HRESULT: 0x80070005 (E_ACCESSDENIED))" || TwitterRt.Status == "Access is denied. (Exception from HRESULT: 0x80070005 (E_ACCESSDENIED))")
                    {
                        ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] = 0;
                        EnableTwitterOptions();

                        TwitterPostMessage.Text = "Could not post to Twitter. Please click on Settings icon and authorize the app.";
                        TwitterPostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        TwitterPostMessage.Text = "Could not post to Twitter. " + ex.Message.ToString();
                        TwitterPostMessage.Foreground = new SolidColorBrush(Colors.Red);
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

                string mailType = "";
                string mailServer = "";
                string mailFromAddress = "";
                string mailFromAddressPwd = "";
                string mailToEmails = "";
                string[] mailToEmailsArray = null;
                mailType = ApplicationData.Current.RoamingSettings.Values["EmailServerType"].ToString();
                mailServer = ApplicationData.Current.RoamingSettings.Values["EmailServerDomain"].ToString();
                mailFromAddress = u.AES_Decrypt(ApplicationData.Current.RoamingSettings.Values["EmailConfigureEmail"].ToString(), "daksatech");
                mailFromAddressPwd = u.AES_Decrypt(ApplicationData.Current.RoamingSettings.Values["EmailConfigurePwd"].ToString(), "daksatech");

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
                else
                {
                    mailToEmails = u.AES_Encrypt(EmailTo.Text, "daksatech");
                }

                mailToEmails = u.createCSVEmailList(mailToEmailsArray, ",");


                #endregion

                #region Sending Mail
                string returnString = "";
                try
                {
                    // sending test mail with serverType, ServerDomain,  toaddress and password, subject, message
                    //(mailType,mailServer,mailFromAddress,mailToEmails,mailFromAddressPwd, EmailSubject.Text, EmailMessage.Text)
                    returnString = u.SendEmail(htmlmailer, mailServer, mailType, mailFromAddress, mailFromAddressPwd, mailFromAddress, mailToEmails, EmailSubject.Text, EmailMessage.Text);

                    //htmlmailer.MailServer = mailServer;

                    //if (mailType != "Yahoo")
                    //{
                    //    htmlmailer.SSLStartMode = HtmlmailersSSLStartModes.sslExplicit;
                    //    htmlmailer.MailPort = 587;
                    //}
                    //else
                    //{
                    //    htmlmailer.MailPort = 465;
                    //}
                    //htmlmailer.User = mailFromAddress;
                    //htmlmailer.Password = mailFromAddressPwd;
                    //htmlmailer.From = mailFromAddress;
                    //htmlmailer.SendTo = mailToEmails;
                    //htmlmailer.Subject = EmailSubject.Text;
                    //htmlmailer.MessageHTML = EmailMessage.Text;
                    //htmlmailer.Config("Hello=Metro"); //This can not be automatically determined in WinRT due to access restrictions.

                    //string message = "";
                    //try
                    //{
                    //    await htmlmailer.SendAsync();
                    //    message = "Success";
                    //}
                    //catch (Exception ex)
                    //{
                    //    message = "Error: " + ex.Message;
                    //}

                    //returnString = message;
                    if (returnString == "Success")
                    {
                        // wrap the below around if else statements; only enable if email test was successful
                        EmailPostMessage.Text = "Mail has sending successfully.";
                        EmailTo.Text = "To Emails (separate with comma, space or semi colon)";
                    }
                    else
                    {
                        EmailPostMessage.Text = "Unable to send Email now.";
                    }
                }
                catch (Exception ex)
                {
                    EmailPostMessage.Text = ex.Message.ToString();
                }

                #endregion
            }
            #endregion
        }

    }
}
