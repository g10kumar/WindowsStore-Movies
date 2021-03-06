﻿using System;
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
using Windows.UI.ApplicationSettings;
using QuotesOfWisdom.Common;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using System.Security;
using System.Net;
using System.ComponentModel;
using System.Net.Http;
using Windows.Data.Json;
using System.ServiceModel;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using System.Threading;

namespace QuotesOfWisdom
{
    public sealed partial class BackgroundModifiedUserControl :  UserControl 
    {
        // creating bgImages list object
        List<BGImages> bglist = new List<BGImages>();
        List<string> bglistphoto = new List<string>();
        int cnt = 0;
        //string genericURL = "";
        bool isBackgroundButtonVisible = true;
        LicenseChangedEventHandler licenseChangeHandler = null;
        //CancellationTokenSource cts = new CancellationTokenSource();
        
        public BackgroundModifiedUserControl()
        {
            
            this.InitializeComponent();
            sessionData.isBackgroundChanged = true;
            LoadFormData();
            stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;

            if (sessionData.genericURL != "")
            {
                sessionData.genericURL = "";
            }

            sessionData.genericURL = "https://api.500px.com/v1/photos?consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&exclude_nude=true&sort=votes_count&rpp=36";
           // sessionData.genericURL = "https://api.500px.com/v1/photos/search?term=bike&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4";
            DisplayImages();
        }

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

        private void DisplayImages()
        {
            try
            {
                stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                IncrementalSource<Flickr, FlickrPhoto> _data = new IncrementalSource<Flickr, FlickrPhoto>(sessionData.genericURL);
                gvImages.ItemsSource = _data;
                //txtSearch.Text = sessionData.searchKeyWord + " --- " + sessionData.totalImages.ToString();
                stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;                
            }
            catch {

                stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imageStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                stackMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                txtMessage.Text = "Unable load images now.  Please try later!";
            }
        }

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
            if (sessionData.isBackgroundChanged)
            {
                ChangeBackground();
                sessionData.isBackgroundChanged = false;
            }
        }

