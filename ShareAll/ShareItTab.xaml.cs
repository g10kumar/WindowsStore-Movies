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
using ShareAll.Common;

using Syncfusion.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.Controls.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.ComponentModel;


using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
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

namespace ShareAll
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShareItTab : Page
    {
        ShareOperation shareOperation;
        private string sharedDataTitle;
        private string sharedDataDescription;
        private string shareQuickLinkId;
        private string sharedText;
        private Uri sharedUri;
        private const string dataFormatName = "http://schema.org/Book";
        string _facebookAppId = "521064607905686"; // You must set your own AppId here
        string _permissions = "user_about_me,read_stream,publish_stream"; // Set your permissions here
        FacebookClient _fb = new FacebookClient();
        string finalSharedText = "";

        private string title = "";
        private string subject = "";
        private string message = "";
        private string FBLinktoShare = "";
        string TweetMessage = "";
        string sText = "";
        string emailAddress = "";
        string tweetMessage = "";
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

        #endregion

        public ShareItTab()
        {
            this.InitializeComponent();
            //DataContext = new TabControlProperties();
            TwitterRt = new TwitterRt("OGAYZv3HDfykHsQw5dtng", "80cs06pnqehabIhv6PY1Vy1RuZX26G5twxJn6Oti0yU", "http://www.daksatech.com");
            htmlmailer.OnSSLServerAuthentication += htmlmailer_OnSSLServerAuthentication;
        }

        void htmlmailer_OnSSLServerAuthentication(object sender, HtmlmailersSSLServerAuthenticationEventArgs e)
        {
            e.Accept = true;

        }

        public TwitterRt TwitterRt { get; private set; }

        private void orientation_Loaded_1(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).ItemsSource = Enum.GetValues(typeof(TabStripPlacement));
            ((ComboBox)sender).SelectedIndex = 1;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShareSites s = (ShareSites)TabControl.SelectedItem;
            TabControl.SelectedIndex = 0;
            configureName.Text = s.Name.ToString();

            PostMessage.Text = "";

            EnableConfigurationOptions();

            #region Commented
            //TabControl tc = (TabControl)sender;
            //CheckBox chk = (CheckBox)tc.FindName("chkFacebook");
            //bool bll = (bool)chk.IsChecked;

            //TabItem p = TabControl.ItemContainerGenerator.ContainerFromItem(TabControl.SelectedItem) as TabItem;
            //if (p != null)
            //{
            //    StackPanel sp = FindVisualChild<StackPanel>(p) as StackPanel;
            //    //CheckBox chk = (CheckBox)sp.FindName("chkFacebook");
            //    TextBlock txt = (TextBlock)sp.FindName("Name");

            //    Windows.UI.Popups.MessageDialog dialog111 = new Windows.UI.Popups.MessageDialog("Name: " + txt.Text.ToString());
            //    dialog111.ShowAsync();
            //}

            //TextBlock txt = (TextBlock)TabControl.FindName("Name");
            //string ss = txt.Text.ToString();
            #endregion
        }

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
                        title = this.sharedDataTitle;
                    }

                    if (this.sharedText != null)
                    {
                        message = this.sharedText;
                        sText = this.sharedText;
                    }
                    else if (this.sharedDataDescription != null)
                    {
                        message = this.sharedDataDescription.ToString();
                        sText = this.sharedDataDescription.ToString();
                    }

                    string extractedLinkFromTwitterText = ExtractLinkFromText(this.sText);
                    string extractedLink = ExtractLinkFromText(this.sharedDataDescription.ToString());

                    if (this.sharedUri != null)
                    {
                        if (extractedLinkFromTwitterText == "")
                        {
                            TweetMessage = sText + " " + this.sharedUri.ToString();
                        }
                        else
                        {
                            if (this.sharedDataTitle != null & this.sharedDataTitle != "" & sText == extractedLinkFromTwitterText)
                            {
                                TweetMessage = this.sharedDataTitle + " " + sText;
                            }
                            else
                            {
                                TweetMessage = sText;
                            }
                        }
                    }
                    else
                    {
                        if (extractedLink != "")
                        {
                            TweetMessage = sText + " " + extractedLink;
                        }
                        else
                        {
                            TweetMessage = sText;
                        }

                    }

                    //EnableTwitterorNot(TweetMessage.ToString().Trim());


                    if (this.sharedUri != null)
                    {
                        FBLinktoShare = this.sharedUri.ToString();
                    }
                    else
                    {
                        //string extractedLink = ExtractLinkFromText(this.sharedDataDescription.ToString());
                        if (extractedLink != "")
                        {
                            FBLinktoShare = extractedLink;
                        }
                    }

                    LoadData();
                    #endregion

                });
            });            

            //LoadData();

            //EnableConfigurationOptions();

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
                btnConfigure.IsEnabled = false;
                chkConfigure.IsEnabled = false;
                chkConfigure.IsChecked = false;
            }
            else
            {
                txtCount.Text = diff.ToString();
                txtCount.Foreground = new SolidColorBrush(Colors.Black);
                btnConfigure.IsEnabled = true;
                if (ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"].ToString() == "1")
                {
                    btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    chkConfigure.IsEnabled = true;
                    if (chkConfigure.IsChecked == false)
                    {
                        chkConfigure.IsChecked = true;
                    }
                }
                else
                {
                    chkConfigure.IsEnabled = false;
                    if (chkConfigure.IsChecked == true)
                    {
                        chkConfigure.IsChecked = false;
                    }
                    btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
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

        private void EnableConfigurationOptions()
        {
            //TabItem p = TabControl.ItemContainerGenerator.ContainerFromItem(TabControl.SelectedItem) as TabItem;
            //if (p != null)
            //{
            //    StackPanel sp = FindVisualChild<StackPanel>(p) as StackPanel;
            //    //CheckBox chk = (CheckBox)sp.FindName("chkFacebook");
            //    TextBlock txt = (TextBlock)sp.FindName("Name");

            //    Windows.UI.Popups.MessageDialog dialog111 = new Windows.UI.Popups.MessageDialog("Name: " + txt.Text.ToString());
            //    dialog111.ShowAsync();
            //}

            ShareSites s = (ShareSites)TabControl.SelectedItem;

            if (s.Name == "Twitter")
            {
                stackTweet.Visibility = Windows.UI.Xaml.Visibility.Visible;
                EnableTwitterorNot(s.Message.ToString().Trim());

                if (ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"].ToString() == "1")
                {
                    if (s.Message.Length <= 140)
                    {
                        btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        chkConfigure.IsEnabled = true;
                        if (chkConfigure.IsChecked == false)
                        {
                            chkConfigure.IsChecked = true;
                        }
                    }
                    else
                    {
                        chkConfigure.IsEnabled = false;
                        if (chkConfigure.IsChecked == true)
                        {
                            chkConfigure.IsChecked = false;
                        }
                        btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                }
                else
                {
                    chkConfigure.IsEnabled = false;
                    if (chkConfigure.IsChecked == true)
                    {
                        chkConfigure.IsChecked = false;
                    }
                    btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
            else if (s.Name == "Facebook")
            {
                stackTweet.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                if (ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isFBConfigure"].ToString() == "1")
                //if (sessionData.isFBConfigure)
                {
                    btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    chkConfigure.IsEnabled = true;
                    if (chkConfigure.IsChecked == false)
                    {
                        chkConfigure.IsChecked = true;
                    }
                }
                else
                {
                    btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    chkConfigure.IsEnabled = false;
                    if (chkConfigure.IsChecked == true)
                    {
                        chkConfigure.IsChecked = false;
                    }
                }
            }
            else if (s.Name == "Email")
            {
                stackTweet.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                if (ApplicationData.Current.RoamingSettings.Values["isEmailConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isEmailConfigure"].ToString() == "1")
                {
                    btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    chkConfigure.IsEnabled = true;
                    if (chkConfigure.IsChecked == false)
                    {
                        chkConfigure.IsChecked = true;
                    }
                }
                else
                {
                    chkConfigure.IsEnabled = false;
                    if (chkConfigure.IsChecked == true)
                    {
                        chkConfigure.IsChecked = false;
                    }
                    btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
        }

        private void LoadData()
        {
            Shares = new ObservableCollection<ShareSites>();
            Shares.Add(new ShareSites("Facebook", title, FBLinktoShare, message));
            Shares.Add(new ShareSites("Twitter", "", "", TweetMessage));
            Shares.Add(new ShareSites("Email", "", title, message));

            //Shares = new ObservableCollection<ShareSites>();
            //Shares.Add(new ShareSites("Facebook", "FB title", "FB subject", "FB Message"));
            //Shares.Add(new ShareSites("Twitter", "Twitter title", "Twitter subject", "Metro style apps are full screen apps tailored to your users' needs, tailored to the device they run on, tailored for touch interaction, and tailored to the Windows user interface. Windows helps you interact with your users, and your users interact with your app."));
            //Shares.Add(new ShareSites("Email", "Email title", "Email subject", "Email Message"));

            TabControl.ItemsSource = Shares;
            TabControl.SelectedIndex = 1;
            
        }

        private ObservableCollection<ShareSites> shares;

        public ObservableCollection<ShareSites> Shares
        {
            get { return shares; }
            set { shares = value; }
        }

        private TabStripPlacement tabPlacement = TabStripPlacement.Top;

        public TabStripPlacement TabPlacement
        {
            get { return tabPlacement; }
            set { tabPlacement = value; RaisePropertyChanged("TabPlacement"); }
        }

        public void RaisePropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        private async void btnConfigure_Click(object sender, RoutedEventArgs e)
        {
            #region Commented
            //ShareSites s = (ShareSites)TabControl.SelectedItem;

            //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(s.Name.ToString());
            //await dialog.ShowAsync();
            #endregion

            ShareSites s = (ShareSites)TabControl.SelectedItem;

            if (s.Name == "Twitter")
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
            else if(s.Name == "Facebook")
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
            else if (s.Name == "Email")
            {
                myPopup.IsOpen = true;
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
        private async void btnShare_Click(object sender, RoutedEventArgs e)
        {
            #region Commented
            //ShareSites s = (ShareSites)TabControl.SelectedItem;            

            //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(s.Name.ToString() + " --- " + s.Title.ToString() + " --- " + s.Subject.ToString() + " --- " + s.Message.ToString());
            //await dialog.ShowAsync();
            #endregion

            ShareSites s = (ShareSites)TabControl.SelectedItem;

            PostMessage.Text = "";

            
            #region Facebook
            if (s.Name == "Facebook" && chkConfigure.IsChecked == true)
            {
                try
                {
                    string _lastMessageId;
                    dynamic parameters = new ExpandoObject();
                    parameters.message = s.Message.ToString();
                    parameters.name = s.Title.ToString();

                    string fbLink = ExtractLinkFromText(s.Subject.ToString());
                    
                    if (fbLink != "")
                    {
                        parameters.link = s.Subject.ToString();
                    }
                    parameters.access_token = ApplicationData.Current.RoamingSettings.Values["FBAccessToken"].ToString();

                    dynamic result = await _fb.PostTaskAsync("me/feed", parameters);

                    if (result.id != null)
                    {
                        _lastMessageId = result.id;
                        PostMessage.Text = "Posted to your Facebook wall.";
                        PostMessage.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        PostMessage.Text = "Post to Facebook failed. Please try again.";
                        PostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    }

                }
                catch (FacebookOAuthException foauthex)
                {
                    if (foauthex.ErrorCode == 506)
                    {
                        PostMessage.Text = "Post to Facebook failed. Message already exists on Facebook.";
                        PostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (foauthex.ErrorCode >= 100 & foauthex.ErrorCode < 200)
                    {
                        PostMessage.Text = "Post to Facebook failed. Please click on Settings icon and authorize the app before trying again.";
                        PostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] = 0;
                        btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else
                    {
                        if (foauthex.Message.IndexOf("Error validating access token") != 0 || foauthex.Message.IndexOf("Invalid OAuth access token") != 0 || foauthex.Message.IndexOf("An active access token must be used") != 0)
                        {
                            PostMessage.Text = "Post to Facebook failed. Please click on Settings icon and authorize the app before trying again.";
                            PostMessage.Foreground = new SolidColorBrush(Colors.Red);
                            ApplicationData.Current.RoamingSettings.Values["isFBConfigure"] = 0;
                            btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        }
                        else
                        {
                            PostMessage.Text = "Post to Facebook failed. " + foauthex.Message.ToString();
                            PostMessage.Foreground = new SolidColorBrush(Colors.Red);
                        }
                    }
                }
                catch (FacebookApiLimitException flimitex)
                {
                    PostMessage.Text = "Post to Facebook failed. " + flimitex.Message.ToString();
                    PostMessage.Foreground = new SolidColorBrush(Colors.Red);
                }
                catch (FacebookApiException fapiex)
                {
                    PostMessage.Text = "Post to Facebook failed. " + fapiex.Message.ToString();
                    PostMessage.Foreground = new SolidColorBrush(Colors.Red);
                }
                catch (Exception ex)
                {
                    PostMessage.Text = "Post to Facebook failed. " + ex.Message.ToString();
                    PostMessage.Foreground = new SolidColorBrush(Colors.Red);
                }

            }
            #endregion

            #region Twitter
            if (s.Name == "Twitter" && chkConfigure.IsChecked == true)
            {
                try
                {
                    tweeetSuccess = await TwitterRt.UpdateStatus(tweetMessage.ToString());

                    if (tweeetSuccess)
                    {
                        PostMessage.Text = "Posted to Twitter.";
                        PostMessage.Foreground = new SolidColorBrush(Colors.Green);
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

                        PostMessage.Text = twitterErrorMessage;
                        PostMessage.Foreground = new SolidColorBrush(Colors.Red);

                    }
                }

                catch (Exception ex)
                {
                    if (ex.Message.ToString() == "Access is denied. (Exception from HRESULT: 0x80070005 (E_ACCESSDENIED))" || TwitterRt.Status == "Access is denied. (Exception from HRESULT: 0x80070005 (E_ACCESSDENIED))")
                    {
                        ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] = 0;
                        EnableTwitterOptions();

                        PostMessage.Text = "Could not post to Twitter. Please click on Settings icon and authorize the app.";
                        PostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        PostMessage.Text = "Could not post to Twitter. " + ex.Message.ToString();
                        PostMessage.Foreground = new SolidColorBrush(Colors.Red);
                    }
                }

            }
            #endregion

            #region Email
            
            if (s.Name == "Email" && chkConfigure.IsChecked == true)
            {

                string mailType = "";
                string mailServer = "";
                string mailFromAddress = "";
                string mailFromAddressPwd = "";
                string mailToEmails = "";
                string[] mailToEmailsArray = null;
                mailType = ApplicationData.Current.RoamingSettings.Values["EmailServerType"].ToString();
                mailServer = ApplicationData.Current.RoamingSettings.Values["EmailServerDomain"].ToString();
                mailFromAddress = AES_Decrypt(ApplicationData.Current.RoamingSettings.Values["EmailConfigureEmail"].ToString(), "daksatech");
                mailFromAddressPwd = AES_Decrypt(ApplicationData.Current.RoamingSettings.Values["EmailConfigurePwd"].ToString(), "daksatech");

                #region Encrypting Sending Email Addreses
                // emailAddress
                if (emailAddress.Contains(","))
                {
                    mailToEmailsArray = emailAddress.Split(',');
                }
                else if (emailAddress.Contains(" "))
                {
                    mailToEmailsArray = emailAddress.Split(' ');
                }
                else if (emailAddress.Contains(";"))
                {
                    mailToEmailsArray = emailAddress.Split(';');
                }
                else
                {
                    mailToEmails = AES_Encrypt(emailAddress, "daksatech");
                }

                mailToEmails = createCSVEmailList(mailToEmailsArray, ",");


                #endregion

                #region Sending Mail
                string returnString = "";
                try
                {
                    // sending test mail with serverType, ServerDomain,  toaddress and password, subject, message
                    //(mailType,mailServer,mailFromAddress,mailToEmails,mailFromAddressPwd, EmailSubject.Text, EmailMessage.Text)

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
                    htmlmailer.Subject = s.Subject;
                    htmlmailer.MessageHTML = s.Message;
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
                        PostMessage.Text = "Mail has sending successfully.";
                        
                    }
                    else
                    {
                        PostMessage.Text = "Unable to send Email now.";
                    }
                }
                catch (Exception ex)
                {
                    PostMessage.Text = ex.Message.ToString();
                }

                #endregion
            }
            #endregion
            
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
            ShareSites s = (ShareSites)TabControl.SelectedItem;

            if (s.Name == "Twitter")
            {
                if (ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"] != null && ApplicationData.Current.RoamingSettings.Values["isTweetConfigure"].ToString() == "1")
                {
                    btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    chkConfigure.IsEnabled = true;
                    if (chkConfigure.IsChecked == false)
                    {
                        chkConfigure.IsChecked = true;
                    }
                }
                else
                {
                    chkConfigure.IsEnabled = false;
                    if (chkConfigure.IsChecked == true)
                    {
                        chkConfigure.IsChecked = false;
                    }
                    btnConfigure.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
        }

        private void Message_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            ShareSites s = (ShareSites)TabControl.SelectedItem;
            if (s.Name == "Twitter")
            {
                EnableTwitterorNot(txt.Text.Trim());
                tweetMessage = txt.Text.Trim();
            }
        }

        private void Title_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            ShareSites s = (ShareSites)TabControl.SelectedItem;
            if (s.Name == "Email")
            {
                emailAddress = txt.Text.Trim();
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
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

                    htmlmailer.MailServer = serverDomain;

                    if (serverType != "Yahoo")
                    {
                        htmlmailer.SSLStartMode = HtmlmailersSSLStartModes.sslExplicit;
                        htmlmailer.MailPort = 587;
                    }
                    else
                    {
                        htmlmailer.MailPort = 465;
                    }
                    htmlmailer.User = txtEmail.Text;
                    htmlmailer.Password = txtPassword.Password;
                    htmlmailer.From = txtEmail.Text;
                    htmlmailer.SendTo = txtEmail.Text;
                    htmlmailer.Subject = "Test Email";
                    htmlmailer.MessageHTML = "This is a test email from Share All.";
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
                        #region Encrypting Configuration Email and Password

                        entryptConfigEmail = AES_Encrypt(txtEmail.Text, "daksatech");
                        entryptConfigPwd = AES_Encrypt(txtPassword.Password.ToString(), "daksatech");

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
        

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}
