using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Linq;
using Windows.UI.Xaml;
using System.Threading.Tasks;

namespace Marketing2
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class ItemsPage : Marketing2.Common.LayoutAwarePage
    {
        List<FeedData> lstFeedData = new List<FeedData>();
        //List<FeedData> lstfeedDataSource = new List<FeedData>();
        bool feedRefreshed = false;
        bool performReload = false;
        public ItemsPage()
        {
            this.InitializeComponent();
            ApplicationData.Current.RoamingSettings.Values["dateLastRefreshed"] = null;
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
            //FeedData feedData = new FeedData();
            

            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Layout.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            DateTime lastRefreshedTime = DateTime.Parse(ApplicationData.Current.RoamingSettings.Values["dateLastRefreshed"].ToString());

            if (sessionData.currentFeeds == null || lastRefreshedTime == null || 
                    (lastRefreshedTime != null && DateTime.Compare(DateTime.Now, lastRefreshedTime) > 5))
            {
                try
                {
                    lstFeedData = await LocalStorage.RestoreAsync<FeedData>();
                }
                catch { }
            
                if (lstFeedData == null)
                {
                    var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                    if (connectionProfile != null)
                    {
                        FeedDataSource feedDataSource = new FeedDataSource();
                        await feedDataSource.GetFeedsAsync();
                        lstFeedData = feedDataSource.Feeds.ToList();
                        feedRefreshed = true;
                        if (lstFeedData != null)
                        {
                            await LocalStorage.SaveAsync<FeedData>(lstFeedData);
                            ApplicationData.Current.RoamingSettings.Values["dateLastRefreshed"] = DateTime.Now;
                        }
                    }
                    else
                    {
                        var messageDialog = new Windows.UI.Popups.MessageDialog("An internet connection is needed to download feeds. Please check your connection and restart the app.");
                        var result = messageDialog.ShowAsync();
                        stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }

                }


                if (lstFeedData != null)
                {
                    sessionData.currentFeeds = lstFeedData.ToList();
                    this.DefaultViewModel["Items"] = lstFeedData;
                    stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Layout.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }

                if (!feedRefreshed)
                {
                    await Task.Run(() => refreshFeeds());
                }
            }


            //{
              
            //    if (sessionData.currentFeeds != null && sessionData.currentFeeds.Count == 0)
            //    {
            //        lstFeedData = await LocalStorage.RestoreAsync<FeedData>();
            //    }

            //    lstFeedData = sessionData.currentFeeds;                

            //}
            


            //if (lstFeedData != null)
            //{
            //    stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //    Layout.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //    this.DefaultViewModel["Items"] = lstFeedData;

            //    await feedDataSource.GetFeedsAsync();
            //    lstFeedData = feedDataSource.Feeds.ToList();
            //    await LocalStorage.SaveAsync<FeedData>(lstFeedData);
            //    sessionData.currentFeeds = lstFeedData.ToList();
                
            //    this.DefaultViewModel["Items"] = lstFeedData;

            //    //Frame frame = new Frame();
            //    //frame.Navigate(typeof(ItemsPage));
            //    //Window.Current.Content = frame;
            //    //// Ensure the current window is active
            //    //Window.Current.Activate();

            //}
        }

        private async Task refreshFeeds()
        {
            FeedDataSource feedDataSource = new FeedDataSource();
            var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (connectionProfile != null)
            {
                await feedDataSource.GetFeedsAsync();
                lstFeedData = feedDataSource.Feeds.ToList();
                feedRefreshed = true;
                if (lstFeedData != null)
                {
                    await LocalStorage.SaveAsync<FeedData>(lstFeedData);
                }
                sessionData.currentFeeds = lstFeedData.ToList();
            }
            else
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("An internet connection is needed to download feeds. Please check your connection and restart the app.");
                var result = messageDialog.ShowAsync();
                stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the split page, configuring the new page
            // by passing the title of the clicked item as a navigation parameter
            if (e.ClickedItem != null)
            {
                string title = ((FeedData)e.ClickedItem).Title;
                this.Frame.Navigate(typeof(SplitPage), title);
            }
        }
    }
}