        private void ChangeBackground()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            {
                if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() != "dynamicStyle")
                {
                    //LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                    if ((string)ApplicationData.Current.RoamingSettings.Values["bgColor"].ToString() != "")
                    {
                        SolidColorBrush sbColorBrush = new SolidColorBrush(Utilities.HexColor(ApplicationData.Current.RoamingSettings.Values["bgColor"].ToString()));
                        LayoutRoot.Background = sbColorBrush;
                    }
                    else
                    {
                        SolidColorBrush sbColorBrush = new SolidColorBrush(Utilities.HexColor("#000000"));
                        LayoutRoot.Background = sbColorBrush;
                    }
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

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = true;
            Button preview = (Button)sender;
            Uri uri = new Uri(preview.Tag.ToString(), UriKind.Absolute);
            ImageSource newSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(uri);
            imgPreview.Source = newSource;
        }

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
        /// Click event of the back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackButtonClicked(object sender, RoutedEventArgs e)
        {
            //if (this.Parent.GetType() == typeof(Popup))
            //{
            //    ((Popup)this.Parent).IsOpen = false;
            //}
            //SettingsPane.Show();

            sessionData.isSearchClicked = false;
            sessionData.genericURL = "";
            isBackgroundButtonVisible = false;
            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            sessionData.tokenSource.Cancel();
            //sessionData.tokenSource.Dispose();
            //FlickrResponse fr = new FlickrResponse(null,0);            
            //CancellationToken token = cts.Token;
            //token.Register(() => cts.Cancel());
           // cts.Cancel();
            //IncrementalSource<Flickr, FlickrPhoto> iload = new IncrementalSource<Flickr, FlickrPhoto>("");
            gvImages.LoadMoreItemsAsync().Cancel();
            //iload.VirtualCount = sessionData.totalImages + 10;
            sessionData.isSearchClicked = true;
            //await gvImages.LoadMoreItemsAsync();
            var gridTemplate = gvImages.Template;
            gvImages.Template = null;

            stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //imageStackPanel.Children.Clear();

            if (sessionData.genericURL != "")
            {
                sessionData.genericURL = "";
            }

            if (txtSearch.Text != "")
            {
                sessionData.genericURL = "https://api.500px.com/v1/photos/search?term=" + txtSearch.Text.Trim() + "&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&rpp=36&sort=votes_count&exclude_nude=true";                            
                //genericURL = "https://api.500px.com/v1/photos?search?term=" + txtSearch.Text.Trim() + "&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK";
                //genericURL = "https://api.500px.com/v1/photos/search?term=bike&page=1&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4";
                //genericURL = "https://api.500px.com/v1/photos?search?term=" + txtSearch.Text.Trim() + "&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4&license_type=6";
                //genericURL = "https://api.500px.com/v1/photos?search?term=" + txtSearch.Text.Trim() + "&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4&only=" + txtSearch.Text.Trim();
                //genericURL = "https://api.500px.com/v1/photos?search?term=" + txtSearch.Text.Trim() + "&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&Consumer_Secret=umzO3TMSuooMAKbWGdw7J0tFSAQFJw3P0LTTksUv&image_size[]=3&image_size[]=4&&license_type=6";
                //genericURL = "https://api.500px.com/v1/photos?search?term=" + txtSearch.Text.Trim() + "&type=photos&consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4&&license_type=-1";

            }
            else
            {
                sessionData.genericURL = "https://api.500px.com/v1/photos?consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&exclude_nude=true&sort=votes_count&rpp=36";
                                         
            }
            this.gvImages.ItemsSource = null;
            sessionData.tokenSource = new CancellationTokenSource();
            gvImages.Template = gridTemplate;
            DisplayImages();
            //gvImages.ItemsSource = new IncrementalSource<Flickr, FlickrPhoto>(genericURL);
            //LoadBackgroundImages(genericURL);
        }

