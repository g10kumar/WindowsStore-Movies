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
                genericURL = "https://api.500px.com/v1/photos/search?term=" + txtSearch.Text.Trim() + "&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&rpp=36&sort=votes_count&exclude_nude=true&page=" + sessionData.currentPage;
            }
            else
            {
                //genericURL = "https://api.500px.com/v1/photos?consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4&exclude_nude=true&rpp=40&page=" + sessionData.currentPage;
                genericURL = "https://api.500px.com/v1/photos?consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&exclude_nude=true&sort=votes_count&rpp=36&page=" + sessionData.currentPage;
            }
            LoadBackgroundImages(genericURL);
        }        

        /// <summary>
        /// Method for loading page initial data
        /// </summary>
        private async void LoadFormData()
        {
            await LoadInAppPurchaseProxyFileAsync();

            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
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

            CurrentApp.LicenseInformation.LicenseChanged += licenseChangeHandler;
        }

        /// <summary>
        /// Refresg method ofr App Purchase
        /// </summary>
        private void InAppPurchaseRefreshScenario()
        {
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
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

                string despoutresult = Regex.Replace(jsonbgImageresult, desppattern, match => match.Value.Replace("\"", ""));

                string namepattern = @"(?<=""name"":"").*?(?="",""description"")";

                string nameoutresult = Regex.Replace(despoutresult, namepattern, match => match.Value.Replace("\"", ""));

                var bgItems = JsonConvert.DeserializeObject<BGImageListings>(nameoutresult);

                sessionData.totalImagesCount = Convert.ToInt32(bgItems.total_items.ToString());

                for (int i = 0; i < bgItems.photos.Count(); i++)
                {
                    BGImages s = new BGImages();

                    s.ImageURLsmall = bgItems.photos[i].images[0].url.ToString().Replace("2.jpg", "3.jpg");
                    s.ImageURLbig = bgItems.photos[i].images[0].url.ToString().Replace("2.jpg", "4.jpg");

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

                    gvImages.ScrollIntoView(gvImages.Items[0]);

                    stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                }
                else
                {
                    stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    imageStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    stackMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    txtMessage.Text = "Unable load images now.  Please try later!";
                }

            }
            catch (Exception ex)
            {
                stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imageStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                stackMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                txtMessage.Text = "Unable load images now.  Please try later! \r\n " + ex.Message.ToString();
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
                genericURL = "https://api.500px.com/v1/photos/search?term=" + txtSearch.Text.Trim() + "&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&rpp=36&sort=votes_count&page=" + sessionData.currentPage;
            }
            else
            {
                genericURL = "https://api.500px.com/v1/photos?consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&rpp=36&sort=votes_count&page=" + sessionData.currentPage;
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
            Uri uri = new Uri(preview.Tag.ToString(), UriKind.Absolute);
            ImageSource newSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(uri);
            imgPreview.Source = newSource;
        }

        /// <summary>
        /// Click event of Background button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnBackground_Click(object sender, RoutedEventArgs e)
        {
            Button background = (Button)sender;

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
        }

        /// <summary>
        /// Click event of the Search button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            sessionData.currentBackgroundImages = null;
            gvImages.ItemsSource = null;
            stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //genericURL = "https://api.500px.com/v1/photos?search?term=" + txtSearch.Text.Trim() + "&sort=votes_count&exclude_nude=true&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4&rpp=40&page=" + sessionData.currentPage;
            genericURL = "https://api.500px.com/v1/photos/search?term=" + txtSearch.Text.Trim() + "&page=" + sessionData.currentPage + "&rpp=36&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&sort=votes_count&exclude_nude=true";
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
                CurrentApp.LicenseInformation.LicenseChanged -= licenseChangeHandler;
            }

            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new SettingsUserControl());
            }
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
    }

    public class BGImageListings
    {
        public string total_items { get; set; }
        public photos[] photos = { };
    }

    public class photos
    {
        public images[] images = { };
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
    }
}
