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
using Windows.ApplicationModel.Store;
using Windows.UI.ViewManagement;
using Windows.Storage;
using Windows.ApplicationModel;
using System.Threading.Tasks;
using QuotesOfWisdom.Common;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Windows.UI.Core;

using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using System.Xml;
using System.Xml.Linq;
using System.Security;
using System.Net;
using System.ComponentModel;
using Windows.UI.Popups;
using Windows.UI.ApplicationSettings;
using Windows.System.Threading;

namespace QuotesOfWisdom
{
    public sealed partial class BackgroundMoreImages : UserControl
    {
        #region Objects
        bool isBackgroundButtonVisible = true;
        LicenseChangedEventHandler licenseChangeHandler = null;
        string genericURL = "";
        #endregion

        /// <summary>
        /// Constructor of the page
        /// </summary>
        public BackgroundMoreImages()
        {
            this.InitializeComponent();
            LoadFormData();
            stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //txtSearch.Text = "bike";
            if (txtSearch.Text != "")
            {
                //genericURL = "https://api.500px.com/v1/photos/search?term=" + txtSearch.Text.Trim() + "&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4&rpp=40&exclude_nude=true&page=" + sessionData.currentPage;
                genericURL = "https://api.500px.com/v1/photos/search?term=" + txtSearch.Text.Trim() + "&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&rpp=36&sort=votes_count&exclude=Nude&page=" + sessionData.currentPage;
            }
            else
            {
                //genericURL = "https://api.500px.com/v1/photos?consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4&exclude=Nude&rpp=40&page=" + sessionData.currentPage;
                genericURL = "https://api.500px.com/v1/photos?consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&exclude=Nude&sort=votes_count&rpp=36&page=" + sessionData.currentPage;
            }
            //Task.Delay(TimeSpan.FromSeconds(30));
            //try
            //{
                LoadBackgroundImages(genericURL);
            //}
            //catch (Exception ex)
            //{
            //    stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //    imageStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //    txtMessage.Text = "Unable to load images now. Please try later! \r\n" + ex.Message.ToString();
            //}
            
        }        

        /// <summary>
        /// Method for loading page initial data
        /// </summary>
        private async void LoadFormData()
        {
            await LoadInAppPurchaseProxyFileAsync();

            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            //LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            string visualState = ApplicationView.Value.ToString();

            var imageLicense = licenseInformation.ProductLicenses["More Background Images"];
            if (imageLicense.IsActive)
            {
                isBackgroundButtonVisible = true;
            }
            else
            {
                if (visualState == "Snapped")
                {

                }
                else
                {

                }
                isBackgroundButtonVisible = false;
            }
        }

        /// <summary>
        /// Method for loading App Purchase proxy file
        /// </summary>
        /// <returns></returns>
        private async Task LoadInAppPurchaseProxyFileAsync()
        {
            StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("Xml");
            StorageFile proxyFile = await proxyDataFolder.GetFileAsync("in-app-purchase.xml");
            licenseChangeHandler = new LicenseChangedEventHandler(InAppPurchaseRefreshScenario);

            CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
            //CurrentApp.LicenseInformation.LicenseChanged += licenseChangeHandler;
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);
        }

