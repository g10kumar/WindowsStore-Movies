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
        DateTime dateLastRefreshed;
        bool performReload = false;
        public ItemsPage()
        {
            this.InitializeComponent();
            //ApplicationData.Current.RoamingSettings.Values["dateLastRefreshed"] = null;
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
            if (ApplicationData.Current.LocalSettings.Values["dateLastRefreshed"] != null)
            {
                dateLastRefreshed = DateTime.Parse(ApplicationData.Current.LocalSettings.Values["dateLastRefreshed"].ToString());
            }

            stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Layout.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //FeedDataSource feedDataSource = (FeedDataSource)App.Current.Resources["feedDataSource"];


            if (sessionData.currentFeeds == null || dateLastRefreshed == null)
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
                        if (feedDataSource != null)
                        {
                            if (feedDataSource.Feeds.Count == 0)
                            {
                                await feedDataSource.GetFeedsAsync();
                            }
                        }
                        //await feedDataSource.GetFeedsAsync();
                        lstFeedData = feedDataSource.Feeds.ToList();
                        //feedRefreshed = true;

                        if (lstFeedData != null)
                        {
                            await LocalStorage.SaveAsync<FeedData>(lstFeedData);
                            ApplicationData.Current.LocalSettings.Values["dateLastRefreshed"] = DateTime.Now.ToString();
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

                if (dateLastRefreshed != null && DateTime.Compare(DateTime.Now, dateLastRefreshed.AddMinutes(60)) > 0)
                {
                    await Task.Run(() => refreshFeeds());
                    dateLastRefreshed = DateTime.Now;
                    ApplicationData.Current.LocalSettings.Values["dateLastRefreshed"] = dateLastRefreshed.ToString();

                }
            }
            else
            {
                if (sessionData.currentFeeds != null)
                {
                    if (sessionData.currentFeeds.Count != 0)
                    {
                        lstFeedData = sessionData.currentFeeds.ToList();
                        this.DefaultViewModel["Items"] = lstFeedData;
                        stackProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        Layout.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
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