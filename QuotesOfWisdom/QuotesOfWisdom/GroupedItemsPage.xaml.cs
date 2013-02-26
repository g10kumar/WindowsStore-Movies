using QuotesOfWisdom.Data;

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
using System.Threading.Tasks;
// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231
using QuotesOfWisdom.NotificationsExtensions.TileContent;
using Windows.UI.Notifications;
using Windows.Storage;
using QuotesOfWisdom.Common;


///////
//using System.Xml;
//using System.Xml.Serialization;
//using System.Text;
//using System.Xml.Linq;
//using Windows.Data.Xml;
//using System.Collections.ObjectModel;
//using Windows.Graphics.Display;
//using Windows.Storage;
//using Windows.Storage.AccessCache;
//using Windows.Storage.FileProperties;
//using Windows.Storage.Streams;
//using Windows.System;
//using Windows.UI.Core;
//using System.Threading.Tasks;
//using System.Net.Http;
//using Windows.Storage.Pickers;
//using Windows.UI.Notifications;
//using Windows.ApplicationModel.DataTransfer;

using Windows.UI.ViewManagement;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;

namespace QuotesOfWisdom
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : QuotesOfWisdom.Common.LayoutAwarePage
    {
        #region Objects
        public DataTransferManager datatransferManager;
        LicenseChangedEventHandler licenseChangeHandler = null;
        #endregion
        public GroupedItemsPage()
        {
            
            this.InitializeComponent();
            ShareSourceLoad();
            //ChangeBackground();
        }

        /// <summary>
        /// Method for Declaring and Initializing datatransfer object
        /// </summary>
        public void ShareSourceLoad()
        {
            datatransferManager = DataTransferManager.GetForCurrentView();
            datatransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }

        /// <summary>
        /// Async method for data sharing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            e.Request.Data.Properties.Title = "Quotes of Wisdom on Windows 8";
            e.Request.Data.Properties.Description = "Quotes of Wisdom ";
            e.Request.Data.SetText("Great Quotations app on Windows 8 - check out Quotes of Wisdom");
        }
        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            await LoadInAppPurchaseProxyFileAsync();

            //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;

            var productLicense = licenseInformation.ProductLicenses["AdFreeVersion"];
            if (productLicense.IsActive)
            {
                adFull.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                adSnapped.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                adFull.Visibility = Windows.UI.Xaml.Visibility.Visible;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            sessionData.isSearch = false;
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            //var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            //this.DefaultViewModel["Groups"] = sampleDataGroups;
            var quotesGroups = Quotes.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = quotesGroups;

            #region Tile Notification

            try
            {
                Uri polledUri = new Uri("http://apps.daksatech.com/quoteService/QuotesTileXML.html");
                PeriodicUpdateRecurrence recurrence = PeriodicUpdateRecurrence.Daily;
                TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdate(polledUri, recurrence);
            }
            catch (Exception ex)
            {
                //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                //dialog.ShowAsync();
            }
            datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            #endregion  
        }

        private async Task LoadInAppPurchaseProxyFileAsync()
        {
            //StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("Xml");
            //StorageFile proxyFile = await proxyDataFolder.GetFileAsync("in-app-purchase.xml");
            licenseChangeHandler = new LicenseChangedEventHandler(InAppPurchaseRefreshScenario);

            //CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
            CurrentApp.LicenseInformation.LicenseChanged += licenseChangeHandler;
            //await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);
        }

        private void InAppPurchaseRefreshScenario()
        {
            //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["AdFreeVersion"];
            if (productLicense.IsActive)
            {
                //rootPage.NotifyUser("You can use Product 1.", NotifyType.StatusMessage);
                adFull.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                adSnapped.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                string visualState = DetermineVisualState(ApplicationView.Value);
                if (visualState == "Snapped")
                {
                    adSnapped.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    adFull.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    adFull.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    adSnapped.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //rootPage.NotifyUser("You don't own Product 1. You must buy Product 1 before you can use it.", NotifyType.ErrorMessage);
            }
        }



        /// <summary>
        /// Invoked when this page is about to unload
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (licenseChangeHandler != null)
            {
                //CurrentAppSimulator.LicenseInformation.LicenseChanged -= licenseChangeHandler;
                CurrentApp.LicenseInformation.LicenseChanged -= licenseChangeHandler;
            }
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(GroupDetailPage), ((QuotesGroup)group).Title);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((QuotesItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
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
                //if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "Generic")
                //{
                //    SolidColorBrush s = new SolidColorBrush();
                //    s.Color = Utilities.GetColor(sessionData.colorValue);
                //    LayoutRoot.Background = s;
                //}
                //else 
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
                {
                    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                }
                else
                {
                    LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
                    //Style style = new Style(typeof(Grid));
                    //style.Setters.Add(new Setter(Grid.BackgroundProperty, ApplicationData.Current.RoamingSettings.Values["Settings"]));
                    //LayoutRoot.Style = style;
                }
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
            }

            //Style style = new Style(typeof(Grid));
            //style.Setters.Add(new Setter(Grid.BackgroundProperty, ApplicationData.Current.RoamingSettings.Values["Settings"]));
            //LayoutRoot.Style = style;
        }

        private void pageRoot_GotFocus_1(object sender, RoutedEventArgs e)
        {
            ChangeBackground();
        }

        private async void btnBuyAdFree_Click_1(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["AdFreeVersion"];
            if (!productLicense.IsActive)
            {
                try
                {
                    //await CurrentAppSimulator.RequestProductPurchaseAsync("AdFreeVersion", false);
                    await CurrentApp.RequestProductPurchaseAsync("AdFreeVersion", false);
                    if (productLicense.IsActive)
                    {
                        this.ShowMessage("You bought the Ad Free version.");
                    }
                }
                catch (Exception)
                {
                    this.ShowMessage("Unable to buy Ad Free version.");
                }
            }
            else
            {
                this.ShowMessage("You already own Ad Free version of app.");
            }
        }

        async void ShowMessage(string msg)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }
    }
}
