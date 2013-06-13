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
using Windows.UI.Popups;

namespace QuotesOfWisdom
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Background : Page
    {
        // creating bgImages list object
        List<BGImages> bglist = new List<BGImages>();
        int cnt = 0;
        public Background()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadBackgroundImages();
        }

        private async void LoadBackgroundImages()
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync("https://api.500px.com/v1/photos?consumer_key=it4eyt0SylP9boHkIM4IMh9cBVmy0NB9XuWGC4AK&image_size[]=3&image_size[]=4&rpp=16&license_type=6");
                var jsonbgImageresult = await response.Content.ReadAsStringAsync();
                jsonbgImageresult = jsonbgImageresult.Replace(@"\", "");

                var bgItems = JsonConvert.DeserializeObject<BGImageListings>(jsonbgImageresult);



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
                // Create the message dialog and set its content and title
                var messageDialog = new MessageDialog("Unable load images now.  Please try later!", "Network Error");

                // Add commands and set their command ids
                messageDialog.Commands.Add(new UICommand("Close", null, 0));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Show the message dialog and get the event that was invoked via the async operator
                var commandChosen = messageDialog.ShowAsync();

                if (commandChosen.Id == 0)
                {
                    // Use the navigation frame to return to the previous page
                    if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
                }               
            }
        }

        void BindBGImages()
        {
            try
            {
                // clears the main stack panel
                imageStackPanel.Children.Clear();

                // creating temp. stack panel
                StackPanel sptmp = new StackPanel();
                sptmp.Orientation = Orientation.Horizontal;
                
                for (var i = 0; i < bglist.Count; i++)
                {
                    // checks the conditions for displaying 4 items per row
                    if ((i % 4 == 0) && (i != 0))
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
                    }

                    cnt++;
                    try
                    {
                        // creates the stack panel object for adding image control
                        StackPanel sp = new StackPanel();
                        sp.Margin = new Thickness(0, 0, 20, 20);

                        // creates the image object and assigns the its Uri proerty
                        Image img = new Image();
                        Uri uri = new Uri(bglist[i].ImageURLsmall.ToString(), UriKind.Absolute);
                        ImageSource newSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(uri);
                        img.Source = newSource;
                        img.Margin = new Thickness(0, 0, 0, 0);
                        img.Height = 150;
                        img.Width = 150;

                        sp.Children.Add(img);
                        
                        // creates the stack panel object for adding button controls
                        StackPanel sp1 = new StackPanel();
                        sp1.Orientation = Orientation.Vertical;
                        sp1.Name = "sp#" + i.ToString();

                        // creating the Preview Button control
                        Button btnPreview = new Button();
                        btnPreview.Content = "Preview";
                        btnPreview.Margin = new Thickness(0, 10, 0, 10);
                        btnPreview.Tag = bglist[i].ImageURLbig.ToString();
                        btnPreview.Click += new RoutedEventHandler(btnPreview_Click);

                        // creating the Set as Background Button control
                        Button btnBackground = new Button();
                        btnBackground.Content = "Set as Background";
                        btnBackground.Tag = bglist[i].ImageURLbig.ToString();
                        btnBackground.Click += new RoutedEventHandler(btnBackground_Click);

                        // adds the buttons to the created stack panel
                        sp1.Children.Add(btnPreview);
                        sp1.Children.Add(btnBackground);

                        sptmp.Children.Add(sp);
                        sptmp.Children.Add(sp1);

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
        /// Method of changing background
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

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = true;
            Button preview = (Button)sender;
            Uri uri = new Uri(preview.Tag.ToString(), UriKind.Absolute);
            ImageSource newSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(uri);
            imgPreview.Source = newSource;            
        }

        private void btnBackground_Click(object sender, RoutedEventArgs e)
        {
            Button background = (Button)sender;
            ApplicationData.Current.RoamingSettings.Values["Settings"] = "dynamicStyle";
            ApplicationData.Current.RoamingSettings.Values["ImageURLForDynamicStyle"] = background.Tag.ToString();
            Utilities.dynamicBackgroundChange(LayoutRoot);
        }

        private void myPopup_Loaded_1(object sender, RoutedEventArgs e)
        {
            PopupLoaded();
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
}
