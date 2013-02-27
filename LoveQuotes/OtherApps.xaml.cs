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

using Windows.UI.Core;
using Windows.UI.ViewManagement;
using System.Collections.ObjectModel;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;


using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

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

namespace LoveQuotes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OtherApps : Page
    {
        private Random rndm;
        Utilities u = new Utilities();

        public OtherApps()
        {
            this.InitializeComponent();
            LoadApplicationData();
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            string visualState = DetermineVisualState(ApplicationView.Value);

            if (visualState == "Snapped")
            {
                appFullMode.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                appSnappedMode.Visibility = Windows.UI.Xaml.Visibility.Visible;
                backButton.Style = App.Current.Resources["SnappedBackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["SnappedPageHeaderTextStyle"] as Style;
            }
            else
            {
                appFullMode.Visibility = Windows.UI.Xaml.Visibility.Visible;
                appSnappedMode.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                backButton.Style = App.Current.Resources["BackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["PageHeaderTextStyle"] as Style;
            }

            
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

        private async void LoadApplicationData()
        {
            //Checking whether Network Connection available or not
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                var client = new HttpClient();
                var response = await client.GetAsync("http://ads.daksatech.com/Windows8AppsList.htm?id=1");
                var jsonresult = await response.Content.ReadAsStringAsync();
                jsonresult = @"{""results"": " + jsonresult + "}";
                
                //return contents from json serialize object
                var myApplicationItems = JsonConvert.DeserializeObject<ApplicationListings>(jsonresult);

                // creating application list object
                List<DaksaApplication> applist = new List<DaksaApplication>();

                rndm = new Random();
                DaksaApplications = new ObservableCollection<DaksaApplication>();

                for (int i = 0; i < myApplicationItems.results.Count(); i++)
                {
                    string costprice = "";

                    if (myApplicationItems.results[i].cost == "0")
                    {
                        costprice = "Free";
                    }
                    else
                    {
                        costprice = "$ " + myApplicationItems.results[i].cost;
                    }
                    DaksaApplications.Add(new DaksaApplication(myApplicationItems.results[i].appname, myApplicationItems.results[i].logo, GetInterval(),costprice, myApplicationItems.results[i].category, myApplicationItems.results[i].description, myApplicationItems.results[i].features, myApplicationItems.results[i].storeurl, "Transparent", "Transparent", myApplicationItems.results[i].bkgcolor));
                }
                
            }
            else
            {

                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Cannot connect to Internet. Please try the app when you have Internet connection.");
                await dialog.ShowAsync();
            }

            tileView.ItemsSource = DaksaApplications;
            tileView.SelectedIndex = 1;
            itemListView.ItemsSource = DaksaApplications;
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        async void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((DaksaApplication)e.ClickedItem).StoreURL;
            await Windows.System.Launcher.LaunchUriAsync(new Uri(itemId.ToString()));
        }

        /// <summary>
        /// Class definition of FeaturedHomeListings
        /// </summary>
        public class ApplicationListings
        {
            public results[] results = { };
        }

        /// <summary>
        /// Class definition of FeaturedHomeListings results
        /// </summary>
        public class results
        {
            public string appname { get; set; }
            public string category { get; set; }
            public string cost { get; set; }
            public string description { get; set; }
            public string features { get; set; }
            public string logo { get; set; }
            public string storeurl { get; set; }
            public string bkgcolor { get; set; }
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
            if (ApplicationData.Current.RoamingSettings.Values["Settings"] != null)
            {
                // Initialize the Radio button from roaming settings
                if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "Generic")
                {
                    SolidColorBrush s = new SolidColorBrush();
                    s.Color = Utilities.GetColor(sessionQuotes.colorValue);
                    LayoutRoot.Background = s;
                }
                else if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
                {
                    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                }
                else
                {
                    LayoutRoot.Style = App.Current.Resources["layoutBlockStyle_f2b100"] as Style;
                }
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle_f2b100"] as Style;
            }
        }

        public double GetInterval()
        {
            return rndm.Next(18, 25);
        }

        private ObservableCollection<DaksaApplication> daksaapplications;

        public ObservableCollection<DaksaApplication> DaksaApplications
        {
            get { return daksaapplications; }
            set { daksaapplications = value; }
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void btnStoreURL_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            await Windows.System.Launcher.LaunchUriAsync(new Uri(btn.Tag.ToString()));
        }

        /// <summary>
        /// Click event of the Back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Use the navigation frame to return to the previous page
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }
    }

    public class DaksaApplication
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public TimeSpan Interval { get; set; }

        public string Cost { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string Features { get; set; }

        public string StoreURL { get; set; }

        public string TileColor { get; set; }

        public string HeaderColor { get; set; }

        public string BkgColor { get; set; }

        public DaksaApplication()
        { }

        public DaksaApplication(string name, string image, double seconds, string cost, string category, string description, string features, string storeURL, string color, string headercolor, string bkgcolor)
        {
            Name = name;
            Image = image;
            Interval = TimeSpan.FromSeconds(seconds);
            Cost = cost;
            Category = category;
            Description = description;
            Features = features;
            StoreURL = storeURL;
            TileColor = color;
            HeaderColor = headercolor;
            BkgColor = bkgcolor;
        }
    }
}