        private async void LoadBackgroundImages(string URL)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(URL);
                var jsonbgImageresult = await response.Content.ReadAsStringAsync();
                jsonbgImageresult = jsonbgImageresult.Replace(@"\", "");

                var bgItems = JsonConvert.DeserializeObject<BGImageListings>(jsonbgImageresult);

                if (bglist.Count > 0)
                {
                    bglist.Clear();
                }


                for (int i = 0; i < bgItems.photos.Count(); i++)
                {
                    BGImages s = new BGImages();

                    if (bgItems.photos[i].images[0].size == "3")
                    {
                        s.ImageURLsmall = bgItems.photos[i].images[0].url.ToString();
                    }

                    if (bgItems.photos[i].images[1].size == "4")
                    {
                        s.ImageURLbig = bgItems.photos[i].images[1].url.ToString();
                    }

                    bglist.Add(s);
                    s = null;
                }

                //bgImagesGridView.ItemsSource = bglist.ToList();
                BindBGImages();
            }
            catch
            {
                stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imageStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                stackMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                txtMessage.Text = "Unable load images now.  Please try later!";

                //// Create the message dialog and set its content and title
                //var messageDialog = new MessageDialog("Unable load images now.  Please try later!", "Network Error");

                //// Add commands and set their command ids
                //messageDialog.Commands.Add(new UICommand("Close", null, 0));

                //// Set the command that will be invoked by default
                //messageDialog.DefaultCommandIndex = 0;

                //// Show the message dialog and get the event that was invoked via the async operator
                //var commandChosen = messageDialog.ShowAsync();

                //if (commandChosen.Id == 0)
                //{
                //    // Use the navigation frame to return to the previous page
                //    if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
                //}               
            }
        }

        void BindBGImages()
        {
            try
            {
                imageStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                stackMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                // clears the main stack panel
                imageStackPanel.Children.Clear();

                // creating temp. stack panel
                StackPanel sptmp = new StackPanel();
                sptmp.Orientation = Orientation.Horizontal;
                sptmp.Margin = new Thickness(0, 0, 0, 15);

                for (var i = 0; i < bglist.Count; i++)
                {
                    // checks the conditions for displaying 4 items per row
                    if ((i % 3 == 0) && (i != 0))
                    {
                        cnt = 0;

                        // if we already added 4 items in the row then adds the temp. stack panel to 
                        // imageStackPanel and makes the temp. stack panel null
                        imageStackPanel.Children.Add(sptmp);
                        sptmp = null;
                    }
                    // checks if the temp. stack panel is null then create the temp. stack panel
                    if (sptmp == null)
                    {
                        sptmp = new StackPanel();
                        sptmp.Orientation = Orientation.Horizontal;
                        sptmp.Margin = new Thickness(0, 0, 0, 15);
                    }

                    cnt++;
                    try
                    {
                        StackPanel spMain = new StackPanel();
                        spMain.Orientation = Orientation.Vertical;
                        spMain.Margin = new Thickness(0, 0, 20, 5);

                        // creates the stack panel object for adding image control
                        StackPanel sp = new StackPanel();
                        sp.Margin = new Thickness(0, 0, 20, 5);
                        sp.Width = 250;
                        // creates the image object and assigns the its Uri proerty
                        Image img = new Image();
                        Uri uri = new Uri(bglist[i].ImageURLsmall.ToString(), UriKind.Absolute);
                        ImageSource newSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(uri);
                        img.Source = newSource;
                        img.Margin = new Thickness(0, 0, 0, 0);
                        img.Height = 150;
                        img.Width = 250;
                        img.Stretch = Stretch.UniformToFill;

                        sp.Children.Add(img);

                        // creates the stack panel object for adding button controls
                        StackPanel sp1 = new StackPanel();
                        sp1.Orientation = Orientation.Horizontal;
                        sp1.Name = "sp#" + i.ToString();

                        // creating the Preview Button control
                        Button btnPreview = new Button();
                        btnPreview.Content = "Preview";
                        btnPreview.Margin = new Thickness(0, 0, 0, 0);
                        btnPreview.FontSize = 16;
                        btnPreview.Tag = bglist[i].ImageURLbig.ToString();
                        btnPreview.Click += new RoutedEventHandler(btnPreview_Click);

                        // creating the Set as Background Button control
                        Button btnBackground = new Button();

                        if (isBackgroundButtonVisible)
                        {
                            btnBackground.Content = "Set as Background";
                        }
                        else
                        {
                            btnBackground.Content = "Buy Images";
                        }
                        btnBackground.FontSize = 16;
                        btnBackground.Margin = new Thickness(0, 0, 0, 0);
                        btnBackground.Tag = bglist[i].ImageURLbig.ToString();
                        btnBackground.Click += new RoutedEventHandler(btnBackground_Click);

                        // adds the buttons to the created stack panel
                        sp1.Children.Add(btnPreview);
                        sp1.Children.Add(btnBackground);

                        //sptmp.Children.Add(sp);
                        //sptmp.Children.Add(sp1);

                        spMain.Children.Add(sp);
                        spMain.Children.Add(sp1);

                        sptmp.Children.Add(spMain);

                    }
                    catch
                    {

                    }
                }
                imageStackPanel.Children.Add(sptmp);
            }
            catch
            {

            }

            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void myPopup_Loaded_1(object sender, RoutedEventArgs e)
        {
            //  PopupLoaded();
        }

        private void PopupLoaded()
        {
            myPopup.HorizontalOffset = (Window.Current.Bounds.Width - myPopup.ActualWidth) / 2;
            myPopup.VerticalOffset = (Window.Current.Bounds.Height - LayoutRoot.ActualHeight) / 2;
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = false;
        }
    }   
}

public class BGImageListings
{
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