        /// <summary>
        /// Refresg method ofr App Purchase
        /// </summary>
        private void InAppPurchaseRefreshScenario()
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            //LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var imageLicense = licenseInformation.ProductLicenses["More Background Images"];
            if (imageLicense.IsActive)
            {
                isBackgroundButtonVisible = true;
            }
            else
            {
                string visualState = ApplicationView.Value.ToString();
                //if (visualState == "Snapped")
                //{

                //}
                //else
                //{

                //}
                isBackgroundButtonVisible = false;
            }
        }

        /// <summary>
        /// LayoutUpdted Event of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_LayoutUpdated(object sender, object e)
        {
            // Calls the Background change method
            ChangeBackground();
        }        

        /// <summary>
        /// Method for changing background
        /// </summary>
        private void ChangeBackground()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            {
                if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() != "dynamicStyle")
                {
                    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                }
                else
                {
                    Utilities.dynamicBackgroundChange(LayoutRoot);
                }
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;

            }
        }

        /// <summary>
        /// Onloaded method for Framework to set the content of Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (isBackgroundButtonVisible)
            {
                button.Content = "Set as Background";
            }
            else
            {
                button.Content = "Buy Images";
            }
        }

        /// <summary>
        /// Method for loading Background Images
        /// </summary>
        /// <param name="URL"></param>
        private async void LoadBackgroundImages(string URL)
        {
            try
            {
                // creating bgImages list object
                List<BGImages> bglist = new List<BGImages>();
                List<BGImages> tmpbglist = new List<BGImages>();

                List<string> listDesp = new List<string>();
                List<string> listName = new List<string>();

                var handler = new HttpClientHandler { AllowAutoRedirect = false };
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36");

                var randString = System.Guid.NewGuid();

                var response = await client.GetAsync(URL + "&" + randString);
                var jsonbgImageresult = await response.Content.ReadAsStringAsync();

                jsonbgImageresult = jsonbgImageresult.Replace(@"\", "");

                string desppattern = @"(?<=""description"":"").*?(?="",""times_viewed"")";

                string despparseresult = Regex.Replace(jsonbgImageresult, desppattern, match => match.Value.Replace("\"", ""));

                string namepattern = @"(?<=""name"":"").*?(?="",""description"")";

                string nameparseresult = Regex.Replace(despparseresult, namepattern, match => match.Value.Replace("\"", ""));

                #region Commented
                /*
                string usernamepattern = @"(?<=""id"":"").*?(?="",""firstname"")";

                string usernameparseresult = Regex.Replace(nameparseresult, usernamepattern, match => match.Value.Replace("\"", ""));

                string firstnamepattern = @"(?<=""username"":"").*?(?="",""lastname"")";

                string firstnameparseresult = Regex.Replace(usernameparseresult, firstnamepattern, match => match.Value.Replace("\"", ""));

                string lastnamepattern = @"(?<=""firstname"":"").*?(?="",""city"")";

                string lastnameparseresult = Regex.Replace(firstnameparseresult, lastnamepattern, match => match.Value.Replace("\"", ""));

                string citypattern = @"(?<=""lastname"":"").*?(?="",""country"")";

                string cityparseresult = Regex.Replace(lastnameparseresult, citypattern, match => match.Value.Replace("\"", ""));

                string countrypattern = @"(?<=""city"":"").*?(?="",""fullname"")";

                string countryparseresult = Regex.Replace(cityparseresult, countrypattern, match => match.Value.Replace("\"", ""));

                string fullnamepattern = @"(?<=""country"":"").*?(?="",""userpic_url"")";

                string fullnameparseresult = Regex.Replace(countryparseresult, fullnamepattern, match => match.Value.Replace("\"", ""));
                */
                #endregion

                var bgItems = JsonConvert.DeserializeObject<BGImageListings>(nameparseresult);

                sessionData.totalImagesCount = Convert.ToInt32(bgItems.total_items.ToString());

                for (int i = 0; i < bgItems.photos.Count(); i++)
                {
                    BGImages s = new BGImages();

                    s.ImageURLsmall = bgItems.photos[i].images[0].url.ToString().Replace("2.jpg", "3.jpg");
                    s.ImageURLbig = bgItems.photos[i].images[0].url.ToString().Replace("2.jpg", "4.jpg");
                    if (bgItems.photos[i].name.ToString() != "Untitled")
                    {
                        s.Title = bgItems.photos[i].name.ToString();
                    }
                    else
                    {
                        s.Title = "";
                    }

                    string userName = "";

                    if (bgItems.photos[i].user.firstname != null)
                    {
                        userName += bgItems.photos[i].user.firstname.ToString();
                    }

                    if (bgItems.photos[i].user.lastname != null)
                    {
                        userName += " " + bgItems.photos[i].user.lastname.ToString();
                    }

                    s.UserName = userName;

                    //if (bgItems.photos[i].user.fullname != null)
                    //{
                    //    s.UserName = bgItems.photos[i].user.fullname.ToString();
                    //}
                    //else
                    //{
                    //    

                    //    if (bgItems.photos[i].user.firstname != null)
                    //    {
                    //        userName += bgItems.photos[i].user.firstname.ToString(); 
                    //    }

                    //    if (bgItems.photos[i].user.lastname != null)
                    //    {
                    //        userName += " " + bgItems.photos[i].user.lastname.ToString();
                    //    }
                    //}
                    //if (bgItems.photos[i].images[0].size == "3")
                    //{
                    //    s.ImageURLsmall = bgItems.photos[i].images[0].url.ToString();
                    //}

                    //if (bgItems.photos[i].images[1].size == "4")
                    //{
                    //    s.ImageURLbig = bgItems.photos[i].images[1].url.ToString();
                    //}

                    bglist.Add(s);
                    s = null;
                }

                if (sessionData.currentBackgroundImages != null && sessionData.currentBackgroundImages.Count() > 0)
                {
                    tmpbglist = sessionData.currentBackgroundImages;
                    sessionData.currentBackgroundImages = null;
                }

                foreach (QuotesOfWisdom.BGImages b in bglist)
                {
                    tmpbglist.Add(b);
                }

                // assinging images from the temp images list into SessionData variable
                sessionData.currentBackgroundImages = tmpbglist;


                if (tmpbglist.Count != 0)
                {
                    imageStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    gvImages.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    if (sessionData.currentBackgroundImages.Count() >= sessionData.totalImagesCount)
                    {
                        btnMore.Visibility = Windows.UI.Xaml.Visibility.Collapsed; 
                    }
                    else
                    {
                        btnMore.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }

                    // binds the bg images list
                    gvImages.ItemsSource = sessionData.currentBackgroundImages.ToList();

                    int pos = Convert.ToInt32(gvImages.Items.Count()) - 36;
                    gvImages.ScrollIntoView(gvImages.Items[pos]);

                    stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                }
                else
                {
                    stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    imageStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    txtMessage.Text = "No images found. Please try a different keyword!";
                }

            }
            catch (Exception ex)
            {
                stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imageStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                stackMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                txtMessage.Text = "Unable to load images now. Please try later!";// \r\n" + ex.Message.ToString();
            }

        }

        /// <summary>
        /// Click event of the btnMore button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMore_Click(object sender, RoutedEventArgs e)
        {
            stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
            
            sessionData.currentPage += 1;

            if (txtSearch.Text != "")
            {
                genericURL = "https://api.500px.com/v1/photos/search?term=" + txtSearch.Text.Trim() + "&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&rpp=36&sort=votes_count&exclude=Nude&page=" + sessionData.currentPage;
            }
            else
            {
                genericURL = "https://api.500px.com/v1/photos?consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&rpp=36&sort=votes_count&exclude=Nude&page=" + sessionData.currentPage;
            }

            LoadBackgroundImages(genericURL);
        }

        /// <summary>
        /// Click event of the Preview button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = true;
            Button preview = (Button)sender;
            hplbtnBackground.Tag = preview.Tag.ToString();
            Uri uri = new Uri(preview.Tag.ToString(), UriKind.Absolute);
            ImageSource newSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(uri);
            imgPreview.Source = newSource;
        }

        /// <summary>
        /// Click event of Background button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackground_Click(object sender, RoutedEventArgs e)
        {
            Button background = (Button)sender;

            #region Commented on 04.07.2013
            /*
            
            if (background.Content.ToString() != "Buy Images")
            {
                ApplicationData.Current.RoamingSettings.Values["Settings"] = "dynamicStyle";
                ApplicationData.Current.RoamingSettings.Values["ImageURLForDynamicStyle"] = background.Tag.ToString();
                Utilities.dynamicBackgroundChange(LayoutRoot);
            }
            else
            {
                LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
                var imageLicense = licenseInformation.ProductLicenses["More Background Images"];
                if (!imageLicense.IsActive)
                {
                    try
                    {
                        await CurrentApp.RequestProductPurchaseAsync("More Background Images", false);
                        if (imageLicense.IsActive)
                        {
                            Utilities.ShowMessage("You bought the More Background Images version.");
                        }
                    }
                    catch (Exception)
                    {
                        Utilities.ShowMessage("Unable to buy More Background Images version.");
                    }
                }
                else
                {
                    Utilities.ShowMessage("You already own More Background Images version of app.");
                }
            }
             * */
            #endregion

            #region Commented
            /*
            // Create the message dialog and set its content
            var messageDialog = new MessageDialog("Custom background images is a paid feature. Purchase this feature now?");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            messageDialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(this.CommandInvokedHandler)));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
             **/
            #endregion

            #region Commented on 15.07.2013
            /*
            if (isBackgroundButtonVisible)
            {
                ApplicationData.Current.RoamingSettings.Values["Settings"] = "dynamicStyle";
                ApplicationData.Current.RoamingSettings.Values["ImageURLForDynamicStyle"] = background.Tag.ToString();
                Utilities.dynamicBackgroundChange(LayoutRoot);
            }
            else
            {
                setBackgroundPopup.IsOpen = true;
            }*/
            #endregion
            SetAsBackground(background.Tag.ToString());
        }

        private void SetAsBackground(string ImageURL)
        {
            if (isBackgroundButtonVisible)
            {
                ApplicationData.Current.RoamingSettings.Values["Settings"] = "dynamicStyle";
                ApplicationData.Current.RoamingSettings.Values["ImageURLForDynamicStyle"] = ImageURL.ToString();
                Utilities.dynamicBackgroundChange(LayoutRoot);
            }
            else
            {
                setBackgroundPopup.IsOpen = true;
            }
        }

        private async void btnYes_Click(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            //LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var imageLicense = licenseInformation.ProductLicenses["More Background Images"];
            if (!imageLicense.IsActive)
            {
                try
                {
                    await CurrentAppSimulator.RequestProductPurchaseAsync("More Background Images", false);
                    //await CurrentApp.RequestProductPurchaseAsync("More Background Images", false);
                    if (imageLicense.IsActive)
                    {
                        Utilities.ShowMessage("You bought the More Background Images version.");
                    }
                }
                catch (Exception)
                {
                    Utilities.ShowMessage("Unable to buy More Background Images version.");
                }
            }
            else
            {
                Utilities.ShowMessage("You already own More Background Images version of app.");
            }
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            setBackgroundPopup.IsOpen = false;
        }

        /// <summary>
        /// Click event of the Search button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            btnMore.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            sessionData.currentBackgroundImages = null;
            gvImages.ItemsSource = null;
            stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //genericURL = "https://api.500px.com/v1/photos?search?term=" + txtSearch.Text.Trim() + "&sort=votes_count&exclude_nude=true&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4&rpp=40&page=" + sessionData.currentPage;
            genericURL = "https://api.500px.com/v1/photos/search?term=" + txtSearch.Text.Trim() + "&page=" + sessionData.currentPage + "&rpp=36&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&sort=votes_count&exclude=Nude";
            LoadBackgroundImages(genericURL);
        }

         /// <summary>
        /// Click event of the back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackButtonClicked(object sender, RoutedEventArgs e)
        {
            genericURL = "";
            isBackgroundButtonVisible = false;
            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            sessionData.currentBackgroundImages = null;
            sessionData.totalImagesCount = 0;
            if (licenseChangeHandler != null)
            {
                CurrentAppSimulator.LicenseInformation.LicenseChanged -= licenseChangeHandler;
                //CurrentApp.LicenseInformation.LicenseChanged -= licenseChangeHandler;
            }

            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new SettingsUserControl());
            }

            //if (this.Parent.GetType() == typeof(Popup))
            //{
            //    ((Popup)this.Parent).IsOpen = false;
            //}
            //SettingsPane.Show();
        }

        /// <summary>
        /// Click event of the pop up Close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = false;
        }

        /// <summary>
        /// Click event of the pop up Set Background button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hplbtnBackground_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton background = (HyperlinkButton)sender;
            SetAsBackground(background.Tag.ToString());
        }

        /// <summary>
        /// Unloaded event of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            genericURL = "";
            isBackgroundButtonVisible = false;
            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            sessionData.currentBackgroundImages = null;
            sessionData.totalImagesCount = 0;
            if (licenseChangeHandler != null)
            {
                CurrentAppSimulator.LicenseInformation.LicenseChanged -= licenseChangeHandler;
                //CurrentApp.LicenseInformation.LicenseChanged -= licenseChangeHandler;
            }
        }
      
    }

    public class BGImageListings
    {
        public string total_items { get; set; }
        public photos[] photos = { };
    }

    public class photos
    {
        public string name { get; set; }
        public images[] images = { };
        public user user = new user();
    }

    public class user
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string fullname { get; set; }
    }

    public class images
    {
        public string size { get; set; }
        public string url { get; set; }
    }

    public class BGImages
    {
        public string ImageURLsmall { get; set; }
        public string ImageURLbig { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
    }
}
