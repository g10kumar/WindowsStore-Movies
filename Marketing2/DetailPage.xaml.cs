using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.Foundation;
using Windows.ApplicationModel.Store;
namespace Marketing2
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class DetailPage : Marketing2.Common.LayoutAwarePage
    {
        public DataTransferManager datatransferManager;
        FeedItem sharedFeedItem;
        public DetailPage()
        {
            this.InitializeComponent();
            //ShareSourceLoad();
            
        }

        public void ShareSourceLoad()
        {
            datatransferManager = DataTransferManager.GetForCurrentView();
            datatransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }

        void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            //FeedItem selectedItem = this.itemListView.SelectedItem as FeedItem;
            if (sharedFeedItem != null && this.Frame != null)
            {
                e.Request.Data.Properties.Title = sharedFeedItem.Title;
                e.Request.Data.Properties.Description = sharedFeedItem.Title + "-" + sharedFeedItem.Author + Environment.NewLine + sharedFeedItem.Link.AbsoluteUri.ToString();
                e.Request.Data.SetText(sharedFeedItem.Title + "-" + sharedFeedItem.Author + Environment.NewLine + sharedFeedItem.Link.AbsoluteUri.ToString());
                e.Request.Data.SetUri(sharedFeedItem.Link);
                e.Request.Data.SetHtmlFormat(sharedFeedItem.Title + "<br /> - " + sharedFeedItem.Author + "<br /><br />" + sharedFeedItem.Link.AbsoluteUri.ToString() + "<br /><br />" + "Shared from <a href='" + CurrentAppSimulator.LinkUri + "'>Marketing</a> app.");
            }
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Run the PopInThemeAnimation 
            Windows.UI.Xaml.Media.Animation.Storyboard sb =
                this.FindName("PopInStoryboard") as Windows.UI.Xaml.Media.Animation.Storyboard;
            if (sb != null) sb.Begin();

            // Add this code to navigate the web view to the selected blog post.
            //string itemTitle = (string)navigationParameter;
            //FeedItem feedItem = FeedDataSource.GetItem(itemTitle);
            //sharedFeedItem = FeedDataSource.GetItem(itemTitle);

            //if (feedItem != null)
            //{
            //    this.contentView.Navigate(feedItem.Link);
            //    this.DataContext = feedItem;
            //}

            string itemTitle = (string)navigationParameter;
            FeedItem feedItem = FeedDataSource.GetItem(itemTitle);
            sharedFeedItem = FeedDataSource.GetItem(itemTitle);

            if (feedItem != null)
            {
                this.contentView.Navigate(feedItem.Link);
                this.DataContext = feedItem;

                ShareSourceLoad();
            }            
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void contentView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            string errorString = "<p>Page could not be loaded.</p><p>Error is: " + e.WebErrorStatus.ToString() + "</p>";
            this.contentView.NavigateToString(errorString);
        }
    }
}